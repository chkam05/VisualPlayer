using VisualPlayer.Attributes;
using VisualPlayer.Enums;
using VisualPlayer.Interfaces;
using VisualPlayer.ViewModels;

using VisualPlayer.Visualizations.Config.Base;

namespace VisualPlayer.Visualizations.OscilloscopeCenter.Config;

public class OscilloscopeCenterVisualizationConfig : ViewModelBase, IVisualizationConfig
{
    private BeamEffect _beamEffect = BeamEffect.Beam;
    private int _lineThickness = VisualizationBaseConfig.DEFAULT_LINE_THICKNESS;
    private bool _smoothLine = true;

    #region PROPERTIES

    [NotifyPropertyUpdate]
    public BeamEffect BeamEffect
    {
        get => _beamEffect;
        set => UpdateProperty(ref _beamEffect, value);
    }

    [NotifyPropertyUpdate]
    public int LineThickness
    {
        get => _lineThickness;
        set => UpdateProperty(ref _lineThickness, System.Math.Clamp(value, VisualizationBaseConfig.LINE_THICKNESS_MINIMUM, VisualizationBaseConfig.LINE_THICKNESS_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public bool SmoothLine
    {
        get => _smoothLine;
        set => UpdateProperty(ref _smoothLine, value);
    }

    #endregion PROPERTIES
}