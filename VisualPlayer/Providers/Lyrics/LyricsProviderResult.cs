namespace VisualPlayer.Providers.Lyrics;

public class LyricsProviderResult
{
    public string AlbumName { get; init; } = string.Empty;
    public string ArtistName { get; init; } = string.Empty;
    public double Duration { get; init; }
    public long Id { get; init; }
    public bool Instrumental { get; init; }
    public string? PlainLyrics { get; init; }
    public string? SyncedLyrics { get; init; }
    public string TrackName { get; init; } = string.Empty;
}
