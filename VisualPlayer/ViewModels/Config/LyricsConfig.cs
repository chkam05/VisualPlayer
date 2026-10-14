using System;
using VisualPlayer.Attributes;
using VisualPlayer.Enums;

namespace VisualPlayer.ViewModels.Config;

public class LyricsConfig : ViewModelBase
{
    #region CONST

    public const int SEEK_INTERVAL_INCREMENT = 1;
    public const int SEEK_INTERVAL_MAXIMUM = 30;
    public const int SEEK_INTERVAL_MINIMUM = 1;

    public const int DEFAULT_SEEK_SHORT_INTERVAL = 1;
    public const int DEFAULT_SEEK_MEDIUM_INTERVAL = 3;
    public const int DEFAULT_SEEK_LONG_INTERVAL = 5;
    public const double BACKGROUND_OPACITY_INCREMENT = 0.01;
    public const double BACKGROUND_OPACITY_MAXIMUM = 1.0;
    public const double BACKGROUND_OPACITY_MINIMUM = 0.5;
    public const double DEFAULT_BACKGROUND_OPACITY = 0.85;
    public const int FONT_SIZE_INCREMENT = 1;
    public const int FONT_SIZE_MAXIMUM = 72;
    public const int FONT_SIZE_MINIMUM = 10;
    public const int DEFAULT_CURRENT_LINE_FONT_SIZE = 32;
    public const int DEFAULT_SURROUNDING_LINE_FONT_SIZE = 14;
    public const int DEFAULT_PLAIN_TEXT_FONT_SIZE = 20;

    #endregion CONST

    private bool _autoDownloadLyrics;
    private double _backgroundOpacity = DEFAULT_BACKGROUND_OPACITY;
    private int _currentLineFontSize = DEFAULT_CURRENT_LINE_FONT_SIZE;
    private bool _autoImportEmbeddedLyrics;
    private bool _displayLyrics = true;
    private LyricsType _preferredType = LyricsType.LineSynced;
    private int _plainTextFontSize = DEFAULT_PLAIN_TEXT_FONT_SIZE;
    private int _seekLongInterval = DEFAULT_SEEK_LONG_INTERVAL;
    private int _seekMediumInterval = DEFAULT_SEEK_MEDIUM_INTERVAL;
    private int _seekShortInterval = DEFAULT_SEEK_SHORT_INTERVAL;
    private int _surroundingLineFontSize = DEFAULT_SURROUNDING_LINE_FONT_SIZE;
    private bool _useEmbeddedLyrics = true;

    #region PROPERTIES

    [NotifyPropertyUpdate]
    public bool AutoDownloadLyrics
    {
        get => _autoDownloadLyrics;
        set => UpdateProperty(ref _autoDownloadLyrics, value);
    }

    [NotifyPropertyUpdate]
    public bool AutoImportEmbeddedLyrics
    {
        get => _autoImportEmbeddedLyrics;
        set => UpdateProperty(ref _autoImportEmbeddedLyrics, value);
    }

    [NotifyPropertyUpdate]
    public double BackgroundOpacity
    {
        get => _backgroundOpacity;
        set => UpdateProperty(ref _backgroundOpacity, Math.Clamp(value, BACKGROUND_OPACITY_MINIMUM, BACKGROUND_OPACITY_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public int CurrentLineFontSize
    {
        get => _currentLineFontSize;
        set => UpdateProperty(ref _currentLineFontSize, Math.Clamp(value, FONT_SIZE_MINIMUM, FONT_SIZE_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public int PlainTextFontSize
    {
        get => _plainTextFontSize;
        set => UpdateProperty(ref _plainTextFontSize, Math.Clamp(value, FONT_SIZE_MINIMUM, FONT_SIZE_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public int SurroundingLineFontSize
    {
        get => _surroundingLineFontSize;
        set => UpdateProperty(ref _surroundingLineFontSize, Math.Clamp(value, FONT_SIZE_MINIMUM, FONT_SIZE_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public bool DisplayLyrics
    {
        get => _displayLyrics;
        set => UpdateProperty(ref _displayLyrics, value);
    }

    [NotifyPropertyUpdate]
    public LyricsType PreferredType
    {
        get => _preferredType;
        set => UpdateProperty(ref _preferredType, value);
    }

    [NotifyPropertyUpdate]
    public bool UseEmbeddedLyrics
    {
        get => _useEmbeddedLyrics;
        set => UpdateProperty(ref _useEmbeddedLyrics, value);
    }

    [NotifyPropertyUpdate]
    public int SeekLongInterval
    {
        get => _seekLongInterval;
        set => UpdateProperty(ref _seekLongInterval, Math.Clamp(value, SEEK_INTERVAL_MINIMUM, SEEK_INTERVAL_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public int SeekMediumInterval
    {
        get => _seekMediumInterval;
        set => UpdateProperty(ref _seekMediumInterval, Math.Clamp(value, SEEK_INTERVAL_MINIMUM, SEEK_INTERVAL_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public int SeekShortInterval
    {
        get => _seekShortInterval;
        set => UpdateProperty(ref _seekShortInterval, Math.Clamp(value, SEEK_INTERVAL_MINIMUM, SEEK_INTERVAL_MAXIMUM));
    }

    #endregion PROPERTIES
}
