using System.Text.Json;
using VisualPlayer.Visualizations.Config.Base;

namespace VisualPlayer.VisualizationBase.Plugins;

/// <summary>
/// A default preset, not a runtime profile. The plugin/preset IDs identify it;
/// NameKey is localized. Configuration must own its JSON (use JsonElement.Clone).
/// The host copies mutable color settings before editing a preset instance.
/// </summary>
public sealed record VisualizationPreset
{
    public required string Id { get; init; }
    public required string NameKey { get; init; }
    public required int ConfigVersion { get; init; }
    public required JsonElement Configuration { get; init; }
    public VisualizationColorConfig? Color { get; init; }
    public bool FillScreen { get; init; }
}
