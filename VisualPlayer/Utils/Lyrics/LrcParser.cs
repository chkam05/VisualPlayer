using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using VisualPlayer.Enums;
using VisualPlayer.ViewModels.Lyrics;

namespace VisualPlayer.Utils.Lyrics;

public static partial class LrcParser
{
    #region CONST

    private const string OFFSET_KEY = "offset";

    #endregion CONST

    #region METHODS

    public static LyricsDocument Parse(
        string content,
        string? trackFilePath = null,
        string? fallbackName = null)
    {
        var metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var parsedLines = new List<LyricsLine>();
        var hasLineTiming = false;
        var hasSegmentTiming = false;

        foreach (var sourceLine in content.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n'))
        {
            var line = sourceLine.TrimEnd();
            var timestamps = LineTimestampRegex().Matches(line);

            if (timestamps.Count == 0)
            {
                if (TryReadMetadata(line, metadata))
                    continue;

                if (!string.IsNullOrWhiteSpace(line))
                {
                    parsedLines.Add(new LyricsLine
                    {
                        Text = line
                    });
                }

                continue;
            }

            hasLineTiming = true;
            var textStart = timestamps[^1].Index + timestamps[^1].Length;
            var text = line[textStart..];
            var segmentMatches = SegmentTimestampRegex().Matches(text);

            foreach (Match timestamp in timestamps)
            {
                var lineStart = ParseTimestamp(timestamp);

                if (segmentMatches.Count == 0)
                {
                    parsedLines.Add(new LyricsLine
                    {
                        Start = lineStart,
                        Text = text
                    });
                    continue;
                }

                hasSegmentTiming = true;
                AddExpandedSegments(parsedLines, text, segmentMatches, lineStart);
            }
        }

        ApplyOffset(parsedLines, metadata);
        SetEndTimes(parsedLines);

        metadata.TryGetValue("ti", out var title);

        return new LyricsDocument
        {
            Name = !string.IsNullOrWhiteSpace(title)
                ? title
                : fallbackName ?? Path.GetFileNameWithoutExtension(trackFilePath) ?? string.Empty,
            TrackFilePath = trackFilePath,
            Source = LyricsSource.AdjacentFile,
            Type = hasSegmentTiming
                ? LyricsType.WordSynced
                : hasLineTiming
                    ? LyricsType.LineSynced
                    : LyricsType.Plain,
            LrcMetadata = metadata,
            Lines = new ObservableCollection<LyricsLine>(parsedLines.OrderBy(line => line.Start))
        };
    }

    private static void AddExpandedSegments(
        ICollection<LyricsLine> destination,
        string text,
        MatchCollection matches,
        TimeSpan lineStart)
    {
        var firstMatch = matches[0];
        var prefix = text[..firstMatch.Index];

        if (!string.IsNullOrEmpty(prefix))
        {
            destination.Add(new LyricsLine
            {
                Start = lineStart,
                Text = prefix
            });
        }

        for (var index = 0; index < matches.Count; index++)
        {
            var match = matches[index];
            var valueStart = match.Index + match.Length;
            var valueEnd = index + 1 < matches.Count
                ? matches[index + 1].Index
                : text.Length;

            destination.Add(new LyricsLine
            {
                Start = ParseTimestamp(match),
                Text = text[valueStart..valueEnd]
            });
        }
    }

    private static void ApplyOffset(
        IEnumerable<LyricsLine> lines,
        IDictionary<string, string> metadata)
    {
        if (!metadata.TryGetValue(OFFSET_KEY, out var offsetText)
            || !int.TryParse(offsetText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var offsetMilliseconds))
        {
            return;
        }

        var offset = TimeSpan.FromMilliseconds(offsetMilliseconds);

        foreach (var line in lines)
            line.Start = Max(TimeSpan.Zero, line.Start + offset);

        metadata.Remove(OFFSET_KEY);
    }

    private static TimeSpan Max(TimeSpan first, TimeSpan second)
    {
        return first >= second ? first : second;
    }

    private static TimeSpan ParseTimestamp(Match match)
    {
        var minutes = int.Parse(match.Groups["minutes"].Value, CultureInfo.InvariantCulture);
        var seconds = int.Parse(match.Groups["seconds"].Value, CultureInfo.InvariantCulture);
        var fractionText = match.Groups["fraction"].Value;
        var fraction = fractionText.Length switch
        {
            0 => 0,
            1 => int.Parse(fractionText, CultureInfo.InvariantCulture) * 100,
            2 => int.Parse(fractionText, CultureInfo.InvariantCulture) * 10,
            _ => int.Parse(fractionText[..Math.Min(3, fractionText.Length)], CultureInfo.InvariantCulture)
        };

        return TimeSpan.FromMinutes(minutes)
            + TimeSpan.FromSeconds(seconds)
            + TimeSpan.FromMilliseconds(fraction);
    }

    private static void SetEndTimes(IReadOnlyList<LyricsLine> lines)
    {
        var ordered = lines.OrderBy(line => line.Start).ToArray();

        for (var index = 0; index < ordered.Length - 1; index++)
            ordered[index].End = ordered[index + 1].Start;
    }

    private static bool TryReadMetadata(string line, IDictionary<string, string> metadata)
    {
        var match = MetadataRegex().Match(line);

        if (!match.Success)
            return false;

        metadata[match.Groups["key"].Value] = match.Groups["value"].Value.Trim();
        return true;
    }

    #endregion METHODS

    #region REGEX

    [GeneratedRegex(@"\[(?<minutes>\d{1,3}):(?<seconds>\d{2})(?:[\.:](?<fraction>\d{1,3}))?\]", RegexOptions.Compiled)]
    private static partial Regex LineTimestampRegex();

    [GeneratedRegex(@"^\[(?<key>[a-zA-Z][a-zA-Z0-9]*):(?<value>.*)\]$", RegexOptions.Compiled)]
    private static partial Regex MetadataRegex();

    [GeneratedRegex(@"<(?<minutes>\d{1,3}):(?<seconds>\d{2})(?:[\.:](?<fraction>\d{1,3}))?>", RegexOptions.Compiled)]
    private static partial Regex SegmentTimestampRegex();

    #endregion REGEX
}
