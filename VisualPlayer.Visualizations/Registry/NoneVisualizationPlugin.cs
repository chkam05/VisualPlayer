using System.ComponentModel;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using VisualPlayer.Interfaces;
using VisualPlayer.VisualizationBase.Plugins;
using VisualPlayer.Visualizations.Config.Base;
using VisualPlayer.Visualizations.Data;

namespace VisualPlayer.Visualizations.Registry;

internal sealed class NoneVisualizationPlugin : IVisualizationPlugin
{
    internal static IReadOnlyDictionary<string, string> English { get; } = new Dictionary<string, string> { ["ENUM_VISUALIZATIONTYPE_NONE"] = "None" };
    public VisualizationDescriptor Descriptor { get; } = new()
    {
        Id = VisualizationRegistry.NoneId, NameKey = "ENUM_VISUALIZATIONTYPE_NONE", PluginVersion = new Version(1, 0), ConfigVersion = 1
    };
    public IReadOnlyDictionary<string, string> EnglishTranslations => English;
    public IReadOnlyList<VisualizationPreset> DefaultPresets { get; } = new[]
    {
        new VisualizationPreset { Id = "default", NameKey = "ENUM_VISUALIZATIONTYPE_NONE", ConfigVersion = 1, Configuration = JsonSerializer.SerializeToElement<object?>(null) }
    };
    public IVisualizationConfig CreateDefaultConfiguration() => new EmptyConfiguration();
    public IVisualizationConfig ReadConfiguration(JsonElement configuration, int configVersion) => CreateDefaultConfiguration();
    public JsonElement WriteConfiguration(IVisualizationConfig configuration) => JsonSerializer.SerializeToElement<object?>(null);
    public IVisualization CreateVisualization(IVisualizationConfig configuration, VisualizationColorConfig color, IVisualizationHostServices services) => new EmptyVisualization();
    public Control? CreateEditor(VisualizationEditorContext context) => null;
    private sealed class EmptyConfiguration : IVisualizationConfig
    {
        public event PropertyChangedEventHandler? PropertyChanged { add { } remove { } }
    }
    private sealed class EmptyVisualization : IVisualization
    {
        public bool HasActiveAnimation => false;
        public int MaximumFrameRate => int.MaxValue;
        public void Draw(DrawingContext context, IBrush brush) { }
        public void PreCalculate(in VisualizationFrame frame) { }
        public void UpdateGraphics(Rect area, double renderScaling) { }
        public void Reset() { }
        public void Dispose() { }
    }
}
