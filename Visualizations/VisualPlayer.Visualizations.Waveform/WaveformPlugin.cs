using System.Text.Json;
using System.Text.Json.Serialization;
using Avalonia.Controls;
using VisualPlayer.Enums;
using VisualPlayer.Interfaces;
using VisualPlayer.VisualizationBase.Plugins;
using VisualPlayer.Visualizations.Config.Base;
using VisualPlayer.Visualizations.Waveform.Config;
using VisualPlayer.Visualizations.Waveform.Resources;
using VisualPlayer.Visualizations.Waveform.Views;

namespace VisualPlayer.Visualizations.Waveform;

public sealed class WaveformPlugin : IVisualizationPlugin
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase, PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };
    public VisualizationDescriptor Descriptor { get; } = new()
    {
        Id = "visualplayer.waveform", NameKey = "ENUM_VISUALIZATIONTYPE_WAVEFORM",
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
                        Point("#E81123", 0),
                        Point("#FFB900", 0.166),
                        Point("#10893E", 0.332),
                        Point("#10893E", 0.668),
                        Point("#FFB900", 0.834),
                        Point("#E81123", 1),
                    ],
                    Type = GradientType.Linear
                },
                Mode = VisualizationColorMode.Rainbow,
                SolidColor = new("#10893E", string.Empty)
            }
    }];
    private static GradientPoint Point(string hex, double position) => new() { Color = new(hex, string.Empty), Position = position };
    public IVisualizationConfig CreateDefaultConfiguration() => new WaveformVisualizationConfig();
    public IVisualizationConfig ReadConfiguration(JsonElement configuration, int configVersion)
    {
        if (configVersion != 1) throw new NotSupportedException($"Unsupported Waveform configuration version: {configVersion}");
        return configuration.Deserialize<WaveformVisualizationConfig>(Options) ?? throw new JsonException("Missing Waveform configuration.");
    }
    public JsonElement WriteConfiguration(IVisualizationConfig configuration)
        => JsonSerializer.SerializeToElement((WaveformVisualizationConfig)configuration, Options);
    public IVisualization CreateVisualization(IVisualizationConfig configuration, VisualizationColorConfig color, IVisualizationHostServices services)
        => new WaveformVisualization((WaveformVisualizationConfig)configuration, color);
    public Control? CreateEditor(VisualizationEditorContext context) => new WaveformConfig { DataContext = context };
}
