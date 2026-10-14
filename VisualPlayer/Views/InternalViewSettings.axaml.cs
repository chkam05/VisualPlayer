using System;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Lucide.Avalonia;
using VisualPlayer.Contexts;
using VisualPlayer.ViewModels;
using VisualPlayer.Views.Base;
using VisualPlayer.Views.Settings;

namespace VisualPlayer.Views;

public partial class InternalViewSettings : InternalViewBase
{
    private static TranslationContext T => ApplicationContext.Instance.Translations;

    private const double COMPACT_NAVIGATION_THRESHOLD = 720;
    private const double COMPACT_NAVIGATION_WIDTH = 64;
    private const double NAVIGATION_WIDTH = 220;

    private readonly AppearanceSettingsView _appearanceSettingsView = new();
    private readonly GeneralSettingsView _generalSettingsView = new();
    private readonly InfoSettingView _infoSettingView = new();
    private readonly KeyboardSettingsView _keyboardSettingsView = new();
    private readonly LogoSettingsView _logoSettingsView = new();
    private readonly LyricsSettingsView _lyricsSettingsView = new();
    private readonly PlayerSettingsView _playerSettingsView = new();
    private readonly VisualizationsSettingsView _visualizationsSettingsView = new();
    
    #region AVALONIA PROPERTIES

    public static readonly StyledProperty<bool> IsNavigationCompactProperty
        = AvaloniaProperty.Register<InternalViewSettings, bool>(nameof(IsNavigationCompact));

    #endregion AVALONIA PROPERTIES

    #region PROPERTIES

    public bool IsNavigationCompact
    {
        get => GetValue(IsNavigationCompactProperty);
        private set => SetValue(IsNavigationCompactProperty, value);
    }

    public ObservableCollection<MenuItemViewModel> NavigationItems { get; }

    #endregion PROPERTIES

    public InternalViewSettings()
    {
        Title = T["MENU_SETTINGS"];
        EnableSaveButton = true;
        SaveRequested += InternalViewSettings_SaveRequested;
        NavigationItems =
        [
            new MenuItemViewModel(LucideIconKind.Settings, T["VIEW_SETTINGS_NAV_GENERAL"], ShowGeneralSettings),
            new MenuItemViewModel(LucideIconKind.CirclePlay, T["VIEW_SETTINGS_NAV_PLAYER"], ShowPlayerSettings),
            new MenuItemViewModel(LucideIconKind.Box, T["VIEW_SETTINGS_NAV_LOGO"], ShowLogoSettings),
            new MenuItemViewModel(LucideIconKind.Keyboard, T["VIEW_SETTINGS_NAV_KEYBOARD"], ShowKeyboardSettings),
            new MenuItemViewModel(LucideIconKind.Type, T["VIEW_SETTINGS_NAV_LYRICS"], ShowLyricsSettings),
            new MenuItemViewModel(LucideIconKind.AudioLines, T["MENU_VISUALIZATIONS"], ShowVisualizationsSettings),
            new MenuItemViewModel(LucideIconKind.Palette, T["VIEW_SETTINGS_NAV_APPEARANCE"], ShowAppearanceSettings),
            MenuItemViewModel.CreateSeparator(),
            new MenuItemViewModel(LucideIconKind.Info, T["VIEW_SETTINGS_NAV_INFO"], ShowInfoSettings)
        ];

        InitializeComponent();

        PART_NavigationList.SelectedIndex = 0;
        SizeChanged += InternalViewSettings_SizeChanged;
        T.LanguageChanged += Translations_LanguageChanged;
    }

    protected override void OnClosed()
    {
        T.LanguageChanged -= Translations_LanguageChanged;
        SaveRequested -= InternalViewSettings_SaveRequested;
        SizeChanged -= InternalViewSettings_SizeChanged;
        base.OnClosed();
    }

    private void Translations_LanguageChanged(object? sender, EventArgs e)
    {
        Title = T["MENU_SETTINGS"];
        NavigationItems[0].Title = T["VIEW_SETTINGS_NAV_GENERAL"];
        NavigationItems[1].Title = T["VIEW_SETTINGS_NAV_PLAYER"];
        NavigationItems[2].Title = T["VIEW_SETTINGS_NAV_LOGO"];
        NavigationItems[3].Title = T["VIEW_SETTINGS_NAV_KEYBOARD"];
        NavigationItems[4].Title = T["VIEW_SETTINGS_NAV_LYRICS"];
        NavigationItems[5].Title = T["MENU_VISUALIZATIONS"];
        NavigationItems[6].Title = T["VIEW_SETTINGS_NAV_APPEARANCE"];
        NavigationItems[8].Title = T["VIEW_SETTINGS_NAV_INFO"];
    }

    private void InternalViewSettings_SaveRequested(object? sender, EventArgs e)
    {
        ApplicationContext.Instance.Config.Save();
        ApplicationContext.Instance.VisualizationConfig.Save();
    }

    private void InternalViewSettings_SizeChanged(object? sender, SizeChangedEventArgs e)
    {
        var isCompact = e.NewSize.Width < COMPACT_NAVIGATION_THRESHOLD;

        IsNavigationCompact = isCompact;
        PART_NavigationList.Width = isCompact
            ? COMPACT_NAVIGATION_WIDTH
            : NAVIGATION_WIDTH;
    }

    private void NavigationList_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (PART_NavigationList.SelectedItem is not MenuItemViewModel menuItem)
            return;

        if (menuItem.IsSeparator)
            return;

        if (menuItem.Action.CanExecute(null))
            menuItem.Action.Execute(null);
    }

    private void ShowAppearanceSettings()
    {
        PART_ContentHost.Content = _appearanceSettingsView;
    }

    private void ShowGeneralSettings()
    {
        PART_ContentHost.Content = _generalSettingsView;
    }

    private void ShowInfoSettings()
    {
        PART_ContentHost.Content = _infoSettingView;
    }

    private void ShowLogoSettings()
    {
        PART_ContentHost.Content = _logoSettingsView;
    }

    private void ShowKeyboardSettings()
    {
        PART_ContentHost.Content = _keyboardSettingsView;
    }

    private void ShowLyricsSettings()
    {
        PART_ContentHost.Content = _lyricsSettingsView;
    }

    private void ShowPlayerSettings()
    {
        PART_ContentHost.Content = _playerSettingsView;
    }

    private void ShowVisualizationsSettings()
    {
        PART_ContentHost.Content = _visualizationsSettingsView;
    }
}
