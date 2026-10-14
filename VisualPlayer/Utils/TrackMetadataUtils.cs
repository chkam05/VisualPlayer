using TagLib;
using System;
using VisualPlayer.Models.Player;
using System.Linq;

namespace VisualPlayer.Utils;

public static class TrackMetadataUtils
{
    public static bool TryLoadMetadata(Track track, out byte[]? coverData)
    {
        ArgumentNullException.ThrowIfNull(track);
        
        TagLib.File file;

        coverData = null;

        try
        {
            file = TagLib.File.Create(track.FilePath);
        }
        catch
        {
            return false;
        }

        using (file)
        {
            if (TryGetValue(() => Normalize(file.Tag.Title), out string? title))
                track.Title = title;

            if (TryGetValue(() => Normalize(file.Tag.Album), out string? album))
                track.Album = album;

            if (TryGetValue(() => GetArtist(file.Tag), out string? artist))
                track.Artist = artist;

            if (TryGetValue(() => file.Properties.Duration, out TimeSpan duration))
                track.Duration = duration;

            TryGetValue(() => GetCoverData(file.Tag), out coverData);
        }

        return true;
    }


    private static string? GetArtist(Tag tag)
    {
        var performers = tag.Performers?
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .ToArray();

        if (performers is { Length: > 0 })
            return string.Join(", ", performers);

        var albumArtists = tag.AlbumArtists?
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .ToArray();

        if (albumArtists is { Length: > 0 })
            return string.Join(", ", albumArtists);

        return null;
    }

    private static byte[]? GetCoverData(Tag tag)
    {
        if (tag.Pictures is not { Length: > 0 })
            return null;

        var picture = tag.Pictures
            .FirstOrDefault(x => x.Type == PictureType.FrontCover)
            ?? tag.Pictures[0];

        if (picture.Data.Count == 0)
            return null;

        return picture.Data.ToArray();
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }

    private static bool TryGetValue<T>(Func<T> getter, out T value)
    {
        try
        {
            value = getter();
            return true;
        }
        catch
        {
            value = default!;
            return false;
        }
    }
}