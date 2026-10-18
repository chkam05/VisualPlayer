using System;
using System.Linq;
using Avalonia;
using VisualPlayer.UI.Localization;
using VisualPlayer.VisualizationBase.Plugins;
using System.Collections.Generic;
using Avalonia.Controls;
using VisualPlayer.Enums;

namespace VisualPlayer.Visualizations.CircleStripes.Views;

public partial class CircleStripesConfig : UserControl
{
    public static readonly DirectProperty<CircleStripesConfig, IReadOnlyList<LocalizedEnumValue<BeamEffect>>> BeamEffectsProperty
        = AvaloniaProperty.RegisterDirect<CircleStripesConfig, IReadOnlyList<LocalizedEnumValue<BeamEffect>>>(nameof(BeamEffects), view => view.BeamEffects);
    public IReadOnlyList<LocalizedEnumValue<BeamEffect>> BeamEffects
        => Enum.GetValues<BeamEffect>().Select(value => new LocalizedEnumValue<BeamEffect>(value,
            (DataContext as VisualizationEditorContext)?.Services.CommonTranslations ?? UiCatalogs.English)).ToArray();

    public static readonly DirectProperty<CircleStripesConfig, IReadOnlyList<LocalizedEnumValue<RadialVisualizationDirection>>> DirectionsProperty
        = AvaloniaProperty.RegisterDirect<CircleStripesConfig, IReadOnlyList<LocalizedEnumValue<RadialVisualizationDirection>>>(nameof(Directions), view => view.Directions);
    public IReadOnlyList<LocalizedEnumValue<RadialVisualizationDirection>> Directions
        => Enum.GetValues<RadialVisualizationDirection>().Select(value => new LocalizedEnumValue<RadialVisualizationDirection>(value,
            (DataContext as VisualizationEditorContext)?.Services.CommonTranslations ?? UiCatalogs.English)).ToArray();

    public static readonly DirectProperty<CircleStripesConfig, IReadOnlyList<LocalizedEnumValue<RotationDirection>>> RotationDirectionsProperty
        = AvaloniaProperty.RegisterDirect<CircleStripesConfig, IReadOnlyList<LocalizedEnumValue<RotationDirection>>>(nameof(RotationDirections), view => view.RotationDirections);
    public IReadOnlyList<LocalizedEnumValue<RotationDirection>> RotationDirections
        => Enum.GetValues<RotationDirection>().Select(value => new LocalizedEnumValue<RotationDirection>(value,
            (DataContext as VisualizationEditorContext)?.Services.CommonTranslations ?? UiCatalogs.English)).ToArray();

    public CircleStripesConfig()
    {
        InitializeComponent();
    }
    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        RaisePropertyChanged(BeamEffectsProperty, null!, BeamEffects);
        RaisePropertyChanged(DirectionsProperty, null!, Directions);
        RaisePropertyChanged(RotationDirectionsProperty, null!, RotationDirections);
    }
}
