using System;
using System.Linq;
using Avalonia;
using VisualPlayer.UI.Localization;
using VisualPlayer.VisualizationBase.Plugins;
using System.Collections.Generic;
using Avalonia.Controls;
using VisualPlayer.Enums;

namespace VisualPlayer.Visualizations.StripesMirrored.Views;

public partial class StripesMirroredConfig : UserControl
{
    public static readonly DirectProperty<StripesMirroredConfig, IReadOnlyList<LocalizedEnumValue<MirroredVisualizationDirection>>> DirectionsProperty
        = AvaloniaProperty.RegisterDirect<StripesMirroredConfig, IReadOnlyList<LocalizedEnumValue<MirroredVisualizationDirection>>>(nameof(Directions), view => view.Directions);
    public IReadOnlyList<LocalizedEnumValue<MirroredVisualizationDirection>> Directions
        => Enum.GetValues<MirroredVisualizationDirection>().Select(value => new LocalizedEnumValue<MirroredVisualizationDirection>(value,
            (DataContext as VisualizationEditorContext)?.Services.CommonTranslations ?? UiCatalogs.English)).ToArray();


    public StripesMirroredConfig()
    {
        InitializeComponent();
    }
    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        RaisePropertyChanged(DirectionsProperty, null!, Directions);
    }
}
