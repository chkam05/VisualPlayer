using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Lucide.Avalonia;
using VisualPlayer.Contexts;
using VisualPlayer.Enums.Config;
using VisualPlayer.ViewModels.Config;

namespace VisualPlayer.Views.Settings;

public partial class GeneralSettingsView : UserControl
{
    private static TranslationContext T => ApplicationContext.Instance.Translations;

    public IReadOnlyList<WindowDoubleClickMode> WindowDoubleClickModes { get; }
        = Enum.GetValues<WindowDoubleClickMode>();

    #region CONVERTERS

    public static IValueConverter FolderIconConverter { get; }
        = new FuncValueConverter<bool, LucideIconKind>(recursive => recursive
            ? LucideIconKind.Folders
            : LucideIconKind.Folder);

    #endregion CONVERTERS

    #region CONSTRUCTORS

    public GeneralSettingsView()
    {
        InitializeComponent();
    }

    #endregion CONSTRUCTORS

    #region BUTTON ACTIONS

    private async void AddFolderButton_Click(object? sender, RoutedEventArgs e)
    {
        if (TopLevel.GetTopLevel(this) is not { } topLevel)
            return;

        var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = T["VIEW_SETTINGS_GENERAL_ADD_LIBRARY_FOLDER_TITLE"],
            AllowMultiple = false
        });

        var path = folders.Count > 0 ? folders[0].TryGetLocalPath() : null;

        if (string.IsNullOrWhiteSpace(path))
            return;

        var config = ApplicationContext.Instance.Config.Library;

        if (System.Linq.Enumerable.Any(config.Folders, folder => string.Equals(
            folder.Path, path, StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        config.Folders.Add(new LibraryFolderConfig
        {
            Path = path,
            IncludeSubdirectories = true
        });
        
        ApplicationContext.Instance.Config.Save();
        await ApplicationContext.Instance.LibraryCore.RefreshAsync();
    }

    private static void FolderMoreButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { ContextMenu: { } menu } button)
            menu.Open(button);
    }

    private async void IncludeSubdirectoriesMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem { DataContext: LibraryFolderConfig folder })
            return;

        folder.IncludeSubdirectories = !folder.IncludeSubdirectories;
        ApplicationContext.Instance.Config.Save();
        await ApplicationContext.Instance.LibraryCore.RefreshFolderAsync(folder);
    }

    private async void RefreshFolderMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is MenuItem { DataContext: LibraryFolderConfig folder })
            await ApplicationContext.Instance.LibraryCore.RefreshFolderAsync(folder);
    }

    private async void RefreshLibraryButton_Click(object? sender, RoutedEventArgs e)
    {
        await ApplicationContext.Instance.LibraryCore.RefreshAsync();
    }

    private async void RemoveFolderMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem { DataContext: LibraryFolderConfig folder })
            return;

        ApplicationContext.Instance.Config.Library.Folders.Remove(folder);
        ApplicationContext.Instance.Config.Save();
        await ApplicationContext.Instance.LibraryCore.RefreshAsync();
    }

    #endregion BUTTON ACTIONS
}
