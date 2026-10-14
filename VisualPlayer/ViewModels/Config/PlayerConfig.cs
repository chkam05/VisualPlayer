using VisualPlayer.Attributes;
using VisualPlayer.Enums.Player;
using VisualPlayer.Utils;

namespace VisualPlayer.ViewModels.Config;

public class PlayerConfig : ViewModelBase
{
    #region CONST

    public const int CROSSFADE_DURATION_DEFAULT = 3;
    public const int CROSSFADE_DURATION_MAXIMUM = 5;
    public const int CROSSFADE_DURATION_MINIMUM = 1;
    public const int PLAYBACK_SPEED_DEFAULT = 100;
    public const int PLAYBACK_SPEED_MAXIMUM = 200;
    public const int PLAYBACK_SPEED_MINIMUM = 50;
    public const int PLAYBACK_SPEED_PRESET_1_DEFAULT = 75;
    public const int PLAYBACK_SPEED_PRESET_2_DEFAULT = 100;
    public const int PLAYBACK_SPEED_PRESET_3_DEFAULT = 125;
    public const int PLAYBACK_SPEED_PRESET_4_DEFAULT = 150;
    public const double VOLUME_DEFAULT = 0.75;
    public const double VOLUME_MAXIMUM = 1.0;
    public const double VOLUME_MINIMUM = 0.0;

    #endregion CONST

    private int _crossfadeDuration = CROSSFADE_DURATION_DEFAULT;
    private bool _crossfadeEnabled;
    private bool _isMuted;
    private bool _isShuffleEnabled;
    private bool _playAfterAddingFiles;
    private bool _playOnStartup;
    private int _playbackSpeed = PLAYBACK_SPEED_DEFAULT;
    private int _playbackSpeedPreset1 = PLAYBACK_SPEED_PRESET_1_DEFAULT;
    private int _playbackSpeedPreset2 = PLAYBACK_SPEED_PRESET_2_DEFAULT;
    private int _playbackSpeedPreset3 = PLAYBACK_SPEED_PRESET_3_DEFAULT;
    private int _playbackSpeedPreset4 = PLAYBACK_SPEED_PRESET_4_DEFAULT;
    private bool _rememberLastPlayList;
    private RepeatMode _repeatMode;
    private bool _showRemainingTime;
    private double _volume = VOLUME_DEFAULT;


    [NotifyPropertyUpdate]
    public int CrossfadeDuration
    {
        get => _crossfadeDuration;
        set => UpdateProperty(
            ref _crossfadeDuration,
            System.Math.Clamp(value, CROSSFADE_DURATION_MINIMUM, CROSSFADE_DURATION_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public bool CrossfadeEnabled
    {
        get => _crossfadeEnabled;
        set => UpdateProperty(ref _crossfadeEnabled, value);
    }

    [NotifyPropertyUpdate]
    public bool IsMuted
    {
        get => _isMuted;
        set => UpdateProperty(ref _isMuted, value);
    }

    [NotifyPropertyUpdate]
    public bool IsShuffleEnabled
    {
        get => _isShuffleEnabled;
        set => UpdateProperty(ref _isShuffleEnabled, value);
    }

    [NotifyPropertyUpdate]
    public bool PlayAfterAddingFiles
    {
        get => _playAfterAddingFiles;
        set => UpdateProperty(ref _playAfterAddingFiles, value);
    }

    [NotifyPropertyUpdate]
    public bool PlayOnStartup
    {
        get => _playOnStartup;
        set => UpdateProperty(ref _playOnStartup, value);
    }

    [NotifyPropertyUpdate]
    public int PlaybackSpeed
    {
        get => _playbackSpeed;
        set => UpdateProperty(ref _playbackSpeed, PlaybackSpeedUtils.NormalizePercent(value));
    }

    [NotifyPropertyUpdate]
    public int PlaybackSpeedPreset1
    {
        get => _playbackSpeedPreset1;
        set => UpdateProperty(ref _playbackSpeedPreset1, PlaybackSpeedUtils.NormalizePercent(value));
    }

    [NotifyPropertyUpdate]
    public int PlaybackSpeedPreset2
    {
        get => _playbackSpeedPreset2;
        set => UpdateProperty(ref _playbackSpeedPreset2, PlaybackSpeedUtils.NormalizePercent(value));
    }

    [NotifyPropertyUpdate]
    public int PlaybackSpeedPreset3
    {
        get => _playbackSpeedPreset3;
        set => UpdateProperty(ref _playbackSpeedPreset3, PlaybackSpeedUtils.NormalizePercent(value));
    }

    [NotifyPropertyUpdate]
    public int PlaybackSpeedPreset4
    {
        get => _playbackSpeedPreset4;
        set => UpdateProperty(ref _playbackSpeedPreset4, PlaybackSpeedUtils.NormalizePercent(value));
    }

    [NotifyPropertyUpdate]
    public bool RememberLastPlayList
    {
        get => _rememberLastPlayList;
        set => UpdateProperty(ref _rememberLastPlayList, value);
    }

    [NotifyPropertyUpdate]
    public RepeatMode RepeatMode
    {
        get => _repeatMode;
        set => UpdateProperty(ref _repeatMode, value);
    }

    [NotifyPropertyUpdate]
    public bool ShowRemainingTime
    {
        get => _showRemainingTime;
        set => UpdateProperty(ref _showRemainingTime, value);
    }

    [NotifyPropertyUpdate]
    public double Volume
    {
        get => _volume;
        set
        {
            if (double.IsFinite(value))
                UpdateProperty(ref _volume, System.Math.Clamp(value, VOLUME_MINIMUM, VOLUME_MAXIMUM));
        }
    }
}
