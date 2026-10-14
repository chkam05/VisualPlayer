using System.Text.Json.Serialization;
using VisualPlayer.Attributes;
using VisualPlayer.Enums;
using VisualPlayer.Interfaces;
using VisualPlayer.ViewModels;

using VisualPlayer.Visualizations.Config.Base;

namespace VisualPlayer.Visualizations.Config;

public class WaveformVisualizationConfig : ViewModelBase, IVisualizationConfig
{
    #region CONST

    public const double OPACITY_MAXIMUM = 1.0;
    public const double OPACITY_MINIMUM = 0.1;
    public const double OPACITY_SMALL_CHANGE = 0.01;
    #endregion CONST

    private int _descentRate = VisualizationBaseConfig.DEFAULT_DESCENT_RATE;
    private VisualizationDirection _direction = VisualizationDirection.BottomUp;
    private BeamEffect _beamEffect = BeamEffect.None;
    private bool _fillEnabled = true;
    private int _lineThickness = VisualizationBaseConfig.DEFAULT_LINE_THICKNESS;
    private double _opacity = 1.0;
    private int _peaksCount = VisualizationBaseConfig.DEFAULT_BARS_COUNT;
    private WaveformTransformation _transformation = WaveformTransformation.None;
    private int _transformationMinimum = VisualizationBaseConfig.DEFAULT_TRANSFORMATION_MINIMUM;

    #region PROPERTIES

    [NotifyPropertyUpdate]
    public BeamEffect BeamEffect
    {
        get => _beamEffect;
        set => UpdateProperty(ref _beamEffect, value);
    }

    [NotifyPropertyUpdate]
    public int DescentRate
    {
        get => _descentRate;
        set => UpdateProperty(ref _descentRate, System.Math.Clamp(value, VisualizationBaseConfig.DESCENT_RATE_MINIMUM, VisualizationBaseConfig.DESCENT_RATE_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public VisualizationDirection Direction
    {
        get => _direction;
        set => UpdateProperty(ref _direction, value);
    }

    [NotifyPropertyUpdate]
    public bool FillEnabled
    {
        get => _fillEnabled;
        set => UpdateProperty(ref _fillEnabled, value);
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
        set => UpdateProperty(ref _opacity, System.Math.Clamp(value, OPACITY_MINIMUM, OPACITY_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public int PeaksCount
    {
        get => _peaksCount;
        set => UpdateProperty(ref _peaksCount, System.Math.Clamp(value, VisualizationBaseConfig.BARS_COUNT_MINIMUM, VisualizationBaseConfig.BARS_COUNT_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public WaveformTransformation Transformation
    {
        get => _transformation;
        set
        {
            if (_transformation == value)
                return;

            UpdateProperty(ref _transformation, value);
            NotifyPropertyChanged(nameof(IsTransformationEnabled));
        }
    }

    [NotifyPropertyUpdate]
    public int TransformationMinimum
    {
        get => _transformationMinimum;
        set => UpdateProperty(ref _transformationMinimum, System.Math.Clamp(value, VisualizationBaseConfig.TRANSFORMATION_MINIMUM_MINIMUM, VisualizationBaseConfig.TRANSFORMATION_MINIMUM_MAXIMUM));
    }

    [JsonIgnore]
    public bool IsTransformationEnabled
    {
        get => Transformation != WaveformTransformation.None;
    }

    #endregion PROPERTIES
}
