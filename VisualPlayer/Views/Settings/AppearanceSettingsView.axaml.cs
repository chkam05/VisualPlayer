using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using VisualPlayer.Contexts;
using VisualPlayer.Enums.Config;
using VisualPlayer.Events;
using VisualPlayer.ViewModels.Config;

namespace VisualPlayer.Views.Settings;

public partial class AppearanceSettingsView : UserControl
{
    private static TranslationContext T => ApplicationContext.Instance.Translations;

    public IReadOnlyList<ColorizationLevel> ColorizationLevels { get; } = Enum.GetValues<ColorizationLevel>();
    public IReadOnlyList<InfoBarDisplayMode> InfoBarDisplayModes { get; } = Enum.GetValues<InfoBarDisplayMode>();
    public IReadOnlyList<ApplicationTheme> Themes { get; } = Enum.GetValues<ApplicationTheme>();
    public IReadOnlyList<ImageFillMode> ImageFillModes { get; } = Enum.GetValues<ImageFillMode>();
    public IReadOnlyList<WindowBackgroundMode> WindowBackgroundModes { get; } = Enum.GetValues<WindowBackgroundMode>();

    #region CONSTRUCTORS

    public AppearanceSettingsView()
    {
        InitializeComponent();
    }

    #endregion CONSTRUCTORS

    #region ACTIONS

    private async void BackgroundImageSelector_AddImageRequested(object? sender, EventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);

        if (topLevel is null)
            return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(
            new FilePickerOpenOptions
            {
                AllowMultiple = false,
                FileTypeFilter =
                [
                    new FilePickerFileType(T["FILETYPE_IMAGE_FILES"])
                    {
                        Patterns = ["*.png", "*.jpg", "*.jpeg", "*.bmp", "*.webp"]
                    }
                ],
                Title = T["VIEW_SETTINGS_APPEARANCE_PICK_BACKGROUND_TITLE"]
            });

        var imagePath = files
            .Select(file => file.TryGetLocalPath())
            .FirstOrDefault(path => !string.IsNullOrWhiteSpace(path));

        if (imagePath is null)
            return;

        var storedImagePath = await CopyBackgroundImageAsync(imagePath);
        BackgroundImageSelector.AddImage(storedImagePath);
    }

    private void BackgroundImageSelector_ImageRemoved(object? sender, BackgroundImageRemovedEventArgs e)
    {
        if (!IsStoredBackgroundPath(e.ImagePath))
            return;

        try
        {
            if (File.Exists(e.ImagePath))
                File.Delete(e.ImagePath);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            Debug.WriteLine($"Could not remove background image: {exception.Message}");
        }
    }

    private void ResetButton_Click(object? sender, RoutedEventArgs e)
    {
        ApplicationContext.Instance.Config.Appearance = new AppearanceConfig();
    }

    #endregion ACTIONS

    #region BACKGROUND IMAGES

    private static async Task<string> CopyBackgroundImageAsync(string sourcePath)
    {
        var sourceFullPath = Path.GetFullPath(sourcePath);

        if (IsStoredBackgroundPath(sourceFullPath))
            return sourceFullPath;

        Directory.CreateDirectory(ConfigurationContext.BackgroundsDirectoryPath);

        var destinationPath = GetAvailableBackgroundPath(Path.GetFileName(sourceFullPath));

        await using var sourceStream = File.OpenRead(sourceFullPath);
        await using var destinationStream = File.Create(destinationPath);
        await sourceStream.CopyToAsync(destinationStream);

        return destinationPath;
    }

    private static string GetAvailableBackgroundPath(string fileName)
    {
        var destinationPath = Path.Combine(ConfigurationContext.BackgroundsDirectoryPath, fileName);

        if (!File.Exists(destinationPath))
            return destinationPath;

        var name = Path.GetFileNameWithoutExtension(fileName);
        var extension = Path.GetExtension(fileName);
        var index = 1;

        do
        {
            destinationPath = Path.Combine(
                ConfigurationContext.BackgroundsDirectoryPath,
                $"{name} ({index++}){extension}");
        }
        while (File.Exists(destinationPath));

        return destinationPath;
    }

    private static bool IsStoredBackgroundPath(string imagePath)
    {
        var backgroundsPath = Path.GetFullPath(ConfigurationContext.BackgroundsDirectoryPath)
            .TrimEnd(Path.DirectorySeparatorChar)
            + Path.DirectorySeparatorChar;

        var imageFullPath = Path.GetFullPath(imagePath);

        return imageFullPath.StartsWith(backgroundsPath, StringComparison.OrdinalIgnoreCase);
    }

    #endregion BACKGROUND IMAGES
}
