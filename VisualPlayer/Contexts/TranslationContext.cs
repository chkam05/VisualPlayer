using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using VisualPlayer.Attributes;
using VisualPlayer.Models.Localization;
using VisualPlayer.Resources;
using VisualPlayer.ViewModels;
using VisualPlayer.ViewModels.Config;
using VisualPlayer.VisualizationBase.Localization;

namespace VisualPlayer.Contexts;

public sealed class TranslationContext : ViewModelBase, ITranslationScope
{
    public event EventHandler? LanguageChanged;

    #region CONST

    public const string DEFAULT_LANGUAGE_CODE = GeneralConfig.DEFAULT_LANGUAGE_CODE;
    public const string DEFAULT_LANGUAGE_NAME = "English";
    public const string LANG_DIRECTORY_NAME = "Lang";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    #endregion CONST

    private readonly ObservableCollection<LanguageDefinition> _availableLanguages = [];
    private readonly GeneralConfig _config;
    private readonly ObservableCollection<string> _loadErrors = [];
    private readonly ReadOnlyObservableCollection<LanguageDefinition> _readOnlyAvailableLanguages;
    private readonly ReadOnlyObservableCollection<string> _readOnlyLoadErrors;
    private LanguageDefinition _currentLanguage = null!;

    #region PROPERTIES

    [NotifyPropertyUpdate]
    public LanguageDefinition CurrentLanguage
    {
        get => _currentLanguage;
        private set => UpdateProperty(ref _currentLanguage, value);
    }

    public ReadOnlyObservableCollection<LanguageDefinition> AvailableLanguages => _readOnlyAvailableLanguages;
    
    public string CurrentLanguageCode => CurrentLanguage.Code;
    
    public ReadOnlyObservableCollection<string> LoadErrors => _readOnlyLoadErrors;

    public LanguageDefinition SelectedLanguage
    {
        get => CurrentLanguage;
        set => SelectLanguage(value);
    }

    public string this[string key] => Translate(key);

    public static string LangDirectoryPath
    {
        get
        {
            var defaultPath = Path.Combine(AppContext.BaseDirectory, LANG_DIRECTORY_NAME);
            if (!OperatingSystem.IsMacOS())
                return defaultPath;

            var bundleResourcesPath = Path.GetFullPath(Path.Combine(
                AppContext.BaseDirectory,
                "..",
                "Resources",
                LANG_DIRECTORY_NAME));

            return Directory.Exists(bundleResourcesPath) ? bundleResourcesPath : defaultPath;
        }
    }

    #endregion PROPERTIES

    #region CONSTRUCTORS

    public TranslationContext(GeneralConfig config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _readOnlyAvailableLanguages = new ReadOnlyObservableCollection<LanguageDefinition>(
            _availableLanguages);
        _readOnlyLoadErrors = new ReadOnlyObservableCollection<string>(_loadErrors);

        ReloadLanguages();
    }

    #endregion CONSTRUCTORS

    public void ReloadLanguages()
    {
        var selectedCode = _config.LanguageCode;

        _availableLanguages.Clear();
        _loadErrors.Clear();
        _availableLanguages.Add(CreateDefaultLanguage());

        foreach (var language in LoadExternalLanguages())
            _availableLanguages.Add(language);

        var selectedLanguage = FindLanguage(selectedCode)
            ?? _availableLanguages[0];

        SetCurrentLanguage(selectedLanguage);
        NotifyPropertyChanged(nameof(AvailableLanguages));
        NotifyPropertyChanged(nameof(LoadErrors));
    }

    public bool SelectLanguage(string? code)
    {
        var language = FindLanguage(code);

        if (language is null)
            return false;

        SetCurrentLanguage(language);
        return true;
    }

    public bool SelectLanguage(LanguageDefinition? language)
    {
        return SelectLanguage(language?.Code);
    }

    public string Translate(string? key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return string.Empty;

        if (CurrentLanguage.Translations.TryGetValue(key, out var translatedValue))
            return translatedValue;

        if (EnglishTranslations.Values.TryGetValue(key, out var fallbackValue))
            return fallbackValue;

        return key;
    }

    public string Format(string key, params object?[] arguments)
    {
        return string.Format(CultureInfo.CurrentCulture, Translate(key), arguments);
    }


    private static LanguageDefinition CreateDefaultLanguage()
    {
        return new LanguageDefinition(
            DEFAULT_LANGUAGE_CODE,
            DEFAULT_LANGUAGE_NAME,
            EnglishTranslations.Values,
            isBuiltIn: true);
    }

    private LanguageDefinition? FindLanguage(string? code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return null;

        return _availableLanguages.FirstOrDefault(language => string.Equals(
            language.Code,
            code.Trim(),
            StringComparison.OrdinalIgnoreCase));
    }

    private IEnumerable<LanguageDefinition> LoadExternalLanguages()
    {
        if (!Directory.Exists(LangDirectoryPath))
            yield break;

        string[] filePaths;

        try
        {
            filePaths = Directory.GetFiles(
                LangDirectoryPath,
                "*.json",
                SearchOption.TopDirectoryOnly);
        }
        catch (Exception exception) when (exception is IOException
            or UnauthorizedAccessException)
        {
            _loadErrors.Add($"Cannot scan language directory: {exception.Message}");
            yield break;
        }

        var loadedCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            DEFAULT_LANGUAGE_CODE
        };

        foreach (var filePath in filePaths.OrderBy(path => path, StringComparer.OrdinalIgnoreCase))
        {
            LanguageDefinition? language;

            try
            {
                language = LoadLanguageFile(filePath);
            }
            catch (Exception exception) when (exception is IOException
                or UnauthorizedAccessException
                or JsonException)
            {
                _loadErrors.Add($"{Path.GetFileName(filePath)}: {exception.Message}");
                continue;
            }

            if (language is null)
            {
                _loadErrors.Add($"{Path.GetFileName(filePath)}: Invalid language file.");
                continue;
            }

            if (!loadedCodes.Add(language.Code))
            {
                _loadErrors.Add(
                    $"{Path.GetFileName(filePath)}: Duplicate language code '{language.Code}'.");
                continue;
            }

            yield return language;
        }
    }

    private static LanguageDefinition? LoadLanguageFile(string filePath)
    {
        var json = File.ReadAllText(filePath);
        var languageFile = JsonSerializer.Deserialize<LanguageFile>(json, JsonOptions);
        var code = NormalizeLanguageCode(languageFile?.Header?.Code);
        var name = languageFile?.Header?.Name?.Trim();

        if (code is null || string.IsNullOrWhiteSpace(name))
            return null;

        var translations = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var entry in languageFile?.Translations ?? [])
        {
            var key = entry.Key.Trim();

            if (key.Length == 0 || entry.Value is null)
                continue;

            translations[key] = entry.Value;
        }

        return new LanguageDefinition(code, name, translations);
    }

    private static string? NormalizeLanguageCode(string? code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return null;

        try
        {
            var culture = CultureInfo.GetCultureInfo(code.Trim());
            return string.IsNullOrWhiteSpace(culture.Name)
                ? null
                : culture.Name;
        }
        catch (CultureNotFoundException)
        {
            return null;
        }
    }

    private void SetCurrentLanguage(LanguageDefinition language)
    {
        var languageChanged = !string.Equals(
            CurrentLanguage?.Code,
            language.Code,
            StringComparison.OrdinalIgnoreCase);

        CurrentLanguage = language;
        _config.LanguageCode = language.Code;

        if (!languageChanged)
            return;

        NotifyPropertyChanged(nameof(CurrentLanguageCode));
        NotifyPropertyChanged(nameof(SelectedLanguage));
        NotifyPropertyChanged("Item[]");
        NotifyPropertyChanged("Item");
        LanguageChanged?.Invoke(this, EventArgs.Empty);
    }
}
