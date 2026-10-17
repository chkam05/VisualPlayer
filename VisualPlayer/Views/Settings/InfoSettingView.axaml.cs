using System.Collections.Generic;
using System.Diagnostics;
using Avalonia.Interactivity;
using Avalonia.Controls;
using VisualPlayer.Models.App;
using VisualPlayer.Utils;

namespace VisualPlayer.Views.Settings;

public partial class InfoSettingView : UserControl
{
    public string AppName => AppInfoGetter.AppName;
    public string AppVersion => AppInfoGetter.AppVersion;
    public string Copyright => $"Copyright (c) {AppInfoGetter.AppAuthor}";
    public IReadOnlyList<NuGetPackageInfo> NuGetPackages => AppInfoGetter.NuGetPackages;
    public IReadOnlyList<FontInfo> Fonts { get; } =
    [
        new("Oxanium", "The Oxanium Project Authors", "https://github.com/sevmeyer/oxanium"),
        new("LED Sled", "Daniel Zadorozny / Iconian Fonts", "https://www.dafont.com/led-sled.font"),
        new("LED Counter 7", "Sizenko Alexander / Style-7", "https://www.dafont.com/led-counter-7.font")
    ];

    #region CONSTRUCTORS

    public InfoSettingView()
    {
        InitializeComponent();
        DataContext = this;
    }

    private void OpenFontWebsite_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: string website })
            return;

        try
        {
            Process.Start(new ProcessStartInfo(website) { UseShellExecute = true });
        }
        catch
        {
            // Opening an external website may be unavailable on a restricted system.
        }
    }

    #endregion CONSTRUCTORS
}
