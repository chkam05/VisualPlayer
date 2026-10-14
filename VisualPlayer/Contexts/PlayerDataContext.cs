using System;
using VisualPlayer.Attributes;
using VisualPlayer.Enums.Player;
using VisualPlayer.ViewModels;

namespace VisualPlayer.Contexts;

public class PlayerDataContext : ViewModelBase
{
    private double _duration;
    private bool _isMuted;
    private bool _isPlaying;
    private bool _isShuffleEnabled;
    private double _playbackSpeed = 1;
    private double _position;
    private RepeatMode _repeatMode;
    private bool _showRemainingTime;
    private double _volume = 0.75;


    [NotifyPropertyUpdate]
    public double Duration
    {
        get => _duration;
        set
        {
            if (_duration == value)
                return;

            UpdateProperty(ref _duration, value);
            NotifyPropertyChanged(nameof(DurationText));
            NotifyPropertyChanged(nameof(TotalDurationText));
            NotifyPropertyChanged(nameof(PositionMaximum));
        }
    }

    [NotifyPropertyUpdate]
    public bool IsPlaying
    {
        get => _isPlaying;
        set => UpdateProperty(ref _isPlaying, value);
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
    public double PlaybackSpeed
    {
        get => _playbackSpeed;
        set
        {
            if (double.IsFinite(value))
                UpdateProperty(ref _playbackSpeed, Math.Clamp(value, 0.5, 2));
        }
    }

    [NotifyPropertyUpdate]
    public double Position
    {
        get => _position;
        set
        {
            if (_position == value)
                return;

            UpdateProperty(ref _position, value);
            NotifyPropertyChanged(nameof(CurrentPositionPreciseText));
            NotifyPropertyChanged(nameof(CurrentPositionText));
            NotifyPropertyChanged(nameof(DurationText));
        }
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
        set
        {
            if (_showRemainingTime == value)
                return;

            UpdateProperty(ref _showRemainingTime, value);
            NotifyPropertyChanged(nameof(DurationText));
        }
    }

    [NotifyPropertyUpdate]
    public double Volume
    {
        get => _volume;
        set
        {
            if (double.IsFinite(value))
                UpdateProperty(ref _volume, Math.Clamp(value, 0, 1));
        }
    }


    public string CurrentPositionPreciseText => FormatPreciseTime(Position);

    public string CurrentPositionText => FormatTime(Position);

    public string DurationText => ShowRemainingTime
        ? $"-{FormatTime(Math.Max(0, Duration - Position))}"
        : FormatTime(Duration);

    public double PositionMaximum => Duration > 0 ? Duration : 1;

    public string TotalDurationText => FormatTime(Duration);
    
    #region UTILITIES
    
    private static string FormatTime(double totalSeconds)
    {
        var time = TimeSpan.FromSeconds(Math.Max(0, totalSeconds));
        return time.TotalHours >= 1
            ? $"{(int)time.TotalHours:00}:{time.Minutes:00}:{time.Seconds:00}"
            : $"{time.Minutes:00}:{time.Seconds:00}";
    }

    private static string FormatPreciseTime(double totalSeconds)
    {
        var time = TimeSpan.FromSeconds(Math.Max(0, totalSeconds));
        return $"{(int)time.TotalMinutes:00}:{time.Seconds:00}.{time.Milliseconds / 10:00}";
    }

    #endregion UTILITIES
}
