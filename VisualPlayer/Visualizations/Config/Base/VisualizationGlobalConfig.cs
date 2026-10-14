using VisualPlayer.Attributes;
using VisualPlayer.Enums;
using VisualPlayer.ViewModels;

namespace VisualPlayer.Visualizations.Config.Base;

public class VisualizationGlobalConfig : ViewModelBase
{
    #region CONST

    public const int ATTACK_TIME_DEFAULT = 20;
    public const int ATTACK_TIME_INCREMENT = 5;
    public const int ATTACK_TIME_MAXIMUM = 1000;
    public const int ATTACK_TIME_MINIMUM = 0;
    public const int FRAME_RATE_DEFAULT = 60;
    public const int FRAME_RATE_INCREMENT = 15;
    public const int FRAME_RATE_MAXIMUM = 240;
    public const int FRAME_RATE_MINIMUM = 15;
    public const int MAXIMUM_DECIBELS_DEFAULT = -10;
    public const int MAXIMUM_DECIBELS_MAXIMUM = 0;
    public const int MAXIMUM_DECIBELS_MINIMUM = -60;
    public const int MAXIMUM_FREQUENCY_DEFAULT = 12000;
    public const int MAXIMUM_FREQUENCY_INCREMENT = 100;
    public const int MAXIMUM_FREQUENCY_MAXIMUM = 24000;
    public const int MAXIMUM_FREQUENCY_MINIMUM = 100;
    public const int MINIMUM_DECIBELS_DEFAULT = -90;
    public const int MINIMUM_DECIBELS_MAXIMUM = -1;
    public const int MINIMUM_DECIBELS_MINIMUM = -120;
    public const int MINIMUM_FREQUENCY_DEFAULT = 50;
    public const int MINIMUM_FREQUENCY_INCREMENT = 10;
    public const int MINIMUM_FREQUENCY_MAXIMUM = 20000;
    public const int MINIMUM_FREQUENCY_MINIMUM = 0;
    public const int RELEASE_TIME_DEFAULT = 180;
    public const int RELEASE_TIME_INCREMENT = 10;
    public const int RELEASE_TIME_MAXIMUM = 3000;
    public const int RELEASE_TIME_MINIMUM = 0;

    #endregion CONST

    private int _attackTime = ATTACK_TIME_DEFAULT;
    private ChannelMode _channelMode = ChannelMode.Mixed;
    private FftSize _fftSize = FftSize.Size2048;
    private FftWindowType _fftWindow = FftWindowType.Hann;
    private int _frameRateLimit = FRAME_RATE_DEFAULT;
    private bool _fillScreen;
    private int _maximumDecibels = MAXIMUM_DECIBELS_DEFAULT;
    private int _maximumFrequency = MAXIMUM_FREQUENCY_DEFAULT;
    private int _minimumDecibels = MINIMUM_DECIBELS_DEFAULT;
    private int _minimumFrequency = MINIMUM_FREQUENCY_DEFAULT;
    private int _releaseTime = RELEASE_TIME_DEFAULT;
    private ScalingStrategy _scalingStrategy = ScalingStrategy.Logarithmic;


    #region PROPERTIES

    [NotifyPropertyUpdate]
    public int AttackTime
    {
        get => _attackTime;
        set => UpdateProperty(ref _attackTime, System.Math.Clamp(value, ATTACK_TIME_MINIMUM, ATTACK_TIME_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public ChannelMode ChannelMode
    {
        get => _channelMode;
        set => UpdateProperty(ref _channelMode, value);
    }

    [NotifyPropertyUpdate]
    public FftSize FftSize
    {
        get => _fftSize;
        set => UpdateProperty(ref _fftSize, value);
    }

    [NotifyPropertyUpdate]
    public FftWindowType FftWindow
    {
        get => _fftWindow;
        set => UpdateProperty(ref _fftWindow, value);
    }

    [NotifyPropertyUpdate]
    public int FrameRateLimit
    {
        get => _frameRateLimit;
        set => UpdateProperty(ref _frameRateLimit, System.Math.Clamp(value, FRAME_RATE_MINIMUM, FRAME_RATE_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public bool FillScreen
    {
        get => _fillScreen;
        set => UpdateProperty(ref _fillScreen, value);
    }

    [NotifyPropertyUpdate]
    public int MaximumDecibels
    {
        get => _maximumDecibels;
        set => UpdateProperty(ref _maximumDecibels, System.Math.Clamp(value, MAXIMUM_DECIBELS_MINIMUM, MAXIMUM_DECIBELS_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public int MaximumFrequency
    {
        get => _maximumFrequency;
        set => UpdateProperty(ref _maximumFrequency, System.Math.Clamp(value, MAXIMUM_FREQUENCY_MINIMUM, MAXIMUM_FREQUENCY_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public int MinimumDecibels
    {
        get => _minimumDecibels;
        set => UpdateProperty(ref _minimumDecibels, System.Math.Clamp(value, MINIMUM_DECIBELS_MINIMUM, MINIMUM_DECIBELS_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public int MinimumFrequency
    {
        get => _minimumFrequency;
        set => UpdateProperty(ref _minimumFrequency, System.Math.Clamp(value, MINIMUM_FREQUENCY_MINIMUM, MINIMUM_FREQUENCY_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public int ReleaseTime
    {
        get => _releaseTime;
        set => UpdateProperty(ref _releaseTime, System.Math.Clamp(value, RELEASE_TIME_MINIMUM, RELEASE_TIME_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public ScalingStrategy ScalingStrategy
    {
        get => _scalingStrategy;
        set => UpdateProperty(ref _scalingStrategy, value);
    }

    #endregion PROPERTIES
}
