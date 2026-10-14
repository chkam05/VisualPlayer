using System;
using Avalonia.Media;
using VisualPlayer.Attributes;
using VisualPlayer.Core;
using VisualPlayer.Models.Player;

namespace VisualPlayer.ViewModels;

public class TrackViewModel : ViewModelBase
{
    #region CONST

    private const string FULL_TIME_FORMAT = @"hh\:mm\:ss";
    private const string TIME_FORMAT = @"mm\:ss";

    #endregion CONST

    #region EVENTS

    public event EventHandler? SelectionChanged;

    #endregion EVENTS

    #region PRIVATE VARIABLES

    private readonly CoverCache _coverCache;
    
    private bool _isCurrentPlaying;
    private bool _isSelected;
    private bool _isSelectionMode;

    #endregion PRIVATE VARIABLES

    #region PROPERTIES

    public Track Model { get; }

    public string Album => Model.DisplayAlbum ?? string.Empty;
    public string Artist => Model.Artist ?? string.Empty;
    public IImage? Cover => _coverCache.GetImage(Model.CoverId);
    public TimeSpan Duration => Model.Duration;
    public string DurationText
    {
        get => Model.Duration.ToString(
            Model.Duration.TotalHours >= 1
                ? FULL_TIME_FORMAT
                : TIME_FORMAT);
    }
    public string FilePath => Model.FilePath;
    public string Title => Model.DisplayTitle;
    
    
    [NotifyPropertyUpdate]
    public bool IsCurrentPlaying
    {
        get => _isCurrentPlaying;
        internal set => UpdateProperty(ref _isCurrentPlaying, value);
    }

    [NotifyPropertyUpdate]
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected == value)
                return;

            UpdateProperty(ref _isSelected, value);
            SelectionChanged?.Invoke(this, System.EventArgs.Empty);
        }
    }

    [NotifyPropertyUpdate]
    public bool IsSelectionMode
    {
        get => _isSelectionMode;
        internal set => UpdateProperty(ref _isSelectionMode, value);
    }

    #endregion PROPERTIES

    #region CONSTRUCTORS

    public TrackViewModel(Track model, CoverCache coverCache)
    {
        Model = model;
        _coverCache = coverCache;
    }

    #endregion CONSTRUCTORS
}