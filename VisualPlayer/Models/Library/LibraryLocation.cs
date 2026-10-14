using VisualPlayer.Enums.Library;

namespace VisualPlayer.Models.Library;

public record LibraryLocation
{
    #region PROPERTIES

    public string? Album { get; init; }
    public string? Artist { get; init; }
    public string? Directory { get; init; }
    public LibrarySection Section { get; init; }

    #endregion PROPERTIES

    #region CONSTRUCTORS

    public LibraryLocation(
        LibrarySection section,
        string? artist,
        string? album,
        string? directory)
    {
        Section = section;
        Artist = artist;
        Album = album;
        Directory = directory;
    }

    #endregion CONSTRUCTORS
}
