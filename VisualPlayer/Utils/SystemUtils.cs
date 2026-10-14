using System;
using Avalonia;
using Avalonia.Platform;
using VisualPlayer.Enums.Config;
using VisualPlayer.Contexts;

namespace VisualPlayer.Utils;

public static class SystemUtils
{
    private static TranslationContext T => ApplicationContext.Instance.Translations;

    #region PROPERTIES

    public static bool IsWindows
    {
        get => OperatingSystem.IsWindows();
    }

    public static bool IsMacOS
    {
        get => OperatingSystem.IsMacOS();
    }

    public static bool IsLinux
    {
        get => OperatingSystem.IsLinux();
    }

    public static string RevealInFileManagerTitle
    {
        get
        {
            if (IsMacOS)
                return T["SYSTEM_SHOW_IN_FINDER"];

            if (IsWindows)
                return T["SYSTEM_SHOW_IN_EXPLORER"];

            if (IsLinux)
                return T["SYSTEM_SHOW_IN_FILE_MANAGER"];

            return T["SYSTEM_SHOW_FILE_LOCATION"];
        }
    }

    #endregion PROPERTIES

    #region THEME

    public static ApplicationTheme GetSystemTheme()
    {
        var platformSettings = Application.Current?.PlatformSettings;

        if (platformSettings is null)
            return ApplicationTheme.Dark;

        return GetSystemTheme(platformSettings.GetColorValues());
    }

    public static ApplicationTheme GetSystemTheme(PlatformColorValues colorValues)
    {
        return colorValues.ThemeVariant switch
        {
            PlatformThemeVariant.Light => ApplicationTheme.Light,
            PlatformThemeVariant.Dark => ApplicationTheme.Dark,
            _ => ApplicationTheme.Dark
        };
    }

    #endregion THEME
}
