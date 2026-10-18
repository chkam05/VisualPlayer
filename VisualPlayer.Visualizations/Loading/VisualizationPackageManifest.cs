using System.Text.Json;
using System.Text.RegularExpressions;
using VisualPlayer.VisualizationBase.Plugins;

namespace VisualPlayer.Visualizations.Loading;

public sealed record VisualizationPackageManifest
{
    public required int ManifestVersion { get; init; }
    public required string Id { get; init; }
    public required string Version { get; init; }
    public required int ApiMajor { get; init; }
    public required int MinimumApiMinor { get; init; }
    public required string TargetFramework { get; init; }
    public required string AvaloniaVersion { get; init; }
    public required string EntryAssembly { get; init; }
    public required string EntryType { get; init; }
    public required string DefaultLanguage { get; init; }
    public required string LanguageDirectory { get; init; }

    public const string SupportedFramework = "net10.0";
    public const string SupportedAvaloniaVersion = "12.1.2";
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        UnmappedMemberHandling = System.Text.Json.Serialization.JsonUnmappedMemberHandling.Disallow
    };

    public static VisualizationPackageManifest Read(string directory)
    {
        var manifestPath = PackagePath.Resolve(directory, "manifest.json");
        if (new FileInfo(manifestPath).Length > 1024 * 1024) throw new JsonException("Manifest exceeds 1 MiB.");
        using var document = JsonDocument.Parse(File.ReadAllText(manifestPath));
        if (document.RootElement.ValueKind != JsonValueKind.Object
            || document.RootElement.EnumerateObject().Select(p => p.Name).Distinct(StringComparer.Ordinal).Count() != document.RootElement.EnumerateObject().Count())
            throw new JsonException("Expected a manifest object without duplicate properties.");
        var manifest = document.RootElement.Deserialize<VisualizationPackageManifest>(Options)
            ?? throw new JsonException("Missing manifest.");
        if (manifest.ManifestVersion != 1) throw new NotSupportedException("Unsupported manifest version.");
        if (manifest.Id is null || !Regex.IsMatch(manifest.Id, "^[a-z0-9][a-z0-9.-]*$") || manifest.Id == "visualplayer.none")
            throw new JsonException("Invalid or reserved plugin ID.");
        if (!System.Version.TryParse(manifest.Version, out var version) || version.Build < 0)
            throw new JsonException("A numeric plugin version with at least three components is required.");
        if (manifest.ApiMajor != VisualizationApi.Major || manifest.MinimumApiMinor < 0 || manifest.MinimumApiMinor > VisualizationApi.Minor)
            throw new NotSupportedException("Incompatible visualization API.");
        if (manifest.TargetFramework != SupportedFramework || manifest.AvaloniaVersion != SupportedAvaloniaVersion)
            throw new NotSupportedException("Unsupported .NET/Avalonia combination.");
        if (manifest.DefaultLanguage != "en-US") throw new NotSupportedException("English defaults must be embedded in the plugin.");
        if (string.IsNullOrWhiteSpace(manifest.EntryType)) throw new JsonException("Missing entry type.");
        if (string.IsNullOrWhiteSpace(manifest.EntryAssembly) || manifest.EntryAssembly.IndexOfAny(['/', '\\', ':']) >= 0
            || !manifest.EntryAssembly.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)) throw new JsonException("Invalid entry assembly.");
        var assembly = PackagePath.Resolve(directory, manifest.EntryAssembly);
        if (!File.Exists(assembly)) throw new FileNotFoundException("Missing entry assembly.", assembly);
        var dependencies = PackagePath.Resolve(directory, Path.ChangeExtension(manifest.EntryAssembly, ".deps.json"));
        if (!File.Exists(dependencies)) throw new FileNotFoundException("Missing dependency manifest.", dependencies);
        PackagePath.Resolve(directory, manifest.LanguageDirectory);
        return manifest;
    }
}

internal static class PackagePath
{
    public static string Resolve(string directory, string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath) || Path.IsPathRooted(relativePath) || relativePath.Contains(':'))
            throw new IOException("Expected a relative package path.");
        var segments = relativePath.Replace('\\', '/').Split('/');
        if (segments.Any(segment => segment is "" or "." or "..")) throw new IOException("Invalid package path segment.");
        var root = Path.GetFullPath(directory);
        CheckLink(root);
        var current = root;
        foreach (var segment in segments)
        {
            current = Path.Combine(current, segment);
            CheckLink(current);
        }
        return current;
    }
    private static void CheckLink(string path)
    {
        if ((File.Exists(path) || Directory.Exists(path)) && (File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0)
            throw new IOException("Links are not supported inside visualization packages.");
    }
}
