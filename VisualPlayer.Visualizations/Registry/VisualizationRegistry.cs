using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Avalonia.Controls;
using VisualPlayer.ViewModels;
using VisualPlayer.VisualizationBase.Localization;
using VisualPlayer.VisualizationBase.Plugins;
using VisualPlayer.Visualizations.Config.Base;

namespace VisualPlayer.Visualizations.Registry;

public sealed class VisualizationRegistration : ViewModelBase, IDisposable
{
    public IVisualizationPlugin Plugin { get; }
    public IVisualizationHostServices Services { get; }
    public VisualizationDescriptor Descriptor { get; }
    public IReadOnlyList<VisualizationPreset> Presets { get; }
    public string Id => Descriptor.Id;
    public string Name => Services.Translations[Descriptor.NameKey];
    private readonly Action _languageChanged;

    internal VisualizationRegistration(IVisualizationPlugin plugin, IVisualizationHostServices services,
        VisualizationDescriptor descriptor, IReadOnlyList<VisualizationPreset> presets, Action languageChanged)
    {
        Plugin = plugin;
        Services = services;
        Descriptor = descriptor;
        Presets = presets;
        _languageChanged = languageChanged;
        services.Translations.LanguageChanged += LanguageChanged;
    }

    private void LanguageChanged(object? sender, EventArgs e)
    {
        NotifyPropertyChanged(nameof(Name));
        _languageChanged();
    }

    public Control? CreateEditor(VisualisationProfile profile)
    {
        if (!profile.IsAvailable || profile.Config is null || profile.PluginId != Id) return null;
        return Plugin.CreateEditor(new VisualizationEditorContext(profile.Config, profile.GetColorConfig(), Services, Descriptor.Capabilities));
    }

    public void Report(string message, Exception? exception = null)
    {
        // Diagnostics supplied by a plugin must not turn a recoverable plugin failure into a host failure.
        try { Services.ReportDiagnostic(VisualizationDiagnosticLevel.Error, $"{Id}: {message}", exception); }
        catch { /* Preserve the original plugin failure. */ }
    }

    public void Dispose() => Services.Translations.LanguageChanged -= LanguageChanged;
}

/// <summary>UI-thread registry; no references to concrete visualizations or the UI library.</summary>
public sealed class VisualizationRegistry : IDisposable
{
    public const string NoneId = "visualplayer.none";
    private readonly ObservableCollection<VisualizationRegistration> _items = [];
    private readonly Dictionary<string, VisualizationRegistration> _byId = new(StringComparer.Ordinal);
    public ReadOnlyObservableCollection<VisualizationRegistration> Available { get; }
    public event EventHandler? Changed;
    public event EventHandler? LanguageChanged;

    public VisualizationRegistry(IVisualizationHostServices? noneServices = null)
    {
        Available = new(_items);
        Register(new NoneVisualizationPlugin(), noneServices ?? new DefaultServices());
    }

    public VisualizationRegistration Register(IVisualizationPlugin plugin, IVisualizationHostServices services)
    {
        ArgumentNullException.ThrowIfNull(plugin);
        ArgumentNullException.ThrowIfNull(services);
        var descriptor = plugin.Descriptor;
        if (!Regex.IsMatch(descriptor.Id, "^[a-z0-9][a-z0-9.-]*$")) throw new ArgumentException("Invalid visualization ID.");
        if (_byId.ContainsKey(descriptor.Id)) throw new ArgumentException($"Visualization already registered: {descriptor.Id}");
        if (descriptor.ApiMajor != VisualizationApi.Major || descriptor.MinimumApiMinor > VisualizationApi.Minor)
            throw new NotSupportedException($"Incompatible plugin API: {descriptor.Id}");
        if (descriptor.ConfigVersion < 1 || string.IsNullOrWhiteSpace(descriptor.NameKey)) throw new ArgumentException("Invalid descriptor.");
        var presets = plugin.DefaultPresets.Select(preset => preset with
        {
            Configuration = preset.Configuration.Clone(), Color = preset.Color?.Copy()
        }).ToArray();
        if (presets.Select(p => p.Id).Distinct(StringComparer.Ordinal).Count() != presets.Length
            || presets.Any(p => string.IsNullOrWhiteSpace(p.Id) || p.ConfigVersion < 1 || string.IsNullOrWhiteSpace(p.NameKey)))
            throw new ArgumentException("Invalid default presets.");
        var registration = new VisualizationRegistration(plugin, services, descriptor, presets,
            () => LanguageChanged?.Invoke(this, EventArgs.Empty));
        _byId.Add(registration.Id, registration);
        _items.Add(registration);
        Changed?.Invoke(this, EventArgs.Empty);
        return registration;
    }

    public VisualizationRegistration? Find(string id) => _byId.GetValueOrDefault(id);

    public bool Unregister(string id)
    {
        if (id == NoneId) throw new InvalidOperationException("The no-visualization option is always available.");
        if (!_byId.Remove(id, out var registration)) return false;
        _items.Remove(registration);
        registration.Dispose();
        Changed?.Invoke(this, EventArgs.Empty);
        return true;
    }

    public void Dispose()
    {
        foreach (var registration in _items) registration.Dispose();
    }

    private sealed class DefaultServices : IVisualizationHostServices
    {
        public ITranslationScope Translations { get; } = new TranslationService().RegisterScope(NoneId, NoneVisualizationPlugin.English);
        public ITranslationScope CommonTranslations => Translations;
        public void ReportDiagnostic(VisualizationDiagnosticLevel level, string message, Exception? exception = null) { }
    }
}
