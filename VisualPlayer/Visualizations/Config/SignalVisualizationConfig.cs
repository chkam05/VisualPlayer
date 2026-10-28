using System.Text.Json.Serialization;
using VisualPlayer.Attributes;
using VisualPlayer.Enums;
using VisualPlayer.Interfaces;
using VisualPlayer.ViewModels;
using VisualPlayer.Visualizations.Config.Base;

namespace VisualPlayer.Visualizations.Config;

public class SignalVisualizationConfig : ViewModelBase, IVisualizationConfig
{
    #region CONST

    public const int VIBRATION_RANGE_MAXIMUM = 64;
    public const int VIBRATION_RANGE_MINIMUM = 1;

    #endregion CONST

    private BeamEffect _beamEffect = BeamEffect.Beam;
    private int _descentRate = VisualizationBaseConfig.DEFAULT_DESCENT_RATE;
    private VisualizationDirection _direction = VisualizationDirection.BottomUp;
    private int _lineThickness = 2;
    private bool _reverseVibration = true;
    private int _stripesCount = 16;
    private bool _vibrationEnabled;
    private int _vibrationRange = 6;
    

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
        set => UpdateProperty(ref _descentRate, System.Math.Clamp(
            value,
            VisualizationBaseConfig.DESCENT_RATE_MINIMUM,
            VisualizationBaseConfig.DESCENT_RATE_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public VisualizationDirection Direction
    {
        get => _direction;
        set => UpdateProperty(ref _direction, value);
    }

    [JsonIgnore]
    public bool IsVibrationEnabled => VibrationEnabled;

    [NotifyPropertyUpdate]
    public int LineThickness
    {
        get => _lineThickness;
        set => UpdateProperty(ref _lineThickness, System.Math.Clamp(
            value,
            VisualizationBaseConfig.LINE_THICKNESS_MINIMUM,
            VisualizationBaseConfig.LINE_THICKNESS_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public bool ReverseVibration
    {
        get => _reverseVibration;
        set => UpdateProperty(ref _reverseVibration, value);
    }

    [NotifyPropertyUpdate]
    public int StripesCount
    {
        get => _stripesCount;
        set => UpdateProperty(ref _stripesCount, System.Math.Clamp(
            value,
            VisualizationBaseConfig.BARS_COUNT_MINIMUM,
            VisualizationBaseConfig.BARS_COUNT_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public bool VibrationEnabled
    {
        get => _vibrationEnabled;
        set
        {
            UpdateProperty(ref _vibrationEnabled, value);
            NotifyPropertyChanged(nameof(IsVibrationEnabled));
        }
    }

    [NotifyPropertyUpdate]
    public int VibrationRange
    {
        get => _vibrationRange;
        set => UpdateProperty(ref _vibrationRange, System.Math.Clamp(
            value,
            VIBRATION_RANGE_MINIMUM,
            VIBRATION_RANGE_MAXIMUM));
    }
}
