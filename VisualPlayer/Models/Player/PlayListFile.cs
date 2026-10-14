using System.Collections.Generic;

namespace VisualPlayer.Models.Player;

public class PlayListFile
{
    public string Name { get; set; } = "Teraz Odtwarzane";
    public List<Track> Tracks { get; set; } = new();
    public string? CurrentTrackPath { get; set; }
}
