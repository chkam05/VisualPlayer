using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Lucide.Avalonia;
using VisualPlayer.Contexts;
using VisualPlayer.Enums;
using VisualPlayer.Enums.UI;
using VisualPlayer.Events;
using VisualPlayer.Resources;
using VisualPlayer.Visualizations.Config;
using VisualPlayer.Visualizations.Config.Base;
using VisualPlayer.Views.Base;
using VisualPlayer.Views.Dialogs;
using VisualPlayer.Views.Visualisations;
using VisualPlayer.Windows;

namespace VisualPlayer.Views;

public partial class InternalViewVisualizations : InternalViewBase
{
    private static TranslationContext T => ApplicationContext.Instance.Translations;

    private readonly VisualizationConfigurationContext _config;

    private readonly PeaksFloatingConfig _peaksFloatingConfig = new();
    private readonly OscilloscopeCenterConfig _oscilloscopeCenterConfig = new();
    private readonly OscilloscopeFilledConfig _oscilloscopeFilledConfig = new();
    private readonly OscilloscopeDoubleConfig _oscilloscopeDoubleConfig = new();
    private readonly PeaksFloatingMirroredConfig _peaksFloatingMirroredConfig = new();
    private readonly PeaksMirroredConfig _peaksMirroredConfig = new();
    private readonly PeaksBouncingConfig _peaksBouncingConfig = new();
    private readonly StripesBouncingConfig _stripesBouncingConfig = new();
    private readonly StripesMirroredConfig _stripesMirroredConfig = new();
    private readonly OscilloscopeConfig _oscilloscopeConfig = new();
    private readonly PeaksConfig _peaksConfig = new();
    private readonly StripesConfig _stripesConfig = new();
    private readonly WaveformConfig _waveformConfig = new();
    private readonly WaveformMirroredConfig _waveformMirroredConfig = new();
    private readonly CircleStripesConfig _circleStripesConfig = new();
    private readonly CircleWaveformConfig _circleWaveformConfig = new();
    
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
            _config.SelectedProfileName = value.Name;
            ProfileName = value.Name;
            UpdateConfigView();
        }
    }

    public ObservableCollection<VisualisationProfile> VisibleProfiles { get; } = [];

    public IReadOnlyList<VisualizationType> VisualizationTypes { get; }
        = Enum.GetValues<VisualizationType>()
            .OrderBy(type => type.ToString(), StringComparer.Ordinal)
            .ToArray();

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

        base.OnClosed();
    }

    private void Translations_LanguageChanged(object? sender, EventArgs e)
    {
        Title = T["MENU_VISUALIZATIONS"];
        RefreshVisibleProfiles();
        UpdateConfigView();
    }

    private void InternalViewVisualizations_SaveRequested(object? sender, EventArgs e)
    {
        _config.Save();
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
            || profile.Name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase));

        if (_sortAscending)
        {
            profiles = profiles
                .OrderBy(profile => profile.IsDefault ? 0 : 1)
                .ThenBy(profile => profile.Name, StringComparer.CurrentCultureIgnoreCase);
        }
        else
        {
            profiles = profiles
                .OrderBy(profile => profile.IsDefault ? 0 : 1)
                .ThenByDescending(profile => profile.Name, StringComparer.CurrentCultureIgnoreCase);
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
            && !DefaultVisualizationProfiles.IsDefaultName(profileName);
    }

    private void UpdateConfigView()
    {
        if (PART_ConfigHost is null || SelectedProfile is null)
            return;

        switch (SelectedProfile.VisualizationType)
        {
            case VisualizationType.PeaksBouncing:
                SelectedProfile.GetConfig<PeaksBouncingVisualizationConfig>();
                _peaksBouncingConfig.DataContext = SelectedProfile;
                PART_ConfigHost.Content = _peaksBouncingConfig;
                break;
            case VisualizationType.StripesBouncing:
                SelectedProfile.GetConfig<StripesBouncingVisualizationConfig>();
                _stripesBouncingConfig.DataContext = SelectedProfile;
                PART_ConfigHost.Content = _stripesBouncingConfig;
                break;
            case VisualizationType.Peaks:
                SelectedProfile.GetConfig<PeaksVisualizationConfig>();
                _peaksConfig.DataContext = SelectedProfile;
                PART_ConfigHost.Content = _peaksConfig;
                break;
            case VisualizationType.Stripes:
                SelectedProfile.GetConfig<StripesVisualizationConfig>();
                _stripesConfig.DataContext = SelectedProfile;
                PART_ConfigHost.Content = _stripesConfig;
                break;
            case VisualizationType.PeaksMirrored:
                SelectedProfile.GetConfig<PeaksMirroredVisualizationConfig>();
                _peaksMirroredConfig.DataContext = SelectedProfile;
                PART_ConfigHost.Content = _peaksMirroredConfig;
                break;
            case VisualizationType.StripesMirrored:
                SelectedProfile.GetConfig<StripesMirroredVisualizationConfig>();
                _stripesMirroredConfig.DataContext = SelectedProfile;
                PART_ConfigHost.Content = _stripesMirroredConfig;
                break;
            case VisualizationType.PeaksFloating:
                SelectedProfile.GetConfig<PeaksFloatingVisualizationConfig>();
                _peaksFloatingConfig.DataContext = SelectedProfile;
                PART_ConfigHost.Content = _peaksFloatingConfig;
                break;
            case VisualizationType.PeaksFloatingMirrored:
                SelectedProfile.GetConfig<PeaksFloatingMirroredVisualizationConfig>();
                _peaksFloatingMirroredConfig.DataContext = SelectedProfile;
                PART_ConfigHost.Content = _peaksFloatingMirroredConfig;
                break;
            case VisualizationType.Oscilloscope:
                SelectedProfile.GetConfig<OscilloscopeVisualizationConfig>();
                _oscilloscopeConfig.DataContext = SelectedProfile;
                PART_ConfigHost.Content = _oscilloscopeConfig;
                break;
            case VisualizationType.OscilloscopeDouble:
                SelectedProfile.GetConfig<OscilloscopeDoubleVisualizationConfig>();
                _oscilloscopeDoubleConfig.DataContext = SelectedProfile;
                PART_ConfigHost.Content = _oscilloscopeDoubleConfig;
                break;
            case VisualizationType.OscilloscopeFilled:
                SelectedProfile.GetConfig<OscilloscopeFilledVisualizationConfig>();
                _oscilloscopeFilledConfig.DataContext = SelectedProfile;
                PART_ConfigHost.Content = _oscilloscopeFilledConfig;
                break;
            case VisualizationType.OscilloscopeCenter:
                SelectedProfile.GetConfig<OscilloscopeCenterVisualizationConfig>();
                _oscilloscopeCenterConfig.DataContext = SelectedProfile;
                PART_ConfigHost.Content = _oscilloscopeCenterConfig;
                break;
            case VisualizationType.Waveform:
                SelectedProfile.GetConfig<WaveformVisualizationConfig>();
                _waveformConfig.DataContext = SelectedProfile;
                PART_ConfigHost.Content = _waveformConfig;
                break;
            case VisualizationType.WaveformMirrored:
                SelectedProfile.GetConfig<WaveformMirroredVisualizationConfig>();
                _waveformMirroredConfig.DataContext = SelectedProfile;
                PART_ConfigHost.Content = _waveformMirroredConfig;
                break;
            case VisualizationType.CircleStripes:
                SelectedProfile.GetConfig<CircleStripesVisualizationConfig>();
                _circleStripesConfig.DataContext = SelectedProfile;
                PART_ConfigHost.Content = _circleStripesConfig;
                break;
            case VisualizationType.CircleWaveform:
                SelectedProfile.GetConfig<CircleWaveformVisualizationConfig>();
                _circleWaveformConfig.DataContext = SelectedProfile;
                PART_ConfigHost.Content = _circleWaveformConfig;
                break;
            default:
                PART_ConfigHost.Content = new TextBlock
                {
                    Margin = new Thickness(8),
                    Opacity = 0.6,
                    Text = T["VIEW_VISUALIZATIONS_DISABLED"]
                };
                break;
        }
    }

    #endregion ACTIONS

    #region BUTTON ACTIONS

    private async void DeleteProfileButton_Click(object? sender, RoutedEventArgs e)
    {
        e.Handled = true;
        if (sender is not Control { DataContext: VisualisationProfile { IsDefault: false } profile }
            || TopLevel.GetTopLevel(this) is not MainWindow window)
        {
            return;
        }

        var dialog = new InternalMessageDialog(
            T["VIEW_VISUALIZATIONS_DELETE_TITLE"],
            T.Format("VIEW_VISUALIZATIONS_DELETE_MESSAGE", profile.Name),
            InternalDialogButtons.YesNo,
            LucideIconKind.Trash);

        if (await window.ShowDialogAsync(dialog) == InternalDialogResult.Yes)
            DeleteProfile(profile);
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
        if (e.PropertyName == nameof(VisualisationProfile.VisualizationType))
            UpdateConfigView();

        if (e.PropertyName == nameof(VisualisationProfile.Name))
            RefreshVisibleProfiles();
    }

    #endregion PROPERTY CHANGES
}
