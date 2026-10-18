using System.Text.Json;
using System.Text.Json.Serialization;
using Avalonia.Controls;
using VisualPlayer.Enums;
using VisualPlayer.Interfaces;
using VisualPlayer.VisualizationBase.Plugins;
using VisualPlayer.Visualizations.Config.Base;
using VisualPlayer.Visualizations.CircleWaveform.Config;
using VisualPlayer.Visualizations.CircleWaveform.Resources;
using VisualPlayer.Visualizations.CircleWaveform.Views;

namespace VisualPlayer.Visualizations.CircleWaveform;

public sealed class CircleWaveformPlugin : IVisualizationPlugin
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase, PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };
    public VisualizationDescriptor Descriptor { get; } = new()
    {
        Id = "visualplayer.circle-waveform", NameKey = "ENUM_VISUALIZATIONTYPE_CIRCLEWAVEFORM",
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
    public IVisualizationConfig CreateDefaultConfiguration() => new CircleWaveformVisualizationConfig();
    public IVisualizationConfig ReadConfiguration(JsonElement configuration, int configVersion)
    {
        if (configVersion != 1) throw new NotSupportedException($"Unsupported CircleWaveform configuration version: {configVersion}");
        return configuration.Deserialize<CircleWaveformVisualizationConfig>(Options) ?? throw new JsonException("Missing CircleWaveform configuration.");
    }
    public JsonElement WriteConfiguration(IVisualizationConfig configuration)
        => JsonSerializer.SerializeToElement((CircleWaveformVisualizationConfig)configuration, Options);
    public IVisualization CreateVisualization(IVisualizationConfig configuration, VisualizationColorConfig color, IVisualizationHostServices services)
        => new CircleWaveformVisualization((CircleWaveformVisualizationConfig)configuration, color);
    public Control? CreateEditor(VisualizationEditorContext context) => new CircleWaveformConfig { DataContext = context };
}
