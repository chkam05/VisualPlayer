using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using TagLib;
using TagLib.Id3v2;
using VisualPlayer.Enums;
using VisualPlayer.Models.Player;
using VisualPlayer.ViewModels.Lyrics;

namespace VisualPlayer.Utils.Lyrics;

public static class EmbeddedLyricsReader
{
    #region METHODS

    public static LyricsDocument? Read(Track track)
    {
        ArgumentNullException.ThrowIfNull(track);

        try
        {
            using var file = TagLib.File.Create(track.FilePath);
            var synchronized = ReadSynchronizedLyrics(file, track);

            if (synchronized is not null)
                return synchronized;

            return ReadCommonLyrics(file.Tag, track);
        }
        catch (CorruptFileException)
        {
            return null;
        }
        catch (UnsupportedFormatException)
        {
            return null;
        }
        catch (IOException)
        {
            return null;
        }
        catch (UnauthorizedAccessException)
        {
            return null;
        }
    }

    private static LyricsDocument? ReadCommonLyrics(TagLib.Tag tag, Track track)
    {
        if (string.IsNullOrWhiteSpace(tag.Lyrics))
            return null;

        var document = LrcParser.Parse(tag.Lyrics, track.FilePath, track.DisplayTitle);

        if (document.Lines.Count == 0)
            return null;

        ApplyTrackMetadata(document, track);
        document.Source = LyricsSource.EmbeddedMetadata;
        return document;
    }

    private static LyricsDocument? ReadSynchronizedLyrics(TagLib.File file, Track track)
    {
        if (file.GetTag(TagTypes.Id3v2) is not TagLib.Id3v2.Tag id3Tag)
            return null;

        var frame = id3Tag
            .GetFrames<SynchronisedLyricsFrame>()
            .FirstOrDefault(candidate => candidate.Type == SynchedTextType.Lyrics
                && candidate.Format == TimestampFormat.AbsoluteMilliseconds
                && candidate.Text.Length > 0);

        if (frame is null)
            return null;

        var entries = frame.Text
            .Where(entry => !string.IsNullOrEmpty(entry.Text))
            .OrderBy(entry => entry.Time)
            .ToArray();

        if (entries.Length == 0)
            return null;

        var lines = new ObservableCollection<LyricsLine>();

        for (var index = 0; index < entries.Length; index++)
        {
            lines.Add(new LyricsLine
            {
                Start = TimeSpan.FromMilliseconds(Math.Max(0, entries[index].Time)),
                End = index + 1 < entries.Length
                    ? TimeSpan.FromMilliseconds(Math.Max(0, entries[index + 1].Time))
                    : null,
                Text = entries[index].Text
            });
        }

        var metadata = CreateTrackMetadata(track);

        if (!string.IsNullOrWhiteSpace(frame.Language))
            metadata["language"] = frame.Language;

        return new LyricsDocument
        {
            Name = track.DisplayTitle,
            TrackFilePath = track.FilePath,
            Type = LyricsType.LineSynced,
            Source = LyricsSource.EmbeddedMetadata,
            LrcMetadata = metadata,
            Lines = lines
        };
    }

    private static void ApplyTrackMetadata(LyricsDocument document, Track track)
    {
        document.Name = track.DisplayTitle;
        document.TrackFilePath = track.FilePath;

        foreach (var entry in CreateTrackMetadata(track))
            document.LrcMetadata.TryAdd(entry.Key, entry.Value);
    }

    private static Dictionary<string, string> CreateTrackMetadata(Track track)
    {
        var metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["ti"] = track.DisplayTitle
        };

        if (!string.IsNullOrWhiteSpace(track.Artist))
            metadata["ar"] = track.Artist;

        if (!string.IsNullOrWhiteSpace(track.Album))
            metadata["al"] = track.Album;

        return metadata;
    }

    #endregion METHODS
}
