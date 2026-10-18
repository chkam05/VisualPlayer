using System;
using System.Linq;
using Avalonia;
using VisualPlayer.UI.Localization;
using VisualPlayer.VisualizationBase.Plugins;
using System.Collections.Generic;
using Avalonia.Controls;
using VisualPlayer.Enums;

namespace VisualPlayer.Visualizations.PeaksFloating.Views;

public partial class PeaksFloatingConfig : UserControl
{
    public static readonly DirectProperty<PeaksFloatingConfig, IReadOnlyList<LocalizedEnumValue<VisualizationDirection>>> DirectionsProperty
        = AvaloniaProperty.RegisterDirect<PeaksFloatingConfig, IReadOnlyList<LocalizedEnumValue<VisualizationDirection>>>(nameof(Directions), view => view.Directions);
    public IReadOnlyList<LocalizedEnumValue<VisualizationDirection>> Directions
        => Enum.GetValues<VisualizationDirection>().Select(value => new LocalizedEnumValue<VisualizationDirection>(value,
            (DataContext as VisualizationEditorContext)?.Services.CommonTranslations ?? UiCatalogs.English)).ToArray();


    public PeaksFloatingConfig()
    {
        InitializeComponent();
    }
    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        RaisePropertyChanged(DirectionsProperty, null!, Directions);
    }
}
