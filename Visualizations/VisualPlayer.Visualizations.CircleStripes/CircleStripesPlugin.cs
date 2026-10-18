using System.Text.Json;
using System.Text.Json.Serialization;
using Avalonia.Controls;
using VisualPlayer.Enums;
using VisualPlayer.Interfaces;
using VisualPlayer.VisualizationBase.Plugins;
using VisualPlayer.Visualizations.Config.Base;
using VisualPlayer.Visualizations.CircleStripes.Config;
using VisualPlayer.Visualizations.CircleStripes.Resources;
using VisualPlayer.Visualizations.CircleStripes.Views;

namespace VisualPlayer.Visualizations.CircleStripes;

public sealed class CircleStripesPlugin : IVisualizationPlugin
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase, PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };
    public VisualizationDescriptor Descriptor { get; } = new()
    {
        Id = "visualplayer.circle-stripes", NameKey = "ENUM_VISUALIZATIONTYPE_CIRCLESTRIPES",
        PluginVersion = new Version(1, 0, 0), ConfigVersion = 1,
        Capabilities = VisualizationCapabilities.Colors | VisualizationCapabilities.Spectrum
    };
    public IReadOnlyDictionary<string, string> EnglishTranslations => Resources.EnglishTranslations.Values;
    public IReadOnlyList<VisualizationPreset> DefaultPresets => [new VisualizationPreset
    {
        Id = "default", NameKey = "PRESET_DEFAULT", ConfigVersion = 1,
        Configuration = WriteConfiguration(CreateDefaultConfiguration()),
        Color = new VisualizationColorConfig
            {
                Gradient = new GradientDefinition()
                {
                    Angle = 0,
                    Points =
                    [
                        Point("#10893E", 0),
                        Point("#10893E", 0.5),
                        Point("#FFB900", 0.75),
                        Point("#E81123", 1),
                    ],
                    Type = GradientType.Radial
                },
                Mode = VisualizationColorMode.Gradient,
                SolidColor = new("#10893E", string.Empty)
            }
    }];
    private static GradientPoint Point(string hex, double position) => new() { Color = new(hex, string.Empty), Position = position };
    public IVisualizationConfig CreateDefaultConfiguration() => new CircleStripesVisualizationConfig();
    public IVisualizationConfig ReadConfiguration(JsonElement configuration, int configVersion)
    {
        if (configVersion != 1) throw new NotSupportedException($"Unsupported CircleStripes configuration version: {configVersion}");
        return configuration.Deserialize<CircleStripesVisualizationConfig>(Options) ?? throw new JsonException("Missing CircleStripes configuration.");
    }
    public JsonElement WriteConfiguration(IVisualizationConfig configuration)
        => JsonSerializer.SerializeToElement((CircleStripesVisualizationConfig)configuration, Options);
    public IVisualization CreateVisualization(IVisualizationConfig configuration, VisualizationColorConfig color, IVisualizationHostServices services)
        => new CircleStripesVisualization((CircleStripesVisualizationConfig)configuration, color);
    public Control? CreateEditor(VisualizationEditorContext context) => new CircleStripesConfig { DataContext = context };
}
