using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using VisualPlayer.Attributes;
using VisualPlayer.Resources;
using VisualPlayer.Visualizations.Config;
using VisualPlayer.Visualizations.Config.Base;

namespace VisualPlayer.Contexts;

public class VisualizationConfigurationContext : ConfigurationContextBase
{
    #region CONST

    private const string FILE_PATH = "visualizations.json";

    #endregion CONST

    private VisualizationGlobalConfig _global;
    private ObservableCollection<VisualisationProfile> _profiles;
    private string _selectedProfileName;

    #region PROPERTIES

    [NotifyPropertyUpdate]
    public VisualizationGlobalConfig Global
    {
        get => _global;
        set => UpdateProperty(ref _global, value ?? new VisualizationGlobalConfig());
    }

    [NotifyPropertyUpdate]
    public ObservableCollection<VisualisationProfile> Profiles
    {
        get => _profiles;
        set => UpdateProperty(ref _profiles, value ?? []);
    }

    [NotifyPropertyUpdate]
    public string SelectedProfileName
    {
        get => _selectedProfileName;
        set => UpdateProperty(ref _selectedProfileName, value?.Trim() ?? string.Empty);
    }


    [JsonIgnore]
    public VisualisationProfile SelectedProfile
    {
        get
        {
            EnsureProfiles();

            return Profiles.First(profile =>
                string.Equals(
                    profile.Name,
                    SelectedProfileName,
                    StringComparison.OrdinalIgnoreCase));
        }
    }

    public static string FilePath => Path.Combine(DirectoryPath, FILE_PATH);

    #endregion PROPERTIES

    #region CONSTRUCTOR

    [JsonConstructor]
    private VisualizationConfigurationContext()
    {
        _global = new VisualizationGlobalConfig();
        _profiles = new ObservableCollection<VisualisationProfile>(
            DefaultVisualizationProfiles.Create());
        _selectedProfileName = DefaultVisualizationProfiles.NONE_NAME;
    }

    #endregion CONSTRUCTOR

    #region LOAD / SAVE

    public void EnsureProfiles()
    {
        RemoveLegacyOscilloscopeDoubleProfile();
        RemoveDuplicateNames();

        var defaultProfiles = DefaultVisualizationProfiles.Create();
        for (var index = 0; index < defaultProfiles.Count; index++)
        {
            var defaultProfile = defaultProfiles[index];
            var existingIndex = FindProfileIndex(defaultProfile.Name);

            if (existingIndex < 0)
            {
                Profiles.Insert(Math.Min(index, Profiles.Count), defaultProfile);
                continue;
            }

            var existingProfile = Profiles[existingIndex];
            var isCompatible = existingProfile.IsDefault
                && existingProfile.VisualizationType == defaultProfile.VisualizationType
                && DefaultVisualizationProfiles.IsCompatible(existingProfile);

            if (!isCompatible)
            {
                Profiles[existingIndex] = defaultProfile;
                continue;
            }

            existingProfile.Name = defaultProfile.Name;
            existingProfile.IsDefault = true;
        }

        if (Profiles.Count == 0)
        {
            foreach (var profile in DefaultVisualizationProfiles.Create())
                Profiles.Add(profile);
        }

        if (FindProfileIndex(SelectedProfileName) < 0)
        {
            SelectedProfileName = FindProfileIndex(DefaultVisualizationProfiles.NONE_NAME) >= 0
                ? DefaultVisualizationProfiles.NONE_NAME
                : Profiles[0].Name;
        }
    }

    public static VisualizationConfigurationContext Load()
    {
        var context = LoadFromFile(FilePath, () => new VisualizationConfigurationContext());
        context.EnsureProfiles();
        return context;
    }

    public void Save()
    {
        EnsureProfiles();
        SaveToFile(FilePath, this);
    }


    private int FindProfileIndex(string name)
    {
        for (var index = 0; index < Profiles.Count; index++)
        {
            if (string.Equals(
                Profiles[index].Name,
                name,
                StringComparison.OrdinalIgnoreCase))
            {
                return index;
            }
        }

        return -1;
    }

    private void RemoveLegacyOscilloscopeDoubleProfile()
    {
        const string legacyName = "Oscilloscope - Double";

        for (var index = Profiles.Count - 1; index >= 0; index--)
        {
            var profile = Profiles[index];
            if (!profile.IsDefault
                || !string.Equals(profile.Name, legacyName, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            Profiles.RemoveAt(index);
            if (string.Equals(SelectedProfileName, legacyName, StringComparison.OrdinalIgnoreCase))
                SelectedProfileName = DefaultVisualizationProfiles.OSCILLOSCOPE_FILLED_NAME;
        }
    }

    private void RemoveDuplicateNames()
    {
        var retainedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (var index = Profiles.Count - 1; index >= 0; index--)
        {
            var profileName = Profiles[index].Name;
            if (string.IsNullOrWhiteSpace(profileName)
                || !retainedNames.Add(profileName))
            {
                Profiles.RemoveAt(index);
            }
        }
    }

    #endregion LOAD / SAVE
}
