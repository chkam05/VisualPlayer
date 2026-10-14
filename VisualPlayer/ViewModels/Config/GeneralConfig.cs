using VisualPlayer.Attributes;

namespace VisualPlayer.ViewModels.Config;

public class GeneralConfig : ViewModelBase
{
    #region CONST

    public const string DEFAULT_LANGUAGE_CODE = "en-US";

    #endregion CONST

    private string _languageCode = DEFAULT_LANGUAGE_CODE;

    [NotifyPropertyUpdate]
    public string LanguageCode
    {
        get => _languageCode;
        set => UpdateProperty(
            ref _languageCode,
            string.IsNullOrWhiteSpace(value)
                ? DEFAULT_LANGUAGE_CODE
                : value.Trim());
    }
}
