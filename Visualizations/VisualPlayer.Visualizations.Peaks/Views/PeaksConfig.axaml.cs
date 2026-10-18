using System;
using System.Linq;
using Avalonia;
using VisualPlayer.UI.Localization;
using VisualPlayer.VisualizationBase.Plugins;
using System.Collections.Generic;
using Avalonia.Controls;
using VisualPlayer.Enums;

namespace VisualPlayer.Visualizations.Peaks.Views;

public partial class PeaksConfig : UserControl
{
    public static readonly DirectProperty<PeaksConfig, IReadOnlyList<LocalizedEnumValue<VisualizationDirection>>> DirectionsProperty
        = AvaloniaProperty.RegisterDirect<PeaksConfig, IReadOnlyList<LocalizedEnumValue<VisualizationDirection>>>(nameof(Directions), view => view.Directions);
    public IReadOnlyList<LocalizedEnumValue<VisualizationDirection>> Directions
        => Enum.GetValues<VisualizationDirection>().Select(value => new LocalizedEnumValue<VisualizationDirection>(value,
            (DataContext as VisualizationEditorContext)?.Services.CommonTranslations ?? UiCatalogs.English)).ToArray();


    public PeaksConfig()
    {
        InitializeComponent();
    }
    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        RaisePropertyChanged(DirectionsProperty, null!, Directions);
    }
}
