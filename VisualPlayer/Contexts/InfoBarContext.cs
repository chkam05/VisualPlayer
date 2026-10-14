using System;
using Avalonia.Media;
using VisualPlayer.Attributes;
using VisualPlayer.ViewModels;

namespace VisualPlayer.Contexts;

public class InfoBarContext : ViewModelBase
{
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

    public string Title => Track?.Title ?? "Brak nagrania";
    public string Album => string.IsNullOrWhiteSpace(Track?.Album) ? "Nieznany" : Track.Album;
    public string Artist => string.IsNullOrWhiteSpace(Track?.Artist) ? "Nieznany" : Track.Artist;
    public IImage? Cover => Track?.Cover;


    #region CONSTRUCTORS

    public InfoBarContext(PlayerDataContext player, TrackViewModel? track)
    {
        Player = player ?? throw new ArgumentNullException(nameof(player));
        Track = track;
    }

    #endregion CONSTRUCTORS
}
