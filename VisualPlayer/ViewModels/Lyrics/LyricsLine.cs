using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;
using VisualPlayer.Attributes;
using VisualPlayer.Models.Lyrics;
using VisualPlayer.ViewModels;

namespace VisualPlayer.ViewModels.Lyrics;

public class LyricsLine : ViewModelBase
{
    private TimeSpan? _end;
    private bool _hasTimeError;
    private ObservableCollection<LyricsSegment> _segments = [];
    private TimeSpan _start;
    private string _text = string.Empty;
    private string _timeCode = "00:00.00";

    #region PROPERTIES

    [NotifyPropertyUpdate]
    public TimeSpan? End
    {
        get => _end;
        set => UpdateProperty(ref _end, value);
    }

    [NotifyPropertyUpdate]
    public ObservableCollection<LyricsSegment> Segments
    {
        get => _segments;
        set => UpdateProperty(ref _segments, value ?? []);
    }

    public bool HasTimeError
    {
        get => _hasTimeError;
        private set
        {
            if (_hasTimeError == value)
                return;

            _hasTimeError = value;
            NotifyPropertyChanged(nameof(HasTimeError));
            NotifyPropertyChanged(nameof(TimeError));
        }
    }

    [NotifyPropertyUpdate]
    public TimeSpan Start
    {
        get => _start;
        set
        {
            if (_start == value)
                return;

            UpdateProperty(ref _start, value);
            _timeCode = FormatTimeCode(value);
            NotifyPropertyChanged(nameof(TimeCode));
        }
    }

    [NotifyPropertyUpdate]
    public string Text
    {
        get => _text;
        set => UpdateProperty(ref _text, value ?? string.Empty);
    }

    public string TimeCode
    {
        get => _timeCode;
        set
        {
            _timeCode = value ?? string.Empty;
            NotifyPropertyChanged(nameof(TimeCode));

            if (TryParseTimeCode(_timeCode, out var result))
            {
                HasTimeError = false;
                Start = result < TimeSpan.Zero
                    ? TimeSpan.Zero
                    : result;
                return;
            }

            HasTimeError = true;
        }
    }

    public string? TimeError
    {
        get => HasTimeError
            ? "Wpisz czas w formacie mm:ss.ff, np. 01:23.45."
            : null;
    }

    private static string FormatTimeCode(TimeSpan value)
    {
        return string.Create(
            CultureInfo.InvariantCulture,
            $"{(int)value.TotalMinutes:00}:{value.Seconds:00}.{value.Milliseconds / 10:00}");
    }

    private static bool TryParseTimeCode(string value, out TimeSpan result)
    {
        var match = Regex.Match(
            value.Trim(),
            @"^(?<minutes>\d{1,4}):(?<seconds>[0-5]\d)(?:[\.:](?<fraction>\d{1,3}))?$",
            RegexOptions.CultureInvariant);

        if (!match.Success)
        {
            result = default;
            return false;
        }

        var minutes = int.Parse(match.Groups["minutes"].Value, CultureInfo.InvariantCulture);
        var seconds = int.Parse(match.Groups["seconds"].Value, CultureInfo.InvariantCulture);
        var fractionText = match.Groups["fraction"].Value;
        var milliseconds = fractionText.Length switch
        {
            0 => 0,
            1 => int.Parse(fractionText, CultureInfo.InvariantCulture) * 100,
            2 => int.Parse(fractionText, CultureInfo.InvariantCulture) * 10,
            _ => int.Parse(fractionText[..3], CultureInfo.InvariantCulture)
        };

        result = TimeSpan.FromMinutes(minutes)
            + TimeSpan.FromSeconds(seconds)
            + TimeSpan.FromMilliseconds(milliseconds);
        return true;
    }

    #endregion PROPERTIES
}
