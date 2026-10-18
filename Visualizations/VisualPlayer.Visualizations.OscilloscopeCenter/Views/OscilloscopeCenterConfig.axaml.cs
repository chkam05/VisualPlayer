using System;
using System.Linq;
using Avalonia;
using VisualPlayer.UI.Localization;
using VisualPlayer.VisualizationBase.Plugins;
using System.Collections.Generic;
using Avalonia.Controls;
using VisualPlayer.Enums;

namespace VisualPlayer.Visualizations.OscilloscopeCenter.Views;

public partial class OscilloscopeCenterConfig : UserControl
{
    public static readonly DirectProperty<OscilloscopeCenterConfig, IReadOnlyList<LocalizedEnumValue<BeamEffect>>> BeamEffectsProperty
        = AvaloniaProperty.RegisterDirect<OscilloscopeCenterConfig, IReadOnlyList<LocalizedEnumValue<BeamEffect>>>(nameof(BeamEffects), view => view.BeamEffects);
    public IReadOnlyList<LocalizedEnumValue<BeamEffect>> BeamEffects
        => Enum.GetValues<BeamEffect>().Select(value => new LocalizedEnumValue<BeamEffect>(value,
            (DataContext as VisualizationEditorContext)?.Services.CommonTranslations ?? UiCatalogs.English)).ToArray();

    public OscilloscopeCenterConfig()
    {
        InitializeComponent();
    }
    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        RaisePropertyChanged(BeamEffectsProperty, null!, BeamEffects);
    }
}
