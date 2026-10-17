using System;
using System.Globalization;
using System.Text;
using Avalonia.Data.Converters;

namespace VisualPlayer.Converters;

public sealed class LatinTransliterationConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string text || string.IsNullOrEmpty(text))
            return value;

        var normalized = text.Normalize(NormalizationForm.FormD);
        var result = new StringBuilder(normalized.Length);

        foreach (var character in normalized)
        {
            var category = CharUnicodeInfo.GetUnicodeCategory(character);
            if (category is UnicodeCategory.NonSpacingMark
                or UnicodeCategory.SpacingCombiningMark
                or UnicodeCategory.EnclosingMark)
            {
                continue;
            }

            result.Append(character switch
            {
                'Ł' => 'L',
                'ł' => 'l',
                'Ð' or 'Đ' => 'D',
                'ð' or 'đ' => 'd',
                'Þ' => "TH",
                'þ' => "th",
                'Æ' => "AE",
                'æ' => "ae",
                'Œ' => "OE",
                'œ' => "oe",
                'Ø' => 'O',
                'ø' => 'o',
                'ß' => "ss",
                _ => character.ToString()
            });
        }

        return result.ToString().Normalize(NormalizationForm.FormC);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
