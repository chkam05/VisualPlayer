using System.Text.Json;
using Avalonia.Controls;
using VisualPlayer.Interfaces;
using VisualPlayer.Visualizations.Config.Base;

namespace VisualPlayer.VisualizationBase.Plugins;

/// <summary>
/// One plugin implements one visualization. Factories, editors and rendering run
/// on the UI thread. Discovery must not create an editor or start rendering.
/// The host disposes visualization instances on replacement/shutdown. The plugin
/// itself owns no active rendering state shared between instances.
/// </summary>
public interface IVisualizationPlugin
{
    VisualizationDescriptor Descriptor { get; }
    IReadOnlyDictionary<string, string> EnglishTranslations { get; }
    IReadOnlyList<VisualizationPreset> DefaultPresets { get; }

    /// <summary>Always return a new independent, valid configuration instance.</summary>
    IVisualizationConfig CreateDefaultConfiguration();

    /// <summary>
    /// Deserialize/migrate the specified schema version into a new configuration.
    /// Throw on unsupported versions or invalid data; do not silently replace user
    /// settings with defaults. The host retains the original JSON on failure.
    /// </summary>
    IVisualizationConfig ReadConfiguration(JsonElement configuration, int configVersion);

    /// <summary>Return owned JSON for Descriptor.ConfigVersion; never mutate the input.</summary>
    JsonElement WriteConfiguration(IVisualizationConfig configuration);

    IVisualization CreateVisualization(IVisualizationConfig configuration,
        VisualizationColorConfig color, IVisualizationHostServices services);

    /// <summary>
    /// Return a fresh editor for the supplied configuration, or null if none is needed.
    /// Use local resources/avares URIs rather than modifying global application styles.
    /// </summary>
    Control? CreateEditor(VisualizationEditorContext context);
}
