using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace VisualPlayer.Models.Player;

public class PlayList
{
    #region PROPERTIES

    public string Name { get; set; } = "Now Playing";
    public ObservableCollection<Track> Tracks { get; } = new();

    #endregion PROPERTIES

    #region UTILITIES

    public bool ContainsTrack(string filePath)
    {
        var comparisonType = OperatingSystem.IsLinux()
            ? StringComparison.Ordinal
            : StringComparison.OrdinalIgnoreCase;
        
        return Tracks.Any(track => string.Equals(track.FilePath, filePath, comparisonType));
    }

    #endregion UTILITIES
}
