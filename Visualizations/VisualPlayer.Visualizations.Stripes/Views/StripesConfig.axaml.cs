using System;
using System.Linq;
using Avalonia;
using VisualPlayer.UI.Localization;
using VisualPlayer.VisualizationBase.Plugins;
using System.Collections.Generic;
using Avalonia.Controls;
using VisualPlayer.Enums;

namespace VisualPlayer.Visualizations.Stripes.Views;

public partial class StripesConfig : UserControl
{
    public static readonly DirectProperty<StripesConfig, IReadOnlyList<LocalizedEnumValue<VisualizationDirection>>> DirectionsProperty
        = AvaloniaProperty.RegisterDirect<StripesConfig, IReadOnlyList<LocalizedEnumValue<VisualizationDirection>>>(nameof(Directions), view => view.Directions);
    public IReadOnlyList<LocalizedEnumValue<VisualizationDirection>> Directions
        => Enum.GetValues<VisualizationDirection>().Select(value => new LocalizedEnumValue<VisualizationDirection>(value,
            (DataContext as VisualizationEditorContext)?.Services.CommonTranslations ?? UiCatalogs.English)).ToArray();


    public StripesConfig()
    {
        InitializeComponent();
    }
    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        RaisePropertyChanged(DirectionsProperty, null!, Directions);
    }
}
