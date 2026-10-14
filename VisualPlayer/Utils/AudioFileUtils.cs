using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia.Platform.Storage;

namespace VisualPlayer.Utils;

public static class AudioFileUtils
{
    #region CONST

    private static readonly HashSet<string> SUPPORTED_EXTENSIONS = new(StringComparer.OrdinalIgnoreCase)
    {
        ".aac",
        ".aif",
        ".aiff",
        ".asf",
        ".flac",
        ".m4a",
        ".mp3",
        ".mp4",
        ".ogg",
        ".opus",
        ".wav",
        ".wma"
    };

    #endregion CONST

    #region PROPERTIES

    public static List<string> FilePickerAudioTypes
    {
        get => SUPPORTED_EXTENSIONS.Select(e => $"*{e}").ToList();
    }

    #endregion PROPERTIES

    public static bool IsSupported(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return false;

        if (!File.Exists(filePath))
            return false;

        var extension = Path.GetExtension(filePath);

        return SUPPORTED_EXTENSIONS.Contains(extension);
    }

    public static string? NormalizePath(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return null;

        try
        {
            return Path.GetFullPath(filePath);
        }
        catch
        {
            return null;
        }
    }
}
