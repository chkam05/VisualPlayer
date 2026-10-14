using System;
using System.Globalization;
using Avalonia.Data.Converters;
using VisualPlayer.Enums.Config;
using VisualPlayer.Contexts;

namespace VisualPlayer.Converters;

public class ImageFillModeToTitleConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            ImageFillMode.Normal => ApplicationContext.Instance.Translations["ENUM_IMAGEFILLMODE_NORMAL"],
            ImageFillMode.Center => ApplicationContext.Instance.Translations["ENUM_IMAGEFILLMODE_CENTER"],
            ImageFillMode.Stretch => ApplicationContext.Instance.Translations["ENUM_IMAGEFILLMODE_STRETCH"],
            ImageFillMode.Uniform => ApplicationContext.Instance.Translations["ENUM_IMAGEFILLMODE_UNIFORM"],
            ImageFillMode.UniformToFill => ApplicationContext.Instance.Translations["ENUM_IMAGEFILLMODE_UNIFORMTOFILL"],
            ImageFillMode.Tile => ApplicationContext.Instance.Translations["ENUM_IMAGEFILLMODE_TILE"],
            _ => string.Empty
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
