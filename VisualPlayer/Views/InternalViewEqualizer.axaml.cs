using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using VisualPlayer.Contexts;
using VisualPlayer.Resources;
using VisualPlayer.ViewModels.Equalizer;
using VisualPlayer.Views.Base;

namespace VisualPlayer.Views;

public partial class InternalViewEqualizer : InternalViewBase
{
    private readonly EqualizerConfigurationContext _config;
    private bool _canCreateProfile;
    private string _newProfileName = string.Empty;
    private string _searchText = string.Empty;
    private EqualizerProfile? _selectedProfile;

    #region AVALONIA PROPERTIES

    public static readonly DirectProperty<InternalViewEqualizer, bool> CanCreateProfileProperty
        = AvaloniaProperty.RegisterDirect<InternalViewEqualizer, bool>(nameof(CanCreateProfile), view => view.CanCreateProfile);

    public static readonly DirectProperty<InternalViewEqualizer, string> NewProfileNameProperty
        = AvaloniaProperty.RegisterDirect<InternalViewEqualizer, string>(nameof(NewProfileName), view => view.NewProfileName, (view, value) => view.NewProfileName = value);

    public static readonly DirectProperty<InternalViewEqualizer, string> SearchTextProperty
        = AvaloniaProperty.RegisterDirect<InternalViewEqualizer, string>(nameof(SearchText), view => view.SearchText, (view, value) => view.SearchText = value);

    public static readonly DirectProperty<InternalViewEqualizer, EqualizerProfile?> SelectedProfileProperty
        = AvaloniaProperty.RegisterDirect<InternalViewEqualizer, EqualizerProfile?>(nameof(SelectedProfile), view => view.SelectedProfile, (view, value) => view.SelectedProfile = value);

    #endregion AVALONIA PROPERTIES

    #region PROPERTIES

    public bool CanCreateProfile
    {
        get => _canCreateProfile;
        private set => SetAndRaise(CanCreateProfileProperty, ref _canCreateProfile, value);
    }

    public string NewProfileName
    {
        get => _newProfileName;
        set
        {
            SetAndRaise(NewProfileNameProperty, ref _newProfileName, value ?? string.Empty);
            UpdateCanCreateProfile();
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

    public EqualizerProfile? SelectedProfile
    {
        get => _selectedProfile;
        set
        {
            if (value is null || ReferenceEquals(_selectedProfile, value))
                return;

            SetAndRaise(SelectedProfileProperty, ref _selectedProfile, value);
            _config.SelectedProfileName = value.Name;
            UpdateCanCreateProfile();
        }
    }

    public ObservableCollection<EqualizerProfile> VisibleProfiles { get; } = [];

    #endregion PROPERTIES

    #region CONSTRUCTOR

    public InternalViewEqualizer()
    {
        Title = "Equalizer";
        EnableSaveButton = true;
        _config = ApplicationContext.Instance.EqualizerConfig;
        _config.EnsureProfiles();
        _config.Profiles.CollectionChanged += Profiles_CollectionChanged;
        SaveRequested += InternalViewEqualizer_SaveRequested;

        InitializeComponent();
        RefreshVisibleProfiles();
        SelectedProfile = _config.SelectedProfile;
    }

    #endregion CONSTRUCTOR

    #region ACTIONS

    private void CreateProfile()
    {
        if (!CanCreateProfile || SelectedProfile is null)
            return;

        var profile = SelectedProfile.Copy(NewProfileName.Trim());
        _config.Profiles.Add(profile);
        SelectedProfile = profile;
        NewProfileName = string.Empty;
        RefreshVisibleProfiles();
    }

    private void DeleteProfile(EqualizerProfile profile)
    {
        if (profile.IsDefault)
            return;

        var removedIndex = _config.Profiles.IndexOf(profile);
        var wasSelected = ReferenceEquals(profile, SelectedProfile);
        _config.Profiles.Remove(profile);
        _config.EnsureProfiles();

        if (wasSelected)
        {
            SelectedProfile = _config.Profiles[
                Math.Clamp(removedIndex - 1, 0, _config.Profiles.Count - 1)];
        }

        RefreshVisibleProfiles();
    }

    protected override void OnClosed()
    {
        _config.Profiles.CollectionChanged -= Profiles_CollectionChanged;
        SaveRequested -= InternalViewEqualizer_SaveRequested;
        base.OnClosed();
    }

    private void RefreshVisibleProfiles()
    {
        var searchText = SearchText.Trim();
        var profiles = _config.Profiles
            .Where(profile =>
                searchText.Length == 0
                || profile.Name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase))
            .OrderBy(profile => profile.IsDefault ? 0 : 1)
            .ThenBy(profile => profile.Name, StringComparer.CurrentCultureIgnoreCase);

        VisibleProfiles.Clear();
        foreach (var profile in profiles)
            VisibleProfiles.Add(profile);

        if (SelectedProfile is not null && VisibleProfiles.Contains(SelectedProfile))
            PART_NavigationList.SelectedItem = SelectedProfile;
    }

    private void UpdateCanCreateProfile()
    {
        var name = NewProfileName.Trim();
        CanCreateProfile = SelectedProfile is not null
            && name.Length > 0
            && !DefaultEqualizerProfiles.IsDefaultName(name)
            && !_config.Profiles.Any(profile =>
                string.Equals(profile.Name, name, StringComparison.CurrentCultureIgnoreCase));
    }

    #endregion ACTIONS

    #region BUTTON ACTIONS

    private void CreateProfileButton_Click(object? sender, RoutedEventArgs e)
    {
        CreateProfile();
    }

    private void DeleteProfileButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Control { DataContext: EqualizerProfile profile })
            DeleteProfile(profile);

        e.Handled = true;
    }

    #endregion BUTTON ACTIONS

    #region COLLECTION CHANGES

    private void Profiles_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        UpdateCanCreateProfile();
    }

    #endregion COLLECTION CHANGES

    #region SAVE

    private void InternalViewEqualizer_SaveRequested(object? sender, EventArgs e)
    {
        _config.Save();
    }

    #endregion SAVE
}
