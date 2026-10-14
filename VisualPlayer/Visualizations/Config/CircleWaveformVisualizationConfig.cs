using System.Text.Json.Serialization;
using VisualPlayer.Attributes;
using VisualPlayer.Enums;
using VisualPlayer.Interfaces;
using VisualPlayer.ViewModels;
using VisualPlayer.Visualizations.Config.Base;

namespace VisualPlayer.Visualizations.Config;

public class CircleWaveformVisualizationConfig : ViewModelBase, IVisualizationConfig
{
    #region CONST

    public const double OPACITY_MAXIMUM = 1.0;
    public const double OPACITY_MINIMUM = 0.1;
    public const double OPACITY_SMALL_CHANGE = 0.01;

    #endregion CONST

    private BeamEffect _beamEffect = BeamEffect.Beam;
    private int _centerOffsetX = VisualizationBaseConfig.DEFAULT_CENTER_OFFSET_X;
    private int _centerOffsetY = VisualizationBaseConfig.DEFAULT_CENTER_OFFSET_Y;
    private int _descentRate = VisualizationBaseConfig.DEFAULT_DESCENT_RATE;
    private bool _fillEnabled = true;
    private int _innerRadius = 32;
    private int _lineThickness = VisualizationBaseConfig.DEFAULT_LINE_THICKNESS;
    private bool _mirror = false;
    private double _opacity = 0.4;
    private int _peaksCount = VisualizationBaseConfig.DEFAULT_CIRCLE_BARS_COUNT;
    private int _rotation = 270;
    private bool _rotationAnimationEnabled = false;
    private RotationDirection _rotationDirection = RotationDirection.Clockwise;
    private int _rotationSpeed = VisualizationBaseConfig.DEFAULT_ROTATION_SPEED;

    #region PROPERTIES

    [NotifyPropertyUpdate]
    public BeamEffect BeamEffect
    {
        get => _beamEffect;
        set => UpdateProperty(ref _beamEffect, value);
    }

    [JsonIgnore]
    public bool CanSetRotation
    {
        get => !RotationAnimationEnabled;
    }

    [NotifyPropertyUpdate]
    public int CenterOffsetX
    {
        get => _centerOffsetX;
        set => UpdateProperty(ref _centerOffsetX, System.Math.Clamp(value, VisualizationBaseConfig.CENTER_OFFSET_MINIMUM, VisualizationBaseConfig.CENTER_OFFSET_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public int CenterOffsetY
    {
        get => _centerOffsetY;
        set => UpdateProperty(ref _centerOffsetY, System.Math.Clamp(value, VisualizationBaseConfig.CENTER_OFFSET_MINIMUM, VisualizationBaseConfig.CENTER_OFFSET_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public int DescentRate
    {
        get => _descentRate;
        set => UpdateProperty(ref _descentRate, System.Math.Clamp(value, VisualizationBaseConfig.DESCENT_RATE_MINIMUM, VisualizationBaseConfig.DESCENT_RATE_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public bool FillEnabled
    {
        get => _fillEnabled;
        set => UpdateProperty(ref _fillEnabled, value);
    }

    [NotifyPropertyUpdate]
    public int InnerRadius
    {
        get => _innerRadius;
        set => UpdateProperty(ref _innerRadius, System.Math.Clamp(value, VisualizationBaseConfig.INNER_RADIUS_MINIMUM, VisualizationBaseConfig.INNER_RADIUS_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public int LineThickness
    {
        get => _lineThickness;
        set => UpdateProperty(ref _lineThickness, System.Math.Clamp(value, VisualizationBaseConfig.LINE_THICKNESS_MINIMUM, VisualizationBaseConfig.LINE_THICKNESS_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public bool Mirror
    {
        get => _mirror;
        set => UpdateProperty(ref _mirror, value);
    }

    [NotifyPropertyUpdate]
    public double Opacity
    {
        get => _opacity;
        set => UpdateProperty(ref _opacity, System.Math.Clamp(value, OPACITY_MINIMUM, OPACITY_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public int PeaksCount
    {
        get => _peaksCount;
        set => UpdateProperty(ref _peaksCount, System.Math.Clamp(value, VisualizationBaseConfig.BARS_COUNT_MINIMUM, VisualizationBaseConfig.BARS_COUNT_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public int Rotation
    {
        get => _rotation;
        set => UpdateProperty(ref _rotation, System.Math.Clamp(value, VisualizationBaseConfig.ROTATION_MINIMUM, VisualizationBaseConfig.ROTATION_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public bool RotationAnimationEnabled
    {
        get => _rotationAnimationEnabled;
        set
        {
            if (_rotationAnimationEnabled == value)
                return;

            UpdateProperty(ref _rotationAnimationEnabled, value);
            NotifyPropertyChanged(nameof(CanSetRotation));
        }
    }

    [NotifyPropertyUpdate]
    public RotationDirection RotationDirection
    {
        get => _rotationDirection;
        set => UpdateProperty(ref _rotationDirection, value);
    }

    [NotifyPropertyUpdate]
    public int RotationSpeed
    {
        get => _rotationSpeed;
        set => UpdateProperty(ref _rotationSpeed, System.Math.Clamp(value, VisualizationBaseConfig.ROTATION_SPEED_MINIMUM, VisualizationBaseConfig.ROTATION_SPEED_MAXIMUM));
    }

    #endregion PROPERTIES
}
