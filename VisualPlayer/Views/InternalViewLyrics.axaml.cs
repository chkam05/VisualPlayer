using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Lucide.Avalonia;
using VisualPlayer.Contexts;
using VisualPlayer.ViewModels;
using VisualPlayer.ViewModels.Lyrics;
using VisualPlayer.Views.Base;
using VisualPlayer.Views.Lyrics;

namespace VisualPlayer.Views;

public partial class InternalViewLyrics : InternalViewBase
{
    private static TranslationContext T => ApplicationContext.Instance.Translations;

    private readonly LyricsDownloaderView _downloaderView = new();
    private readonly LyricsListView _listView = new();

    private string _createOrEditButtonTitle = T["VIEW_LYRICS_BUTTON_CREATE"];
    private bool _hasCurrentTrack;

    #region AVALONIA PROPERTIES

    public static readonly DirectProperty<InternalViewLyrics, string> CreateOrEditButtonTitleProperty
        = AvaloniaProperty.RegisterDirect<InternalViewLyrics, string>(nameof(CreateOrEditButtonTitle), view => view.CreateOrEditButtonTitle);

    public static readonly DirectProperty<InternalViewLyrics, bool> HasCurrentTrackProperty
        = AvaloniaProperty.RegisterDirect<InternalViewLyrics, bool>(nameof(HasCurrentTrack), view => view.HasCurrentTrack);

    #endregion AVALONIA PROPERTIES

    #region EVENTS

    public event System.Action<LyricsDocument>? EditorRequested;

    #endregion EVENTS

    #region PROPERTIES

    public string CreateOrEditButtonTitle
    {
        get => _createOrEditButtonTitle;
        private set => SetAndRaise(CreateOrEditButtonTitleProperty, ref _createOrEditButtonTitle, value);
    }

    public bool HasCurrentTrack
    {
        get => _hasCurrentTrack;
        private set => SetAndRaise(HasCurrentTrackProperty, ref _hasCurrentTrack, value);
    }

    public ObservableCollection<MenuItemViewModel> NavigationItems { get; }

    #endregion PROPERTIES

    #region CONSTRUCTORS

    public InternalViewLyrics()
    {
        Title = T["MENU_LYRICS"];
        NavigationItems =
        [
            new MenuItemViewModel(LucideIconKind.ListMusic, T["VIEW_LYRICS_NAV_LIST"], ShowList),
            new MenuItemViewModel(LucideIconKind.CloudDownload, T["VIEW_LYRICS_NAV_ONLINE"], ShowDownloader)
        ];

        InitializeComponent();

        _listView.EditRequested += OpenEditor;
        ApplicationContext.Instance.LyricsCore.PropertyChanged += LyricsCore_PropertyChanged;
        ApplicationContext.Instance.PlayerCore.CurrentTrackChanged += PlayerCore_CurrentTrackChanged;
        T.LanguageChanged += Translations_LanguageChanged;

        UpdateCurrentTrackState();
        PART_NavigationList.SelectedIndex = 0;
    }

    #endregion CONSTRUCTORS

    #region ACTIONS

    private void OpenEditorButton_Click(object? sender, RoutedEventArgs e)
    {
        if (!HasCurrentTrack)
            return;

        var lyricsCore = ApplicationContext.Instance.LyricsCore;
        var document = lyricsCore.CurrentDocument
            ?? lyricsCore.CreateForCurrentTrack();

        OpenEditor(document);
    }

    private async void ImportLyricsButton_Click(object? sender, RoutedEventArgs e)
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
                    new FilePickerFileType(T["FILETYPE_LYRICS_FILES"])
                    {
                        Patterns = ["*.lrc", "*.txt"]
                    }
                ],
                Title = T["VIEW_LYRICS_IMPORT_TITLE"]
            });

        var file = files.FirstOrDefault();

        if (file is null)
            return;

        await using var stream = await file.OpenReadAsync();
        using var reader = new StreamReader(stream);
        var content = await reader.ReadToEndAsync();
        var fileName = Path.GetFileNameWithoutExtension(file.Name);

        ApplicationContext.Instance.LyricsCore.ImportText(content, fileName);
        ShowList();
        PART_NavigationList.SelectedIndex = 0;
    }

    private void NavigationList_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (PART_NavigationList.SelectedItem is MenuItemViewModel menuItem
            && menuItem.Action.CanExecute(null))
        {
            menuItem.Action.Execute(null);
        }
    }

    private void ShowDownloader()
    {
        PART_ContentHost.Content = _downloaderView;
    }

    private void OpenEditor(LyricsDocument document)
    {
        EditorRequested?.Invoke(document);
    }

    private void ShowList()
    {
        PART_ContentHost.Content = _listView;
    }

    private void UpdateCurrentTrackState()
    {
        var context = ApplicationContext.Instance;
        HasCurrentTrack = context.PlayerCore.CurrentTrack is not null;
        CreateOrEditButtonTitle = context.LyricsCore.HasCurrentTrackLyrics
            ? T["VIEW_LYRICS_BUTTON_EDIT"]
            : T["VIEW_LYRICS_BUTTON_CREATE"];
    }

    #endregion ACTIONS

    #region PROPERTY CHANGES

    private void LyricsCore_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Core.LyricsCore.CurrentDocument)
            || e.PropertyName == nameof(Core.LyricsCore.HasCurrentTrackLyrics))
        {
            UpdateCurrentTrackState();
        }
    }

    private void PlayerCore_CurrentTrackChanged(object? sender, System.EventArgs e)
    {
        UpdateCurrentTrackState();
    }

    #endregion PROPERTY CHANGES

    #region VIEW ACTIONS

    protected override void OnClosed()
    {
        _listView.EditRequested -= OpenEditor;
        ApplicationContext.Instance.LyricsCore.PropertyChanged -= LyricsCore_PropertyChanged;
        ApplicationContext.Instance.PlayerCore.CurrentTrackChanged -= PlayerCore_CurrentTrackChanged;
        T.LanguageChanged -= Translations_LanguageChanged;
        base.OnClosed();
    }

    private void Translations_LanguageChanged(object? sender, EventArgs e)
    {
        Title = T["MENU_LYRICS"];
        NavigationItems[0].Title = T["VIEW_LYRICS_NAV_LIST"];
        NavigationItems[1].Title = T["VIEW_LYRICS_NAV_ONLINE"];
        UpdateCurrentTrackState();
    }

    #endregion VIEW ACTIONS
}
