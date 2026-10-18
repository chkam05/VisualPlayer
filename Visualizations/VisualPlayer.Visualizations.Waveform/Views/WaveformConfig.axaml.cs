using System;
using System.Linq;
using Avalonia;
using VisualPlayer.UI.Localization;
using VisualPlayer.VisualizationBase.Plugins;
using System.Collections.Generic;
using Avalonia.Controls;
using VisualPlayer.Enums;

namespace VisualPlayer.Visualizations.Waveform.Views;

public partial class WaveformConfig : UserControl
{
    public static readonly DirectProperty<WaveformConfig, IReadOnlyList<LocalizedEnumValue<BeamEffect>>> BeamEffectsProperty
        = AvaloniaProperty.RegisterDirect<WaveformConfig, IReadOnlyList<LocalizedEnumValue<BeamEffect>>>(nameof(BeamEffects), view => view.BeamEffects);
    public IReadOnlyList<LocalizedEnumValue<BeamEffect>> BeamEffects
        => Enum.GetValues<BeamEffect>().Select(value => new LocalizedEnumValue<BeamEffect>(value,
            (DataContext as VisualizationEditorContext)?.Services.CommonTranslations ?? UiCatalogs.English)).ToArray();

    public static readonly DirectProperty<WaveformConfig, IReadOnlyList<LocalizedEnumValue<VisualizationDirection>>> DirectionsProperty
        = AvaloniaProperty.RegisterDirect<WaveformConfig, IReadOnlyList<LocalizedEnumValue<VisualizationDirection>>>(nameof(Directions), view => view.Directions);
    public IReadOnlyList<LocalizedEnumValue<VisualizationDirection>> Directions
        => Enum.GetValues<VisualizationDirection>().Select(value => new LocalizedEnumValue<VisualizationDirection>(value,
            (DataContext as VisualizationEditorContext)?.Services.CommonTranslations ?? UiCatalogs.English)).ToArray();

    public static readonly DirectProperty<WaveformConfig, IReadOnlyList<LocalizedEnumValue<WaveformTransformation>>> TransformationsProperty
        = AvaloniaProperty.RegisterDirect<WaveformConfig, IReadOnlyList<LocalizedEnumValue<WaveformTransformation>>>(nameof(Transformations), view => view.Transformations);
    public IReadOnlyList<LocalizedEnumValue<WaveformTransformation>> Transformations
        => Enum.GetValues<WaveformTransformation>().Select(value => new LocalizedEnumValue<WaveformTransformation>(value,
            (DataContext as VisualizationEditorContext)?.Services.CommonTranslations ?? UiCatalogs.English)).ToArray();

    public WaveformConfig()
    {
        InitializeComponent();
    }
    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        RaisePropertyChanged(BeamEffectsProperty, null!, BeamEffects);
        RaisePropertyChanged(DirectionsProperty, null!, Directions);
        RaisePropertyChanged(TransformationsProperty, null!, Transformations);
    }
}
