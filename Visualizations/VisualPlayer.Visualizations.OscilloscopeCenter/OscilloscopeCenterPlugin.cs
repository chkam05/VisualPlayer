using System.Text.Json;
using System.Text.Json.Serialization;
using Avalonia.Controls;
using VisualPlayer.Enums;
using VisualPlayer.Interfaces;
using VisualPlayer.VisualizationBase.Plugins;
using VisualPlayer.Visualizations.Config.Base;
using VisualPlayer.Visualizations.OscilloscopeCenter.Config;
using VisualPlayer.Visualizations.OscilloscopeCenter.Resources;
using VisualPlayer.Visualizations.OscilloscopeCenter.Views;

namespace VisualPlayer.Visualizations.OscilloscopeCenter;

public sealed class OscilloscopeCenterPlugin : IVisualizationPlugin
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase, PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };
    public VisualizationDescriptor Descriptor { get; } = new()
    {
        Id = "visualplayer.oscilloscope-center", NameKey = "ENUM_VISUALIZATIONTYPE_OSCILLOSCOPECENTER",
        PluginVersion = new Version(1, 0, 0), ConfigVersion = 1,
        Capabilities = VisualizationCapabilities.Colors | VisualizationCapabilities.Waveform
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
    public IVisualizationConfig CreateDefaultConfiguration() => new OscilloscopeCenterVisualizationConfig();
    public IVisualizationConfig ReadConfiguration(JsonElement configuration, int configVersion)
    {
        if (configVersion != 1) throw new NotSupportedException($"Unsupported OscilloscopeCenter configuration version: {configVersion}");
        return configuration.Deserialize<OscilloscopeCenterVisualizationConfig>(Options) ?? throw new JsonException("Missing OscilloscopeCenter configuration.");
    }
    public JsonElement WriteConfiguration(IVisualizationConfig configuration)
        => JsonSerializer.SerializeToElement((OscilloscopeCenterVisualizationConfig)configuration, Options);
    public IVisualization CreateVisualization(IVisualizationConfig configuration, VisualizationColorConfig color, IVisualizationHostServices services)
        => new OscilloscopeCenterVisualization((OscilloscopeCenterVisualizationConfig)configuration, color);
    public Control? CreateEditor(VisualizationEditorContext context) => new OscilloscopeCenterConfig { DataContext = context };
}
