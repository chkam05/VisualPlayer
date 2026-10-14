using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Avalonia;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using VisualPlayer.Enums.Config;

namespace VisualPlayer.Converters;

public class BackgroundImageBrushConverter : IMultiValueConverter
{
    public static BackgroundImageBrushConverter Instance { get; } = new();

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count < 2
            || values[0] is not string path
            || string.IsNullOrWhiteSpace(path)
            || values[1] is not ImageFillMode fillMode)
        {
            return null;
        }

        try
        {
            Bitmap bitmap;

            if (Uri.TryCreate(path, UriKind.Absolute, out var uri) && uri.Scheme == "avares")
            {
                using var stream = AssetLoader.Open(uri);
                bitmap = new Bitmap(stream);
            }
            else
            {
                if (!File.Exists(path))
                    return null;

                bitmap = new Bitmap(path);
            }

            var brush = new ImageBrush(bitmap);

            switch (fillMode)
            {
                case ImageFillMode.Normal:
                    brush.Stretch = Stretch.None;
                    brush.AlignmentX = AlignmentX.Left;
                    brush.AlignmentY = AlignmentY.Top;
                    break;

                case ImageFillMode.Center:
                    brush.Stretch = Stretch.None;
                    brush.AlignmentX = AlignmentX.Center;
                    brush.AlignmentY = AlignmentY.Center;
                    break;

                case ImageFillMode.Stretch:
                    brush.Stretch = Stretch.Fill;
                    break;

                case ImageFillMode.Uniform:
                    brush.Stretch = Stretch.Uniform;
                    break;

                case ImageFillMode.UniformToFill:
                    brush.Stretch = Stretch.UniformToFill;
                    break;

                case ImageFillMode.Tile:
                    brush.Stretch = Stretch.None;
                    brush.TileMode = TileMode.Tile;
                    brush.DestinationRect = new RelativeRect(
                        new Rect(
                            new Point(0, 0),
                            new Size(bitmap.PixelSize.Width, bitmap.PixelSize.Height)),
                        RelativeUnit.Absolute);
                    break;
            }

            return brush;
        }
        catch
        {
            return null;
        }
    }
}
