using System.Text.Json.Serialization;
using VisualPlayer.Attributes;
using VisualPlayer.Enums;
using VisualPlayer.Interfaces;
using VisualPlayer.ViewModels;
using VisualPlayer.Visualizations.Config.Base;

namespace VisualPlayer.Visualizations.Config;

public class CircleStripesVisualizationConfig : ViewModelBase, IVisualizationConfig
{
    private BeamEffect _beamEffect = BeamEffect.Beam;
    private int _centerOffsetX = VisualizationBaseConfig.DEFAULT_CENTER_OFFSET_X;
    private int _centerOffsetY = VisualizationBaseConfig.DEFAULT_CENTER_OFFSET_Y;
    private int _descentRate = VisualizationBaseConfig.DEFAULT_DESCENT_RATE;
    private RadialVisualizationDirection _direction = RadialVisualizationDirection.InsideOut;
    private int _innerRadius = 64;
    private int _lineThickness = VisualizationBaseConfig.DEFAULT_LINE_THICKNESS;
    private bool _mirror = true;
    private int _rotation = 270;
    private bool _rotationAnimationEnabled = false;
    private RotationDirection _rotationDirection = RotationDirection.Clockwise;
    private int _rotationSpeed = VisualizationBaseConfig.DEFAULT_ROTATION_SPEED;
    private int _stripesCount = VisualizationBaseConfig.DEFAULT_CIRCLE_BARS_COUNT;

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
    public RadialVisualizationDirection Direction
    {
        get => _direction;
        set => UpdateProperty(ref _direction, value);
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

    [NotifyPropertyUpdate]
    public int StripesCount
    {
        get => _stripesCount;
        set => UpdateProperty(ref _stripesCount, System.Math.Clamp(value, VisualizationBaseConfig.BARS_COUNT_MINIMUM, VisualizationBaseConfig.BARS_COUNT_MAXIMUM));
    }

    #endregion PROPERTIES
}
