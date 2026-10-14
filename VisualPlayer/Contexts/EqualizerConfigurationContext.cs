using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using VisualPlayer.Attributes;
using VisualPlayer.Resources;
using VisualPlayer.ViewModels.Equalizer;

namespace VisualPlayer.Contexts;

public class EqualizerConfigurationContext : ConfigurationContextBase
{
    #region CONST

    private const string FILE_PATH = "equalizer.json";

    #endregion CONST

    private ObservableCollection<EqualizerProfile> _profiles;
    private string _selectedProfileName;

    #region PROPERTIES

    [NotifyPropertyUpdate]
    public ObservableCollection<EqualizerProfile> Profiles
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
    public EqualizerProfile SelectedProfile
    {
        get
        {
            EnsureProfiles();

            return Profiles.First(profile =>
                string.Equals(profile.Name, SelectedProfileName, StringComparison.OrdinalIgnoreCase));
        }
    }

    public static string FilePath
    {
        get => Path.Combine(DirectoryPath, FILE_PATH);
    }

    #endregion PROPERTIES

    #region CONSTRUCTOR

    [JsonConstructor]
    private EqualizerConfigurationContext()
    {
        _profiles = new ObservableCollection<EqualizerProfile>(DefaultEqualizerProfiles.Create());
        _selectedProfileName = DefaultEqualizerProfiles.FLAT_NAME;
    }

    #endregion CONSTRUCTOR

    #region LOAD / SAVE

    public void EnsureProfiles()
    {
        RemoveDuplicateNames();

        var defaultProfiles = DefaultEqualizerProfiles.Create();

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
            
            if (!existingProfile.IsDefault || !DefaultEqualizerProfiles.IsCompatible(existingProfile))
            {
                Profiles[existingIndex] = defaultProfile;
                continue;
            }

            Profiles[existingIndex] = defaultProfile;
        }

        if (FindProfileIndex(SelectedProfileName) < 0)
            SelectedProfileName = DefaultEqualizerProfiles.FLAT_NAME;
    }

    public static EqualizerConfigurationContext Load()
    {
        var context = LoadFromFile(FilePath, () => new EqualizerConfigurationContext());
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
            if (string.Equals(Profiles[index].Name, name, StringComparison.OrdinalIgnoreCase))
                return index;
        }

        return -1;
    }

    private void RemoveDuplicateNames()
    {
        var retainedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (var index = Profiles.Count - 1; index >= 0; index--)
        {
            if (string.IsNullOrWhiteSpace(Profiles[index].Name)
                || !retainedNames.Add(Profiles[index].Name))
            {
                Profiles.RemoveAt(index);
            }
        }
    }

    #endregion LOAD / SAVE
}
