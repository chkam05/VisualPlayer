using System.Text.Json;
using System.Text.Json.Serialization;
using Avalonia.Controls;
using VisualPlayer.Enums;
using VisualPlayer.Interfaces;
using VisualPlayer.VisualizationBase.Plugins;
using VisualPlayer.Visualizations.Config.Base;
using VisualPlayer.Visualizations.PeaksFloating.Config;
using VisualPlayer.Visualizations.PeaksFloating.Resources;
using VisualPlayer.Visualizations.PeaksFloating.Views;

namespace VisualPlayer.Visualizations.PeaksFloating;

public sealed class PeaksFloatingPlugin : IVisualizationPlugin
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase, PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };
    public VisualizationDescriptor Descriptor { get; } = new()
    {
        Id = "visualplayer.peaks-floating", NameKey = "ENUM_VISUALIZATIONTYPE_PEAKSFLOATING",
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
                    Angle = 270,
                    Points =
                    [
                        Point("#10893E", 0),
                        Point("#10893E", 0.33),
                        Point("#FFB900", 0.66),
                        Point("#E81123", 1),
                    ],
                    Type = GradientType.Linear
                },
                Mode = VisualizationColorMode.Gradient,
                SolidColor = new("#10893E", string.Empty)
            }
    }];
    private static GradientPoint Point(string hex, double position) => new() { Color = new(hex, string.Empty), Position = position };
    public IVisualizationConfig CreateDefaultConfiguration() => new PeaksFloatingVisualizationConfig();
    public IVisualizationConfig ReadConfiguration(JsonElement configuration, int configVersion)
    {
        if (configVersion != 1) throw new NotSupportedException($"Unsupported PeaksFloating configuration version: {configVersion}");
        return configuration.Deserialize<PeaksFloatingVisualizationConfig>(Options) ?? throw new JsonException("Missing PeaksFloating configuration.");
    }
    public JsonElement WriteConfiguration(IVisualizationConfig configuration)
        => JsonSerializer.SerializeToElement((PeaksFloatingVisualizationConfig)configuration, Options);
    public IVisualization CreateVisualization(IVisualizationConfig configuration, VisualizationColorConfig color, IVisualizationHostServices services)
        => new PeaksFloatingVisualization((PeaksFloatingVisualizationConfig)configuration, color);
    public Control? CreateEditor(VisualizationEditorContext context) => new PeaksFloatingConfig { DataContext = context };
}
