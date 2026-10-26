using System;
using System.Collections.ObjectModel;
using VisualPlayer.Attributes;
using VisualPlayer.Enums.Library;

namespace VisualPlayer.ViewModels.Config;

public class LibraryConfig : ViewModelBase
{
    private ObservableCollection<LibraryFolderConfig> _folders = [];
    private bool _showArtistAlbums;
    private double _albumsLayout;
    private double _artistsLayout;
    private double _directoriesLayout;
    private double _tracksLayout;
    private LibrarySection _lastSection = LibrarySection.Tracks;

    #region PROPERTIES

    [NotifyPropertyUpdate]
    public LibrarySection LastSection
    {
        get => _lastSection;
        set => UpdateProperty(ref _lastSection, Enum.IsDefined(value) ? value : LibrarySection.Tracks);
    }

    public ObservableCollection<LibraryFolderConfig> Folders
    {
        get => _folders;
        set => _folders = value ?? [];
    }

    [NotifyPropertyUpdate]
    public double AlbumsLayout
    {
        get => _albumsLayout;
        set => UpdateProperty(ref _albumsLayout, value);
    }

    [NotifyPropertyUpdate]
    public double ArtistsLayout
    {
        get => _artistsLayout;
        set => UpdateProperty(ref _artistsLayout, value);
    }

    [NotifyPropertyUpdate]
    public double DirectoriesLayout
    {
        get => _directoriesLayout;
        set => UpdateProperty(ref _directoriesLayout, value);
    }

    [NotifyPropertyUpdate]
    public bool ShowArtistAlbums
    {
        get => _showArtistAlbums;
        set => UpdateProperty(ref _showArtistAlbums, value);
    }

    [NotifyPropertyUpdate]
    public double TracksLayout
    {
        get => _tracksLayout;
        set => UpdateProperty(ref _tracksLayout, value);
    }

    #endregion PROPERTIES
}
