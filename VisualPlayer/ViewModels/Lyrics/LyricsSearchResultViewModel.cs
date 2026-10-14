using System;
using VisualPlayer.Attributes;
using VisualPlayer.Enums;
using VisualPlayer.Providers.Lyrics;

namespace VisualPlayer.ViewModels.Lyrics;

public class LyricsSearchResultViewModel : ViewModelBase
{
    private string _album = string.Empty;
    private string _artist = string.Empty;
    private TimeSpan _duration;
    private string _name = string.Empty;
    private LyricsType _type;

    internal LyricsProviderResult Result { get; }

    [NotifyPropertyUpdate]
    public string Album
    {
        get => _album;
        set => UpdateProperty(ref _album, value ?? string.Empty);
    }

    [NotifyPropertyUpdate]
    public string Artist
    {
        get => _artist;
        set => UpdateProperty(ref _artist, value ?? string.Empty);
    }

    [NotifyPropertyUpdate]
    public TimeSpan Duration
    {
        get => _duration;
        set => UpdateProperty(ref _duration, value);
    }

    [NotifyPropertyUpdate]
    public string Name
    {
        get => _name;
        set => UpdateProperty(ref _name, value ?? string.Empty);
    }

    [NotifyPropertyUpdate]
    public LyricsType Type
    {
        get => _type;
        set => UpdateProperty(ref _type, value);
    }

    public LyricsSearchResultViewModel(LyricsProviderResult result)
    {
        Result = result;
        Album = result.AlbumName;
        Artist = result.ArtistName;
        Duration = TimeSpan.FromSeconds(result.Duration);
        Name = result.TrackName;
        Type = !string.IsNullOrWhiteSpace(result.SyncedLyrics)
            ? LyricsType.LineSynced
            : LyricsType.Plain;
    }
}
