using System.Text.Json;
using System.Text.Json.Serialization;
using Avalonia.Controls;
using VisualPlayer.Enums;
using VisualPlayer.Interfaces;
using VisualPlayer.VisualizationBase.Plugins;
using VisualPlayer.Visualizations.Config.Base;
using VisualPlayer.Visualizations.Peaks.Config;
using VisualPlayer.Visualizations.Peaks.Resources;
using VisualPlayer.Visualizations.Peaks.Views;

namespace VisualPlayer.Visualizations.Peaks;

public sealed class PeaksPlugin : IVisualizationPlugin
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase, PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };
    public VisualizationDescriptor Descriptor { get; } = new()
    {
        Id = "visualplayer.peaks", NameKey = "ENUM_VISUALIZATIONTYPE_PEAKS",
        PluginVersion = new Version(1, 0, 0), ConfigVersion = 1,
        Capabilities = VisualizationCapabilities.Colors | VisualizationCapabilities.Spectrum | VisualizationCapabilities.FloatingBarGravity
    };
    public IReadOnlyDictionary<string, string> EnglishTranslations => Resources.EnglishTranslations.Values;
    public IReadOnlyList<VisualizationPreset> DefaultPresets => [new VisualizationPreset
    {
        Id = "default", NameKey = Descriptor.NameKey, ConfigVersion = 1,
        Configuration = WriteConfiguration(CreateDefaultConfiguration()),
        Color = new VisualizationColorConfig
        {
            Gradient = new GradientDefinition
            {
                Angle = 270, Type = GradientType.Linear,
                Points = [Point("#10893E", 0), Point("#10893E", .33), Point("#FFB900", .66), Point("#E81123", 1)]
            },
            Mode = VisualizationColorMode.Gradient, SolidColor = new("#10893E", string.Empty)
        }
    }];
    private static GradientPoint Point(string hex, double position) => new() { Color = new(hex, string.Empty), Position = position };
    public IVisualizationConfig CreateDefaultConfiguration() => new PeaksVisualizationConfig();
    public IVisualizationConfig ReadConfiguration(JsonElement configuration, int configVersion)
    {
        if (configVersion != 1) throw new NotSupportedException($"Unsupported Peaks configuration version: {configVersion}");
        return configuration.Deserialize<PeaksVisualizationConfig>(Options) ?? throw new JsonException("Missing Peaks configuration.");
    }
    public JsonElement WriteConfiguration(IVisualizationConfig configuration)
        => JsonSerializer.SerializeToElement((PeaksVisualizationConfig)configuration, Options);
    public IVisualization CreateVisualization(IVisualizationConfig configuration, VisualizationColorConfig color, IVisualizationHostServices services)
        => new PeaksVisualization((PeaksVisualizationConfig)configuration, color);
    public Control? CreateEditor(VisualizationEditorContext context) => new PeaksConfig { DataContext = context };
}
