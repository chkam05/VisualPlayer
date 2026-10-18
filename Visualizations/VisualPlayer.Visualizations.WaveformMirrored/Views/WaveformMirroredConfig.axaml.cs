using System;
using System.Linq;
using Avalonia;
using VisualPlayer.UI.Localization;
using VisualPlayer.VisualizationBase.Plugins;
using System.Collections.Generic;
using Avalonia.Controls;
using VisualPlayer.Enums;

namespace VisualPlayer.Visualizations.WaveformMirrored.Views;

public partial class WaveformMirroredConfig : UserControl
{
    public static readonly DirectProperty<WaveformMirroredConfig, IReadOnlyList<LocalizedEnumValue<BeamEffect>>> BeamEffectsProperty
        = AvaloniaProperty.RegisterDirect<WaveformMirroredConfig, IReadOnlyList<LocalizedEnumValue<BeamEffect>>>(nameof(BeamEffects), view => view.BeamEffects);
    public IReadOnlyList<LocalizedEnumValue<BeamEffect>> BeamEffects
        => Enum.GetValues<BeamEffect>().Select(value => new LocalizedEnumValue<BeamEffect>(value,
            (DataContext as VisualizationEditorContext)?.Services.CommonTranslations ?? UiCatalogs.English)).ToArray();

    public static readonly DirectProperty<WaveformMirroredConfig, IReadOnlyList<LocalizedEnumValue<MirroredVisualizationDirection>>> DirectionsProperty
        = AvaloniaProperty.RegisterDirect<WaveformMirroredConfig, IReadOnlyList<LocalizedEnumValue<MirroredVisualizationDirection>>>(nameof(Directions), view => view.Directions);
    public IReadOnlyList<LocalizedEnumValue<MirroredVisualizationDirection>> Directions
        => Enum.GetValues<MirroredVisualizationDirection>().Select(value => new LocalizedEnumValue<MirroredVisualizationDirection>(value,
            (DataContext as VisualizationEditorContext)?.Services.CommonTranslations ?? UiCatalogs.English)).ToArray();

    public static readonly DirectProperty<WaveformMirroredConfig, IReadOnlyList<LocalizedEnumValue<WaveformTransformation>>> TransformationsProperty
        = AvaloniaProperty.RegisterDirect<WaveformMirroredConfig, IReadOnlyList<LocalizedEnumValue<WaveformTransformation>>>(nameof(Transformations), view => view.Transformations);
    public IReadOnlyList<LocalizedEnumValue<WaveformTransformation>> Transformations
        => Enum.GetValues<WaveformTransformation>().Select(value => new LocalizedEnumValue<WaveformTransformation>(value,
            (DataContext as VisualizationEditorContext)?.Services.CommonTranslations ?? UiCatalogs.English)).ToArray();

    public WaveformMirroredConfig()
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
