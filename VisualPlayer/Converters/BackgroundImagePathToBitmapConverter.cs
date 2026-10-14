using System;
using System.Globalization;
using System.IO;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace VisualPlayer.Converters;

public class BackgroundImagePathToBitmapConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string path || string.IsNullOrWhiteSpace(path))
            return null;

        try
        {
            if (Uri.TryCreate(path, UriKind.Absolute, out var uri) && uri.Scheme == "avares")
            {
                using var stream = AssetLoader.Open(uri);
                return new Bitmap(stream);
            }

            return File.Exists(path)
                ? new Bitmap(path)
                : null;
        }
        catch
        {
            return null;
        }
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
