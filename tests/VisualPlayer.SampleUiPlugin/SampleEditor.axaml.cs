using Avalonia.Controls;
using VisualPlayer.VisualizationBase.Plugins;

namespace VisualPlayer.SampleUiPlugin;

public partial class SampleEditor : UserControl
{
    public SampleEditor()
    {
        InitializeComponent();
    }
    public SampleEditor(VisualizationEditorContext context) : this() => DataContext = context;
}
