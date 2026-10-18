using System.Reflection;
using System.Runtime.Loader;
using VisualPlayer.Interfaces;

namespace VisualPlayer.Visualizations.Loading;

internal sealed class VisualizationLoadContext : AssemblyLoadContext
{
    private readonly AssemblyDependencyResolver _resolver;
    private readonly string _directory;
    private readonly Dictionary<string, Assembly> _shared;
    public VisualizationLoadContext(string entryAssembly, IEnumerable<Assembly> shared)
        : base($"Visualization:{entryAssembly}", isCollectible: false)
    {
        _resolver = new AssemblyDependencyResolver(entryAssembly);
        _directory = Path.GetDirectoryName(entryAssembly)!;
        _shared = shared.Append(typeof(IVisualization).Assembly).Distinct()
            .ToDictionary(a => a.GetName().Name!, StringComparer.OrdinalIgnoreCase);
    }
    protected override Assembly? Load(AssemblyName name)
    {
        if (name.Name is null) return null;
        if (_shared.TryGetValue(name.Name, out var shared)) return RequireVersion(name, shared);
        if (name.Name.StartsWith("Avalonia", StringComparison.Ordinal) || name.Name.StartsWith("Lucide", StringComparison.Ordinal)
            || name.Name == "VisualPlayer.UI") return RequireVersion(name, Default.LoadFromAssemblyName(name));
        if (name.Name is "VisualPlayer" or "VisualPlayer.Visualizations") throw new FileLoadException("Plugin must not reference the application/runtime.");
        var path = _resolver.ResolveAssemblyToPath(name);
        return path is null ? null : LoadFromAssemblyPath(RequirePackagePath(path));
    }
    protected override nint LoadUnmanagedDll(string name)
    {
        var path = _resolver.ResolveUnmanagedDllToPath(name);
        return path is null ? 0 : LoadUnmanagedDllFromPath(RequirePackagePath(path));
    }
    private string RequirePackagePath(string path)
        => PackagePath.Resolve(_directory, Path.GetRelativePath(_directory, path));
    private static Assembly RequireVersion(AssemblyName requested, Assembly shared)
    {
        if (requested.Version is not null && requested.Version != shared.GetName().Version)
            throw new FileLoadException($"Incompatible shared dependency: {requested.FullName}");
        return shared;
    }
}
