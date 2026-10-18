using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using VisualPlayer.Contexts;
using VisualPlayer.Visualizations.Registry;
using VisualPlayer.Events;
using VisualPlayer.Visualizations.Config.Base;
using VisualPlayer.Views.Base;

namespace VisualPlayer.Views;

public partial class InternalViewVisualizations : InternalViewBase
{
    private static TranslationContext T => ApplicationContext.Instance.Translations;

    private readonly VisualizationConfigurationContext _config;

    private bool _canSaveProfile;
    private bool _sortAscending = true;
    private string _profileName = string.Empty;
    private string _searchText = string.Empty;
    private VisualisationProfile? _selectedProfile;

    #region AVALONIA PROPERTIES

    public static readonly DirectProperty<InternalViewVisualizations, bool> CanSaveProfileProperty
        = AvaloniaProperty.RegisterDirect<InternalViewVisualizations, bool>(nameof(CanSaveProfile), view => view.CanSaveProfile);

    public static readonly DirectProperty<InternalViewVisualizations, string> ProfileNameProperty
        = AvaloniaProperty.RegisterDirect<InternalViewVisualizations, string>(nameof(ProfileName), view => view.ProfileName, (view, value) => view.ProfileName = value);

    public static readonly DirectProperty<InternalViewVisualizations, string> SearchTextProperty
        = AvaloniaProperty.RegisterDirect<InternalViewVisualizations, string>(nameof(SearchText), view => view.SearchText, (view, value) => view.SearchText = value);

    public static readonly DirectProperty<InternalViewVisualizations, VisualisationProfile?> SelectedProfileProperty
        = AvaloniaProperty.RegisterDirect<InternalViewVisualizations, VisualisationProfile?>(nameof(SelectedProfile), view => view.SelectedProfile, (view, value) => view.SelectedProfile = value);

    #endregion AVALONIA PROPERTIES

    #region EVENTS

    public event EventHandler<VisualizationProfileOverwriteRequestedEventArgs>? ProfileOverwriteRequested;

    #endregion EVENTS

    #region PROPERTIES

    public bool CanSaveProfile
    {
        get => _canSaveProfile;
        private set => SetAndRaise(CanSaveProfileProperty, ref _canSaveProfile, value);
    }

    public string ProfileName
    {
        get => _profileName;
        set
        {
            SetAndRaise(ProfileNameProperty, ref _profileName, value ?? string.Empty);
            UpdateCanSaveProfile();
        }
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            SetAndRaise(SearchTextProperty, ref _searchText, value ?? string.Empty);
            RefreshVisibleProfiles();
        }
    }

    public VisualisationProfile? SelectedProfile
    {
        get => _selectedProfile;
        set
        {
            if (ReferenceEquals(_selectedProfile, value) || value is null)
                return;

            if (_selectedProfile is not null)
                _selectedProfile.PropertyChanged -= SelectedProfile_PropertyChanged;

            SetAndRaise(SelectedProfileProperty, ref _selectedProfile, value);
            value.PropertyChanged += SelectedProfile_PropertyChanged;
            _config.SelectedProfileId = value.ProfileId;
            ProfileName = value.Name;
            UpdateConfigView();
        }
    }

    public ObservableCollection<VisualisationProfile> VisibleProfiles { get; } = [];

    public ReadOnlyObservableCollection<VisualizationRegistration> VisualizationTypes => _config.Registry.Available;

    #endregion PROPERTIES

    #region CONSTRUCTOR

    public InternalViewVisualizations()
    {
        Title = T["MENU_VISUALIZATIONS"];
        EnableSaveButton = true;
        SaveRequested += InternalViewVisualizations_SaveRequested;
        _config = ApplicationContext.Instance.VisualizationConfig;
        _config.EnsureProfiles();
        _config.Profiles.CollectionChanged += Profiles_CollectionChanged;
        T.LanguageChanged += Translations_LanguageChanged;

        InitializeComponent();
        RefreshVisibleProfiles();
        SelectedProfile = _config.SelectedProfile;
    }

    #endregion CONSTRUCTOR

    #region ACTIONS

    protected override void OnClosed()
    {
        SaveRequested -= InternalViewVisualizations_SaveRequested;
        _config.Profiles.CollectionChanged -= Profiles_CollectionChanged;
        T.LanguageChanged -= Translations_LanguageChanged;
        if (_selectedProfile is not null)
            _selectedProfile.PropertyChanged -= SelectedProfile_PropertyChanged;

        PART_ConfigHost.Content = null;
        base.OnClosed();
    }

    private void Translations_LanguageChanged(object? sender, EventArgs e)
    {
        Title = T["MENU_VISUALIZATIONS"];
        RefreshVisibleProfiles();
        if (SelectedProfile is { IsAvailable: false } || SelectedProfile?.PluginId == VisualizationRegistry.NoneId) UpdateConfigView();
    }

    private void InternalViewVisualizations_SaveRequested(object? sender, EventArgs e)
    {
        try { _config.Save(); }
        catch (Exception exception) { ShowStatus(T.Format("VIEW_VISUALIZATIONS_SAVE_ERROR", exception.Message)); }
    }

    public void OverwriteProfile(VisualisationProfile profile)
    {
        if (SelectedProfile is null || profile.IsDefault)
            return;

        profile.OverwriteFrom(SelectedProfile);
        SelectedProfile = profile;
        ProfileName = profile.Name;
        RefreshVisibleProfiles();
    }

    private void CreateProfile(string profileName)
    {
        if (SelectedProfile is null)
            return;

        var profile = SelectedProfile.Copy(profileName);
        _config.Profiles.Add(profile);
        SelectedProfile = profile;
    }

    private void DeleteProfile(VisualisationProfile profile)
    {
        if (profile.IsDefault)
            return;

        var removedIndex = _config.Profiles.IndexOf(profile);
        var wasSelected = ReferenceEquals(profile, SelectedProfile);
        _config.Profiles.Remove(profile);
        _config.EnsureProfiles();

        if (wasSelected)
            SelectedProfile = _config.Profiles[Math.Clamp(removedIndex - 1, 0, _config.Profiles.Count - 1)];
    }

    private void RefreshVisibleProfiles()
    {
        if (_config is null)
            return;

        var searchText = SearchText.Trim();
        var profiles = _config.Profiles.Where(profile =>
            searchText.Length == 0
            || profile.DisplayName.Contains(searchText, StringComparison.CurrentCultureIgnoreCase));

        if (_sortAscending)
        {
            profiles = profiles
                .OrderBy(profile => profile.IsDefault ? 0 : 1)
                .ThenBy(profile => profile.DisplayName, StringComparer.CurrentCultureIgnoreCase);
        }
        else
        {
            profiles = profiles
                .OrderBy(profile => profile.IsDefault ? 0 : 1)
                .ThenByDescending(profile => profile.DisplayName, StringComparer.CurrentCultureIgnoreCase);
        }

        VisibleProfiles.Clear();
        foreach (var profile in profiles)
            VisibleProfiles.Add(profile);

        if (PART_NavigationList is not null
            && SelectedProfile is not null
            && VisibleProfiles.Contains(SelectedProfile))
            PART_NavigationList.SelectedItem = SelectedProfile;
    }

    private void UpdateCanSaveProfile()
    {
        var profileName = ProfileName.Trim();
        CanSaveProfile = SelectedProfile is not null
            && profileName.Length > 0
            && !_config.Profiles.Any(p => p.IsDefault && (p.Name.Equals(profileName, StringComparison.CurrentCultureIgnoreCase) || p.DisplayName.Equals(profileName, StringComparison.CurrentCultureIgnoreCase)));
    }

    private void UpdateConfigView()
    {
        if (PART_ConfigHost is null || SelectedProfile is null)
            return;

        PART_ConfigHost.Content = null;
        if (!SelectedProfile.IsAvailable)
        {
            ShowStatus(T.Format("VIEW_VISUALIZATIONS_PLUGIN_UNAVAILABLE", SelectedProfile.PluginId, SelectedProfile.ConfigurationError ?? string.Empty));
            return;
        }
        if (SelectedProfile.PluginId == VisualizationRegistry.NoneId)
        {
            ShowStatus(T["VIEW_VISUALIZATIONS_DISABLED"]);
            return;
        }
        try { PART_ConfigHost.Content = SelectedProfile.Registration?.CreateEditor(SelectedProfile); }
        catch (Exception exception)
        {
            SelectedProfile.Registration?.Report("Cannot create configuration editor.", exception);
            ShowStatus(T.Format("VIEW_VISUALIZATIONS_EDITOR_ERROR", SelectedProfile.PluginId));
        }
    }

    private void ShowStatus(string text) => PART_ConfigHost.Content = new TextBlock
    {
        Margin = new Thickness(8), Opacity = 0.6, Text = text, TextWrapping = Avalonia.Media.TextWrapping.Wrap
    };
    #endregion ACTIONS

    #region BUTTON ACTIONS

    private void DeleteProfileButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Control { DataContext: VisualisationProfile profile })
            DeleteProfile(profile);

        e.Handled = true;
    }

    private void SaveProfileButton_Click(object? sender, RoutedEventArgs e)
    {
        if (!CanSaveProfile)
            return;

        var profileName = ProfileName.Trim();
        var existingProfile = _config.Profiles.FirstOrDefault(profile =>
            !profile.IsDefault
            && string.Equals(profile.Name, profileName, StringComparison.CurrentCultureIgnoreCase));

        if (existingProfile is not null)
        {
            ProfileOverwriteRequested?.Invoke(
                this,
                new VisualizationProfileOverwriteRequestedEventArgs(existingProfile));
            return;
        }

        CreateProfile(profileName);
    }

    private void SortButton_Click(object? sender, RoutedEventArgs e)
    {
        PART_SortButton.ContextMenu?.Open(PART_SortButton);
        e.Handled = true;
    }

    private void SortAscendingMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        _sortAscending = true;
        RefreshVisibleProfiles();
    }

    private void SortDescendingMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        _sortAscending = false;
        RefreshVisibleProfiles();
    }

    #endregion BUTTON ACTIONS

    #region COLLECTION CHANGES

    private void Profiles_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        RefreshVisibleProfiles();
        UpdateCanSaveProfile();
    }

    #endregion COLLECTION CHANGES

    #region PROPERTY CHANGES

    private void SelectedProfile_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(VisualisationProfile.Config))
            UpdateConfigView();

        if (e.PropertyName is nameof(VisualisationProfile.Name) or nameof(VisualisationProfile.DisplayName))
            RefreshVisibleProfiles();
    }

    #endregion PROPERTY CHANGES
}
