using System.Collections.Generic;

namespace VisualPlayer.Models.Player;

public class PlayListFile
{
    public string Name { get; set; } = "Now Playing";
    public List<Track> Tracks { get; set; } = new();
    public string? CurrentTrackPath { get; set; }
}
