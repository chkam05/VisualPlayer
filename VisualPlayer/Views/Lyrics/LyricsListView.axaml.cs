using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;
using Lucide.Avalonia;
using VisualPlayer.Contexts;
using VisualPlayer.Enums;
using VisualPlayer.Enums.UI;
using VisualPlayer.Utils;
using VisualPlayer.Utils.Lyrics;
using VisualPlayer.ViewModels.Lyrics;
using VisualPlayer.Views.Dialogs;
using VisualPlayer.Windows;

namespace VisualPlayer.Views.Lyrics;

public partial class LyricsListView : UserControl
{
    private string _searchPhrase = string.Empty;
    private LyricsDocumentSortMode _sortMode = LyricsDocumentSortMode.NameAscending;
    private bool _isLibrarySubscribed;

    #region AVALONIA PROPERTIES

    public static readonly DirectProperty<LyricsListView, string> SearchPhraseProperty
        = AvaloniaProperty.RegisterDirect<LyricsListView, string>(
            nameof(SearchPhrase),
            view => view.SearchPhrase,
            (view, value) => view.SearchPhrase = value);

    #endregion AVALONIA PROPERTIES

    public event Action<LyricsDocument>? EditRequested;

    #region PROPERTIES

    public string SearchPhrase
    {
        get => _searchPhrase;
        set
        {
            SetAndRaise(SearchPhraseProperty, ref _searchPhrase, value ?? string.Empty);
            RefreshDocuments();
        }
    }

    public ObservableCollection<LyricsDocument> VisibleDocuments { get; } = [];

    #endregion PROPERTIES

    #region CONSTRUCTORS

    public LyricsListView()
    {
        InitializeComponent();
        SubscribeLibrary();
        RefreshDocuments();
    }

    #endregion CONSTRUCTORS

    private void AssignMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        if (GetDocument(sender) is { } document)
            ApplicationContext.Instance.LyricsCore.AssignToCurrentTrack(document);
    }

    private async void DeleteMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        if (GetDocument(sender) is not { } document
            || TopLevel.GetTopLevel(this) is not MainWindow window)
        {
            return;
        }

        var dialog = new InternalMessageDialog(
            "Usuń tekst",
            $"Czy na pewno chcesz usunąć tekst „{document.SuggestedName}”? Tej operacji nie można cofnąć.",
            InternalDialogButtons.YesNo,
            LucideIconKind.Trash);

        if (await window.ShowDialogAsync(dialog) == InternalDialogResult.Yes)
            ApplicationContext.Instance.LyricsCore.Delete(document);
    }

    private void EditMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        if (GetDocument(sender) is { } document)
            EditRequested?.Invoke(document);
    }

    private async void ExportMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        if (GetDocument(sender) is not { } document
            || TopLevel.GetTopLevel(this) is not { } topLevel)
        {
            return;
        }

        var isPlainText = document.Type == LyricsType.Plain;
        var extension = isPlainText ? "txt" : "lrc";
        var file = await topLevel.StorageProvider.SaveFilePickerAsync(
            new FilePickerSaveOptions
            {
                DefaultExtension = extension,
                FileTypeChoices =
                [
                    new FilePickerFileType(isPlainText ? "Zwykły tekst" : "Tekst synchronizowany LRC")
                    {
                        Patterns = [$"*.{extension}"]
                    }
                ],
                ShowOverwritePrompt = true,
                SuggestedFileName = $"{document.SuggestedName}.{extension}",
                Title = "Eksportuj tekst piosenki"
            });

        if (file is null)
            return;

        await using var stream = await file.OpenWriteAsync();
        stream.SetLength(0);
        await using var writer = new StreamWriter(stream);
        await writer.WriteAsync(LrcExporter.Export(document));
    }

    private void OptionsButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button)
            button.ContextMenu?.Open(button);
    }

    private void SortButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button)
            button.ContextMenu?.Open(button);
    }

    private void SortMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem { Tag: string sortMode }
            || !Enum.TryParse<LyricsDocumentSortMode>(sortMode, out var parsedSortMode))
        {
            return;
        }

        _sortMode = parsedSortMode;
        RefreshDocuments();
    }

    private async void RevealMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        if (GetDocument(sender) is not { } document)
            return;

        var filePath = ApplicationContext.Instance.LyricsCore.GetStoragePath(document);
        await FileManagerUtils.RevealFileAsync(filePath);
    }

    private static LyricsDocument? GetDocument(object? sender)
    {
        return sender is MenuItem { DataContext: LyricsDocument document }
            ? document
            : null;
    }

    #region LIBRARY

    private void Documents_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        RefreshDocuments();
    }

    private void RefreshDocuments()
    {
        IEnumerable<LyricsDocument> documents = ApplicationContext.Instance.LyricsCore.Documents;
        var phrase = SearchPhrase.Trim();

        if (!string.IsNullOrWhiteSpace(phrase))
        {
            documents = documents.Where(document =>
                document.SuggestedName.Contains(phrase, StringComparison.CurrentCultureIgnoreCase)
                || (document.Artist?.Contains(phrase, StringComparison.CurrentCultureIgnoreCase) ?? false)
                || (document.Album?.Contains(phrase, StringComparison.CurrentCultureIgnoreCase) ?? false));
        }

        documents = _sortMode switch
        {
            LyricsDocumentSortMode.NameDescending => documents.OrderByDescending(
                document => document.SuggestedName,
                StringComparer.CurrentCultureIgnoreCase),
            LyricsDocumentSortMode.TypeAscending => documents
                .OrderBy(document => document.Type)
                .ThenBy(document => document.SuggestedName, StringComparer.CurrentCultureIgnoreCase),
            LyricsDocumentSortMode.TypeDescending => documents
                .OrderByDescending(document => document.Type)
                .ThenBy(document => document.SuggestedName, StringComparer.CurrentCultureIgnoreCase),
            _ => documents.OrderBy(
                document => document.SuggestedName,
                StringComparer.CurrentCultureIgnoreCase)
        };

        VisibleDocuments.Clear();

        foreach (var document in documents)
            VisibleDocuments.Add(document);
    }

    private void SubscribeLibrary()
    {
        if (_isLibrarySubscribed)
            return;

        ApplicationContext.Instance.LyricsCore.Documents.CollectionChanged += Documents_CollectionChanged;
        _isLibrarySubscribed = true;
    }

    private void UnsubscribeLibrary()
    {
        if (!_isLibrarySubscribed)
            return;

        ApplicationContext.Instance.LyricsCore.Documents.CollectionChanged -= Documents_CollectionChanged;
        _isLibrarySubscribed = false;
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        SubscribeLibrary();
        RefreshDocuments();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        UnsubscribeLibrary();
        base.OnDetachedFromVisualTree(e);
    }

    #endregion LIBRARY
}
