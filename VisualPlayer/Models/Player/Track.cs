using System;
using System.IO;
using System.Text.Json.Serialization;

namespace VisualPlayer.Models.Player;

public class Track
{
    #region PROPERTIES

    public required string FilePath { get; init; }
    public string? Title { get; set; }
    public string? Album { get; set; }
    public string? Artist { get; set; }
    public TimeSpan Duration { get; set; }
    [JsonIgnore]
    public string? CoverId { get; set; }


    [JsonIgnore]
    public string DisplayTitle
    {
        get => !string.IsNullOrWhiteSpace(Title)
            ? Title
            : Path.GetFileNameWithoutExtension(FilePath);
    }
    
    [JsonIgnore]
    public string? DisplayAlbum
    {
        get => !string.IsNullOrWhiteSpace(Album)
            ? Album
            : Path.GetFileName(Path.GetDirectoryName(FilePath));
    }

    #endregion PROPERTIES
}
