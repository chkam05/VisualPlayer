using System.Collections.Generic;
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

    #region CONSTRUCTORS

    public InfoSettingView()
    {
        InitializeComponent();
        DataContext = this;
    }

    #endregion CONSTRUCTORS
}
