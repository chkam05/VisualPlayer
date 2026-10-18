using System.Globalization;

namespace VisualPlayer.VisualizationBase.Localization;

/// <summary>One language selection, independent application/UI/plugin catalogs; no global singleton.</summary>
public sealed class TranslationService
{
    private readonly Dictionary<string, TranslationCatalog> _catalogs = new(StringComparer.OrdinalIgnoreCase);
    public string CurrentLanguageCode { get; private set; }
    public TranslationService(string languageCode = "en-US") => CurrentLanguageCode = NormalizeLanguageCode(languageCode);

    public TranslationCatalog RegisterScope(string id, IReadOnlyDictionary<string, string> english, string? directory = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(english);
        var catalog = new TranslationCatalog(id, english, CurrentLanguageCode);
        if (!_catalogs.TryAdd(id, catalog)) throw new ArgumentException($"Catalog already registered: {id}", nameof(id));
        if (directory is not null) catalog.LoadDirectory(directory);
        return catalog;
    }

    public void SelectLanguage(string code)
    {
        CurrentLanguageCode = NormalizeLanguageCode(code);
        foreach (var scope in _catalogs.Values) scope.SelectLanguage(CurrentLanguageCode);
    }

    /// <summary>Remove a scope owned by a failed or closed plugin host; existing references remain valid.</summary>
    public bool UnregisterScope(string id) => _catalogs.Remove(id);

    internal static string NormalizeLanguageCode(string code)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        var name = CultureInfo.GetCultureInfo(code.Trim()).Name;
        return name.Length > 0 ? name : throw new ArgumentException("An explicit language is required.", nameof(code));
    }
}
