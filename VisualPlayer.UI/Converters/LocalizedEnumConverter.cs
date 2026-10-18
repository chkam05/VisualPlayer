using System.Globalization;
using Avalonia.Data.Converters;
using VisualPlayer.UI.Localization;
using VisualPlayer.VisualizationBase.Localization;

namespace VisualPlayer.Converters;

/// <summary>
/// MultiBinding inputs: value, catalog, catalog.CurrentLanguageCode. The last input
/// invalidates converted enum labels when language changes in an existing view.
/// Single bindings can supply a catalog through Translations and default to English.
/// </summary>
public abstract class LocalizedEnumConverter : IValueConverter, IMultiValueConverter
{
    public ITranslationScope Translations { get; set; } = UiCatalogs.English;
    protected abstract string? GetTranslationKey(object? value);
    private object Translate(object? value, ITranslationScope scope)
        => GetTranslationKey(value) is { } key ? scope[key] : value?.ToString() ?? string.Empty;
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => Translate(value, Translations);
    public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        => Translate(values.Count > 0 ? values[0] : null,
            values.Count > 1 && values[1] is ITranslationScope scope ? scope : Translations);
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
