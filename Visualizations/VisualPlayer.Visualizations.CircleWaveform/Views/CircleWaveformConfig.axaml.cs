using System;
using System.Linq;
using Avalonia;
using VisualPlayer.UI.Localization;
using VisualPlayer.VisualizationBase.Plugins;
using System.Collections.Generic;
using Avalonia.Controls;
using VisualPlayer.Enums;

namespace VisualPlayer.Visualizations.CircleWaveform.Views;

public partial class CircleWaveformConfig : UserControl
{
    public static readonly DirectProperty<CircleWaveformConfig, IReadOnlyList<LocalizedEnumValue<BeamEffect>>> BeamEffectsProperty
        = AvaloniaProperty.RegisterDirect<CircleWaveformConfig, IReadOnlyList<LocalizedEnumValue<BeamEffect>>>(nameof(BeamEffects), view => view.BeamEffects);
    public IReadOnlyList<LocalizedEnumValue<BeamEffect>> BeamEffects
        => Enum.GetValues<BeamEffect>().Select(value => new LocalizedEnumValue<BeamEffect>(value,
            (DataContext as VisualizationEditorContext)?.Services.CommonTranslations ?? UiCatalogs.English)).ToArray();

    public static readonly DirectProperty<CircleWaveformConfig, IReadOnlyList<LocalizedEnumValue<RotationDirection>>> RotationDirectionsProperty
        = AvaloniaProperty.RegisterDirect<CircleWaveformConfig, IReadOnlyList<LocalizedEnumValue<RotationDirection>>>(nameof(RotationDirections), view => view.RotationDirections);
    public IReadOnlyList<LocalizedEnumValue<RotationDirection>> RotationDirections
        => Enum.GetValues<RotationDirection>().Select(value => new LocalizedEnumValue<RotationDirection>(value,
            (DataContext as VisualizationEditorContext)?.Services.CommonTranslations ?? UiCatalogs.English)).ToArray();

    public CircleWaveformConfig()
    {
        InitializeComponent();
    }
    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        RaisePropertyChanged(BeamEffectsProperty, null!, BeamEffects);
        RaisePropertyChanged(RotationDirectionsProperty, null!, RotationDirections);
    }
}
