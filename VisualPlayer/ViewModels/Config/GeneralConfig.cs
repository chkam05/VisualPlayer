using System;
using VisualPlayer.Attributes;
using VisualPlayer.Enums.Config;

namespace VisualPlayer.ViewModels.Config;

public class GeneralConfig : ViewModelBase
{
    #region CONST

    public const string DEFAULT_LANGUAGE_CODE = "en-US";

    #endregion CONST

    private string _languageCode = DEFAULT_LANGUAGE_CODE;
    private bool _clickToHideControls = true;
    private bool _doubleClickWindowAction = true;
    private WindowDoubleClickMode _windowDoubleClickMode = WindowDoubleClickMode.FullScreen;


    [NotifyPropertyUpdate]
    public bool ClickToHideControls
    {
        get => _clickToHideControls;
        set => UpdateProperty(ref _clickToHideControls, value);
    }

    [NotifyPropertyUpdate]
    public bool DoubleClickWindowAction
    {
        get => _doubleClickWindowAction;
        set => UpdateProperty(ref _doubleClickWindowAction, value);
    }

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

    [NotifyPropertyUpdate]
    public WindowDoubleClickMode WindowDoubleClickMode
    {
        get => _windowDoubleClickMode;
        set => UpdateProperty(
            ref _windowDoubleClickMode,
            Enum.IsDefined(value) ? value : WindowDoubleClickMode.FullScreen);
    }
}
