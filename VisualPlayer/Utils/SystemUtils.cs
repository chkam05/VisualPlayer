using System;
using Avalonia;
using Avalonia.Platform;
using VisualPlayer.Enums.Config;

namespace VisualPlayer.Utils;

public static class SystemUtils
{
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
                return "Pokaż w Finderze";

            if (IsWindows)
                return "Pokaż w Eksploratorze plików";

            if (IsLinux)
                return "Pokaż w menedżerze plików";

            return "Pokaż położenie pliku";
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
