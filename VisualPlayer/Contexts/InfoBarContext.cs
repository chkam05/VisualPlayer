using System;
using Avalonia.Media;
using VisualPlayer.Attributes;
using VisualPlayer.ViewModels;

namespace VisualPlayer.Contexts;

public class InfoBarContext : ViewModelBase, IDisposable
{
    private readonly TranslationContext _translations;
    private TrackViewModel? _track;

    public PlayerDataContext Player { get; private set; }

    [NotifyPropertyUpdate]
    public TrackViewModel? Track
    {
        get => _track;
        internal set
        {
            if (ReferenceEquals(_track, value))
                return;

            UpdateProperty(ref _track, value);
            NotifyPropertyChanged(nameof(Title));
            NotifyPropertyChanged(nameof(Album));
            NotifyPropertyChanged(nameof(Artist));
            NotifyPropertyChanged(nameof(Cover));
        }
    }

    public string Title => Track?.Title ?? _translations["INFOBAR_NO_TRACK"];
    public string AlbumLabel => _translations["INFOBAR_TEXT_ALBUM"];
    public string Album => string.IsNullOrWhiteSpace(Track?.Album)
        ? _translations["INFOBAR_UNKNOWN"]
        : Track.Album;
    public string ArtistLabel => _translations["INFOBAR_TEXT_ARTIST"];
    public string Artist => string.IsNullOrWhiteSpace(Track?.Artist)
        ? _translations["INFOBAR_UNKNOWN"]
        : Track.Artist;
    public IImage? Cover => Track?.Cover;


    #region CONSTRUCTORS

    public InfoBarContext(
        PlayerDataContext player,
        TrackViewModel? track,
        TranslationContext translations)
    {
        Player = player ?? throw new ArgumentNullException(nameof(player));
        _translations = translations ?? throw new ArgumentNullException(nameof(translations));
        _translations.LanguageChanged += Translations_LanguageChanged;
        Track = track;
    }

    #endregion CONSTRUCTORS

    public void Dispose()
    {
        _translations.LanguageChanged -= Translations_LanguageChanged;
    }

    private void Translations_LanguageChanged(object? sender, EventArgs e)
    {
        NotifyPropertyChanged(nameof(Title));
        NotifyPropertyChanged(nameof(AlbumLabel));
        NotifyPropertyChanged(nameof(Album));
        NotifyPropertyChanged(nameof(ArtistLabel));
        NotifyPropertyChanged(nameof(Artist));
    }
}
