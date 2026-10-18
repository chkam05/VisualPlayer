using System.Collections.ObjectModel;
using System.Reflection;
using VisualPlayer.VisualizationBase.Localization;
using VisualPlayer.VisualizationBase.Plugins;
using VisualPlayer.Visualizations.Registry;

namespace VisualPlayer.Visualizations.Loading;

public sealed record VisualizationPackage(string Directory, VisualizationPackageSource Source, VisualizationPackageManifest Manifest);
public sealed record VisualizationPluginDiagnostic(string Path, string? PluginId, VisualizationDiagnosticLevel Level, string Message, Exception? Exception = null);

/// <summary>Startup-only loading. Contexts remain alive for the process; this is not a sandbox or hot reload.</summary>
public sealed class VisualizationPluginLoader : IDisposable
{
    private readonly VisualizationRegistry _registry;
    private readonly TranslationService _translations;
    private readonly ITranslationScope _common;
    private readonly Assembly[] _shared;
    private readonly Action<VisualizationPluginDiagnostic>? _report;
    private readonly ObservableCollection<VisualizationPluginDiagnostic> _diagnostics = [];
    private readonly List<(VisualizationPackage Package, VisualizationLoadContext Context)> _loaded = [];
    private bool _started, _disposed;
    public ReadOnlyObservableCollection<VisualizationPluginDiagnostic> Diagnostics { get; }
    public IReadOnlyList<VisualizationPackage> LoadedPackages => _loaded.Select(p => p.Package).ToArray();

    public VisualizationPluginLoader(VisualizationRegistry registry, TranslationService translations, ITranslationScope common,
        IEnumerable<Assembly> sharedAssemblies, Action<VisualizationPluginDiagnostic>? report = null)
    {
        _registry = registry; _translations = translations; _common = common; _shared = sharedAssemblies.ToArray(); _report = report;
        Diagnostics = new(_diagnostics);
    }

    public IReadOnlyList<VisualizationPackage> Discover(IEnumerable<VisualizationPluginRoot> roots)
    {
        var found = new List<VisualizationPackage>();
        foreach (var root in roots.DistinctBy(r => (System.IO.Path.GetFullPath(r.Path), r.Source)))
        {
            if (!System.IO.Directory.Exists(root.Path)) continue;
            try
            {
                foreach (var directory in System.IO.Directory.GetDirectories(root.Path).Order(StringComparer.Ordinal))
                {
                    // Dependencies are never inspected as entry points. Every active package needs its manifest.
                    if (!File.Exists(System.IO.Path.Combine(directory, "manifest.json"))) continue;
                    try { found.Add(new(System.IO.Path.GetFullPath(directory), root.Source, VisualizationPackageManifest.Read(directory))); }
                    catch (Exception exception) { Report(directory, null, "Cannot read compatible package manifest.", exception); }
                }
            }
            catch (Exception exception) { Report(root.Path, null, "Cannot scan visualization directory.", exception); }
        }
        return found;
    }

    public void Load(IEnumerable<VisualizationPluginRoot> roots)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (_started) throw new InvalidOperationException("Visualization loading is startup-only; restart to change packages.");
        _started = true;
        // Rank every candidate before executing any plugin code. User packages win, then version and ordinal path.
        var groups = Discover(roots).GroupBy(p => p.Manifest.Id, StringComparer.Ordinal).OrderBy(g => g.Key, StringComparer.Ordinal);
        foreach (var group in groups)
        {
            if (_registry.Find(group.Key) is not null)
            {
                Report(group.First().Directory, group.Key, "Plugin ID already registered by the host.");
                continue;
            }
            foreach (var package in group.OrderByDescending(p => p.Source).ThenByDescending(p => Version.Parse(p.Manifest.Version))
                         .ThenBy(p => p.Directory, StringComparer.Ordinal))
                if (TryLoad(package)) break;
        }
    }

    private bool TryLoad(VisualizationPackage package)
    {
        var registeredCatalog = false;
        IVisualizationPlugin? plugin = null;
        try
        {
            var manifest = package.Manifest;
            var entry = PackagePath.Resolve(package.Directory, manifest.EntryAssembly);
            var context = new VisualizationLoadContext(entry, _shared);
            var assembly = context.LoadFromAssemblyPath(entry);
            var type = assembly.GetType(manifest.EntryType, throwOnError: true)!;
            if (!type.IsPublic || type.IsAbstract || !typeof(IVisualizationPlugin).IsAssignableFrom(type))
                throw new TypeLoadException("Entry type must be a public concrete IVisualizationPlugin with a public parameterless constructor.");
            plugin = (IVisualizationPlugin)(Activator.CreateInstance(type) ?? throw new TypeLoadException("Cannot create plugin."));
            var descriptor = plugin.Descriptor;
            if (descriptor.Id != manifest.Id || descriptor.PluginVersion != Version.Parse(manifest.Version)
                || descriptor.ApiMajor != manifest.ApiMajor || descriptor.MinimumApiMinor != manifest.MinimumApiMinor)
                throw new InvalidDataException("Manifest and runtime descriptor disagree.");
            var catalog = _translations.RegisterScope(manifest.Id, plugin.EnglishTranslations,
                PackagePath.Resolve(package.Directory, manifest.LanguageDirectory));
            registeredCatalog = true;
            var services = new Services(catalog, _common, (level, message, exception) => Report(package.Directory, manifest.Id, message, exception, level));
            _registry.Register(plugin, services);
            _loaded.Add((package, context));
            foreach (var error in catalog.LoadErrors) Report(package.Directory, manifest.Id, error, level: VisualizationDiagnosticLevel.Warning);
            return true;
        }
        catch (Exception exception)
        {
            if (plugin is not null && ReferenceEquals(_registry.Find(package.Manifest.Id)?.Plugin, plugin))
            {
                try { _registry.Unregister(package.Manifest.Id); }
                catch (Exception cleanup) { Report(package.Directory, package.Manifest.Id, "Cannot detach failed registration.", cleanup); }
            }
            if (registeredCatalog) _translations.UnregisterScope(package.Manifest.Id);
            Report(package.Directory, package.Manifest.Id, "Cannot activate visualization package; trying the next compatible candidate.", exception);
            return false;
        }
    }

    private void Report(string path, string? id, string message, Exception? exception = null,
        VisualizationDiagnosticLevel level = VisualizationDiagnosticLevel.Error)
    {
        var diagnostic = new VisualizationPluginDiagnostic(path, id, level, message, exception);
        _diagnostics.Add(diagnostic);
        try { _report?.Invoke(diagnostic); } catch { /* A logger cannot break startup or rendering. */ }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        foreach (var loaded in _loaded)
        {
            _registry.Unregister(loaded.Package.Manifest.Id);
            _translations.UnregisterScope(loaded.Package.Manifest.Id);
        }
        _loaded.Clear();
    }
    private sealed class Services(ITranslationScope local, ITranslationScope common,
        Action<VisualizationDiagnosticLevel, string, Exception?> report) : IVisualizationHostServices
    {
        public ITranslationScope Translations => local;
        public ITranslationScope CommonTranslations => common;
        public void ReportDiagnostic(VisualizationDiagnosticLevel level, string message, Exception? exception = null)
            => report(level, message, exception);
    }
}
