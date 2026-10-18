using System.Text.Json;
using Avalonia.Controls;
using VisualPlayer.Interfaces;
using VisualPlayer.VisualizationBase.Plugins;
using VisualPlayer.Visualizations.Config.Base;

namespace VisualPlayer.SamplePlugin;

// Loader fixture; the normal SamplePlugin remains the SDK-only contract example.
public sealed class ThrowingPlugin : IVisualizationPlugin
{
    public ThrowingPlugin() => throw new InvalidOperationException("Injected plugin constructor failure.");
    public VisualizationDescriptor Descriptor => throw new NotSupportedException();
    public IReadOnlyDictionary<string, string> EnglishTranslations => throw new NotSupportedException();
    public IReadOnlyList<VisualizationPreset> DefaultPresets => throw new NotSupportedException();
    public IVisualizationConfig CreateDefaultConfiguration() => throw new NotSupportedException();
    public IVisualizationConfig ReadConfiguration(JsonElement configuration, int configVersion) => throw new NotSupportedException();
    public JsonElement WriteConfiguration(IVisualizationConfig configuration) => throw new NotSupportedException();
    public IVisualization CreateVisualization(IVisualizationConfig configuration, VisualizationColorConfig color, IVisualizationHostServices services) => throw new NotSupportedException();
    public Control? CreateEditor(VisualizationEditorContext context) => throw new NotSupportedException();
}
