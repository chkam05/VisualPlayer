using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using VisualPlayer.Attributes;
using VisualPlayer.Enums;
using VisualPlayer.ViewModels;

namespace VisualPlayer.ViewModels.Lyrics;

public class LyricsDocument : ViewModelBase
{
    private bool _isCurrentTrack;
    private string? _album;
    private string? _artist;
    private string? _language;
    private Dictionary<string, string> _lrcMetadata = new(StringComparer.OrdinalIgnoreCase);
    private ObservableCollection<LyricsLine> _lines = [];
    private string _name = string.Empty;
    private string? _providerId;
    private string? _providerName;
    private string? _trackFilePath;
    private LyricsType _type;
    private LyricsSource _source;

    #region PROPERTIES

    public Guid Id { get; set; } = Guid.NewGuid();

    [NotifyPropertyUpdate]
    public string? Album
    {
        get => _album;
        set => UpdateProperty(ref _album, value);
    }

    [NotifyPropertyUpdate]
    public string? Artist
    {
        get => _artist;
        set => UpdateProperty(ref _artist, value);
    }

    [NotifyPropertyUpdate]
    public string? Language
    {
        get => _language;
        set => UpdateProperty(ref _language, value);
    }

    [NotifyPropertyUpdate]
    public Dictionary<string, string> LrcMetadata
    {
        get => _lrcMetadata;
        set => UpdateProperty(ref _lrcMetadata, value ?? new(StringComparer.OrdinalIgnoreCase));
    }

    [NotifyPropertyUpdate]
    public LyricsSource Source
    {
        get => _source;
        set => UpdateProperty(ref _source, value);
    }

    [NotifyPropertyUpdate]
    public string? ProviderId
    {
        get => _providerId;
        set => UpdateProperty(ref _providerId, value);
    }

    [NotifyPropertyUpdate]
    public string? ProviderName
    {
        get => _providerName;
        set => UpdateProperty(ref _providerName, value);
    }

    [JsonIgnore]
    [NotifyPropertyUpdate]
    public bool IsCurrentTrack
    {
        get => _isCurrentTrack;
        set => UpdateProperty(ref _isCurrentTrack, value);
    }

    [NotifyPropertyUpdate]
    public ObservableCollection<LyricsLine> Lines
    {
        get => _lines;
        set => UpdateProperty(ref _lines, value ?? []);
    }

    [NotifyPropertyUpdate]
    public string Name
    {
        get => _name;
        set => UpdateProperty(ref _name, value ?? string.Empty);
    }

    [NotifyPropertyUpdate]
    public string? TrackFilePath
    {
        get => _trackFilePath;
        set
        {
            if (_trackFilePath == value)
                return;

            UpdateProperty(ref _trackFilePath, value);
            NotifyPropertyChanged(nameof(IsAssigned));
        }
    }

    [NotifyPropertyUpdate]
    public LyricsType Type
    {
        get => _type;
        set => UpdateProperty(ref _type, value);
    }


    [JsonIgnore]
    public bool IsAssigned
    {
        get => !string.IsNullOrWhiteSpace(TrackFilePath);
    }

    [JsonIgnore]
    public string StorageFileName
    {
        get => $"{Id:N}.lyrics.json";
    }

    [JsonIgnore]
    public string SuggestedName
    {
        get => !string.IsNullOrWhiteSpace(Name)
            ? Name
            : Path.GetFileNameWithoutExtension(TrackFilePath) ?? string.Empty;
    }

    #endregion PROPERTIES
}
