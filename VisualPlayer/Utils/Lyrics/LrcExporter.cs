using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using VisualPlayer.Enums;
using VisualPlayer.ViewModels.Lyrics;

namespace VisualPlayer.Utils.Lyrics;

public static class LrcExporter
{
    #region METHODS

    public static string Export(LyricsDocument document)
    {
        var result = new StringBuilder();
        var metadata = new Dictionary<string, string>(document.LrcMetadata, StringComparer.OrdinalIgnoreCase);

        if (!string.IsNullOrWhiteSpace(document.Name))
            metadata["ti"] = document.Name;

        foreach (var entry in metadata.OrderBy(entry => GetMetadataOrder(entry.Key)))
            result.Append('[').Append(entry.Key).Append(':').Append(entry.Value).AppendLine("]");

        if (metadata.Count > 0 && document.Lines.Count > 0)
            result.AppendLine();

        foreach (var line in document.Lines.OrderBy(line => line.Start))
        {
            if (document.Type == LyricsType.Plain)
            {
                result.AppendLine(line.Text);
                continue;
            }

            var timestamp = FormatTimestamp(line.Start);
            result.Append('[').Append(timestamp).Append(']');

            if (document.Type == LyricsType.WordSynced)
                result.Append('<').Append(timestamp).Append('>');

            result.AppendLine(line.Text);
        }

        return result.ToString();
    }

    private static string FormatTimestamp(TimeSpan value)
    {
        var totalMinutes = (int)value.TotalMinutes;
        return string.Create(
            CultureInfo.InvariantCulture,
            $"{totalMinutes:00}:{value.Seconds:00}.{value.Milliseconds / 10:00}");
    }

    private static int GetMetadataOrder(string key)
    {
        return key.ToLowerInvariant() switch
        {
            "ti" => 0,
            "ar" => 1,
            "al" => 2,
            "by" => 3,
            "offset" => 4,
            "length" => 5,
            "re" => 6,
            "ve" => 7,
            _ => 100
        };
    }

    #endregion METHODS
}
