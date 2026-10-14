using System;

namespace VisualPlayer.Models.Lyrics;

public class LyricsSegment
{
    public TimeSpan? End { get; set; }
    public TimeSpan Start { get; set; }
    public string Text { get; set; } = string.Empty;
}
