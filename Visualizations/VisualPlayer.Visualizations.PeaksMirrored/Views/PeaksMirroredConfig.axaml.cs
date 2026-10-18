using System;
using System.Linq;
using Avalonia;
using VisualPlayer.UI.Localization;
using VisualPlayer.VisualizationBase.Plugins;
using System.Collections.Generic;
using Avalonia.Controls;
using VisualPlayer.Enums;

namespace VisualPlayer.Visualizations.PeaksMirrored.Views;

public partial class PeaksMirroredConfig : UserControl
{
    public static readonly DirectProperty<PeaksMirroredConfig, IReadOnlyList<LocalizedEnumValue<MirroredVisualizationDirection>>> DirectionsProperty
        = AvaloniaProperty.RegisterDirect<PeaksMirroredConfig, IReadOnlyList<LocalizedEnumValue<MirroredVisualizationDirection>>>(nameof(Directions), view => view.Directions);
    public IReadOnlyList<LocalizedEnumValue<MirroredVisualizationDirection>> Directions
        => Enum.GetValues<MirroredVisualizationDirection>().Select(value => new LocalizedEnumValue<MirroredVisualizationDirection>(value,
            (DataContext as VisualizationEditorContext)?.Services.CommonTranslations ?? UiCatalogs.English)).ToArray();


    public PeaksMirroredConfig()
    {
        InitializeComponent();
    }
    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        RaisePropertyChanged(DirectionsProperty, null!, Directions);
    }
}
