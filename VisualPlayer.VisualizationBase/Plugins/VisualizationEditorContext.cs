using VisualPlayer.Interfaces;
using VisualPlayer.Visualizations.Config.Base;

namespace VisualPlayer.VisualizationBase.Plugins;

/// <summary>
/// Public editor data, independent of the runtime profile. Configuration and Color
/// are the live editable instances; opening an editor must not replace them.
/// An editor using subscriptions must detach them when removed from the visual tree.
/// </summary>
public sealed class VisualizationEditorContext
{
    public IVisualizationConfig Configuration { get; }
    public VisualizationColorConfig Color { get; }
    public IVisualizationHostServices Services { get; }
    public VisualizationCapabilities Capabilities { get; }
    public bool SupportsFloatingBarGravity => Capabilities.HasFlag(VisualizationCapabilities.FloatingBarGravity);

    public VisualizationEditorContext(IVisualizationConfig configuration,
        VisualizationColorConfig color, IVisualizationHostServices services,
        VisualizationCapabilities capabilities = VisualizationCapabilities.None)
    {
        Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        Color = color ?? throw new ArgumentNullException(nameof(color));
        Services = services ?? throw new ArgumentNullException(nameof(services));
        Capabilities = capabilities;
    }
}
