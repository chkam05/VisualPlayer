using VisualPlayer.Attributes;
using VisualPlayer.Enums;
using VisualPlayer.Interfaces;
using VisualPlayer.ViewModels;

using VisualPlayer.Visualizations.Config.Base;

namespace VisualPlayer.Visualizations.Config;

public class OscilloscopeFilledVisualizationConfig : ViewModelBase, IVisualizationConfig
{
    #region CONST

    public const double OPACITY_MAXIMUM = 1.0;
    public const double OPACITY_MINIMUM = 0.1;
    public const double OPACITY_SMALL_CHANGE = 0.01;

    #endregion CONST

    private BeamEffect _beamEffect = BeamEffect.Beam;
    private int _lineThickness = VisualizationBaseConfig.DEFAULT_LINE_THICKNESS;
    private double _opacity = 0.5;
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
    public double Opacity
    {
        get => _opacity;
        set => UpdateProperty(
            ref _opacity,
            System.Math.Clamp(value, OPACITY_MINIMUM, OPACITY_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public bool SmoothLine
    {
        get => _smoothLine;
        set => UpdateProperty(ref _smoothLine, value);
    }

    #endregion PROPERTIES
}
