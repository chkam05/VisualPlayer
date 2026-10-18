namespace VisualPlayer.VisualizationBase.Plugins;

/// <summary>Version of the public plugin contract, independent of package/config versions.</summary>
public static class VisualizationApi
{
    public const int Major = 1;
    public const int Minor = 0;
}

[Flags]
public enum VisualizationCapabilities
{
    None = 0,
    Colors = 1,
    Spectrum = 2,
    Waveform = 4,
    FloatingBarGravity = 8
}

/// <summary>
/// Metadata validated by the host before registration. Id is persistent and must
/// never depend on the display name, language or CLR type name.
/// </summary>
public sealed record VisualizationDescriptor
{
    public required string Id { get; init; }
    public required string NameKey { get; init; }
    public string? DescriptionKey { get; init; }
    public required Version PluginVersion { get; init; }
    public int ApiMajor { get; init; } = VisualizationApi.Major;
    public int MinimumApiMinor { get; init; } = VisualizationApi.Minor;
    public required int ConfigVersion { get; init; }
    public VisualizationCapabilities Capabilities { get; init; }
}
