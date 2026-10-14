using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using VisualPlayer.Contexts;
using VisualPlayer.Enums;
using VisualPlayer.ViewModels.Config;

namespace VisualPlayer.Views.Settings;

public partial class LyricsSettingsView : UserControl
{
    public IReadOnlyList<LyricsType> LyricsTypes { get; } = Enum.GetValues<LyricsType>();

    #region CONSTRUCTORS

    public LyricsSettingsView()
    {
        InitializeComponent();
    }

    #endregion CONSTRUCTORS

    #region ACTIONS

    private void ResetButton_Click(object? sender, RoutedEventArgs e)
    {
        var defaults = new LyricsConfig();
        var config = ApplicationContext.Instance.Config.Lyrics;

        config.AutoDownloadLyrics = defaults.AutoDownloadLyrics;
        config.AutoImportEmbeddedLyrics = defaults.AutoImportEmbeddedLyrics;
        config.BackgroundOpacity = defaults.BackgroundOpacity;
        config.CurrentLineFontSize = defaults.CurrentLineFontSize;
        config.DisplayLyrics = defaults.DisplayLyrics;
        config.PreferredType = defaults.PreferredType;
        config.PlainTextFontSize = defaults.PlainTextFontSize;
        config.SeekLongInterval = defaults.SeekLongInterval;
        config.SeekMediumInterval = defaults.SeekMediumInterval;
        config.SeekShortInterval = defaults.SeekShortInterval;
        config.SurroundingLineFontSize = defaults.SurroundingLineFontSize;
        config.UseEmbeddedLyrics = defaults.UseEmbeddedLyrics;
    }

    #endregion ACTIONS
}
