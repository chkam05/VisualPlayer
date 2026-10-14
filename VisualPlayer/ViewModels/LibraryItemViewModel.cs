using System.Collections.Generic;
using System.IO;
using Avalonia.Media;
using Lucide.Avalonia;
using VisualPlayer.Attributes;
using VisualPlayer.Enums.Library;
using VisualPlayer.Models.Player;

namespace VisualPlayer.ViewModels;

public class LibraryItemViewModel : ViewModelBase
{
    private bool _isSelected;

    #region PROPERTIES

    public IImage? Cover { get; init; }
    public LibraryItemKind Kind { get; init; }
    public string? Path { get; init; }
    public string Subtitle { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public Track? Track { get; init; }
    public IReadOnlyList<Track> Tracks { get; init; } = [];

    public LucideIconKind Icon
    {
        get
        {
            switch (Kind)
            {
                case LibraryItemKind.Album:
                    return LucideIconKind.DiscAlbum;

                case LibraryItemKind.Artist:
                    return LucideIconKind.UserRound;

                case LibraryItemKind.Directory:
                    return LucideIconKind.Folder;

                default:
                    return LucideIconKind.Music;
            }
        }
    }

    [NotifyPropertyUpdate]
    public bool IsSelected
    {
        get => _isSelected;
        set => UpdateProperty(ref _isSelected, value);
    }

    public string FileName
    {
        get => Track is null ? string.Empty : System.IO.Path.GetFileName(Track.FilePath);
    }

    #endregion PROPERTIES
}
