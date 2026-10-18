using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.Json;
using VisualPlayer.ViewModels;

namespace VisualPlayer.VisualizationBase.Localization;

/// <summary>An independent catalog with English fallback, owned by a TranslationService.</summary>
public sealed class TranslationCatalog : ViewModelBase, ITranslationScope
{
    private readonly IReadOnlyDictionary<string, string> _english;
    private readonly Dictionary<string, IReadOnlyDictionary<string, string>> _languages = new(StringComparer.OrdinalIgnoreCase);
    private readonly ObservableCollection<string> _errors = [];
    private readonly HashSet<string> _missingKeys = new(StringComparer.OrdinalIgnoreCase);
    public string Id { get; }
    public string CurrentLanguageCode { get; private set; }
    public ReadOnlyObservableCollection<string> LoadErrors { get; }
    public event EventHandler? LanguageChanged;
    public string this[string key] => Translate(key);

    internal TranslationCatalog(string id, IReadOnlyDictionary<string, string> english, string languageCode)
    {
        Id = id;
        _english = Copy(english);
        CurrentLanguageCode = languageCode;
        LoadErrors = new(_errors);
    }

    public string Translate(string? key)
    {
        if (string.IsNullOrWhiteSpace(key)) return string.Empty;
        if (_languages.TryGetValue(CurrentLanguageCode, out var catalog) && catalog.TryGetValue(key, out var value)) return value;
        if (_english.TryGetValue(key, out var fallback)) return fallback;
        if (_missingKeys.Add(key)) _errors.Add($"{Id}: Missing translation '{key}'.");
        return key;
    }

    public string Format(string key, params object?[] arguments)
        => string.Format(CultureInfo.CurrentCulture, Translate(key), arguments);

    public void AddLanguage(string code, IReadOnlyDictionary<string, string> translations)
    {
        _languages[TranslationService.NormalizeLanguageCode(code)] = Copy(translations);
        Refresh();
    }

    public void LoadLanguage(Stream stream)
    {
        using var json = JsonDocument.Parse(stream);
        var code = json.RootElement.GetProperty("header").GetProperty("code").GetString()
            ?? throw new JsonException("Missing language code.");
        var translations = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var entry in json.RootElement.GetProperty("translations").EnumerateObject())
        {
            if (entry.Value.ValueKind == JsonValueKind.Null) continue;
            if (!translations.TryAdd(entry.Name, entry.Value.GetString() ?? string.Empty))
                throw new JsonException($"Duplicate translation key: {entry.Name}");
        }
        AddLanguage(code, translations);
    }

    /// <summary>Bad files are reported individually; a missing directory retains English fallback.</summary>
    public void LoadDirectory(string path)
    {
        if (!Directory.Exists(path)) return;
        try
        {
            foreach (var file in Directory.EnumerateFiles(path, "*.json").Order(StringComparer.OrdinalIgnoreCase))
            {
                try { using var stream = File.OpenRead(file); LoadLanguage(stream); }
                catch (Exception exception) when (exception is IOException or UnauthorizedAccessException
                    or JsonException or InvalidOperationException or KeyNotFoundException or ArgumentException)
                { _errors.Add($"{Id}/{Path.GetFileName(file)}: {exception.Message}"); }
            }
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        { _errors.Add($"{Id}: {exception.Message}"); }
    }

    internal void SelectLanguage(string code)
    {
        if (CurrentLanguageCode == code) return;
        CurrentLanguageCode = code;
        Refresh();
    }

    private void Refresh()
    {
        NotifyPropertyChanged(nameof(CurrentLanguageCode));
        NotifyPropertyChanged("Item[]");
        NotifyPropertyChanged("Item");
        LanguageChanged?.Invoke(this, EventArgs.Empty);
    }

    private static IReadOnlyDictionary<string, string> Copy(IReadOnlyDictionary<string, string> values)
    {
        var copy = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var pair in values)
        {
            if (!string.IsNullOrWhiteSpace(pair.Key) && !string.IsNullOrWhiteSpace(pair.Value))
                copy.Add(pair.Key.Trim(), pair.Value);
        }
        return new ReadOnlyDictionary<string, string>(copy);
    }
}
