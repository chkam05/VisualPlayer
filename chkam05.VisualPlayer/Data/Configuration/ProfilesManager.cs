using chkam05.VisualPlayer.Data.Files;
using chkam05.VisualPlayer.Utilities;
using chkam05.VisualPlayer.Visualisations.Profiles;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Data.Configuration
{
    public class ProfilesManager<T> : INotifyPropertyChanged where T : BaseProfile
    {

        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private ObservableCollection<string> _profilesNames;
        private T _selectedProfile;

        private string _newProfileName = "New Profile";
        private FileKind _profileFileType = FileKind.JSON;
        private string _profilesPath;


        //  GETTERS & SETTERS

        public ObservableCollection<string> ProfilesNames
        {
            get => _profilesNames;
            private set
            {
                _profilesNames = value;
                _profilesNames.CollectionChanged += (s, e) => { OnPropertyChanged(nameof(ProfilesNames)); };
                OnPropertyChanged(nameof(ProfilesNames));
            }
        }

        public T SelectedProfile
        {
            get => _selectedProfile;
            set
            {
                _selectedProfile = value;
                OnPropertyChanged(nameof(SelectedProfile));
            }
        }

        public string NewProfileName
        {
            get => _newProfileName;
            private set
            {
                _newProfileName = value;
                OnPropertyChanged(nameof(NewProfileName));
            }
        }

        public FileKind ProfileFileType
        {
            get => _profileFileType;
            private set
            {
                _profileFileType = value;
                OnPropertyChanged(nameof(ProfileFileType));
            }
        }

        public string ProfilesPath
        {
            get => _profilesPath;
            private set
            {
                _profilesPath = value;
                OnPropertyChanged(nameof(ProfilesPath));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> ProfilesManager class constructor. </summary>
        public ProfilesManager(string profilesPath, FileKind profileFileType, string newProfileName)
        {
            ProfilesPath = profilesPath;
            ProfileFileType = profileFileType;
            NewProfileName = newProfileName;

            SelectedProfile = GetDefaultProfile();
        }

        #endregion CLASS METHODS

        #region FILES METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get list of profiles files paths. </summary>
        /// <returns> List of profiles files paths. </returns>
        protected virtual List<string> GetProfilesList()
        {
            var profileFileConfig = FilesManager.GetFileTypeByKind(_profileFileType);
            var profilesPath = _profilesPath;
            var profiles = FilesManager.GetFilesList(profilesPath, new List<FileConfig> { profileFileConfig });

            return profiles;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update profiles names list. </summary>
        protected virtual void UpdateProfilesList()
        {
            var defaultProfiles = new List<string>() { GetDefaultProfile().Name };
            var profiles = GetProfilesList();
            var profilesNames = new List<string>();

            profilesNames.AddRange(defaultProfiles);

            profilesNames.AddRange(profiles
                .Select(p => Path.GetFileNameWithoutExtension(p))
                .Where(p => !defaultProfiles.Contains(p)));

            ProfilesNames = new ObservableCollection<string>(profilesNames);
        }

        #endregion FILES METHODS

        #region ITEMS MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Create new profile, based on current profile and save under new name. </summary>
        public virtual void CreateProfile()
        {
            SaveSelectedProfile();

            string filePrefix = $"{_newProfileName} ";
            int profileNameCounter = 0;
            var profilesWithNewName = GetProfilesList()
                .Select(p => Path.GetFileNameWithoutExtension(p))
                .Where(p => p.StartsWith(filePrefix));

            if (profilesWithNewName.Any())
            {
                profileNameCounter = profilesWithNewName.Select(p =>
                {
                    if (int.TryParse(p.Substring(filePrefix.Length), out int lastIndex))
                        return lastIndex;
                    return 0;
                }).Max();
            }

            SelectedProfile.Name = $"{filePrefix}{profileNameCounter + 1}";
            SaveProfile(SelectedProfile);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Rename selected profile. </summary>
        /// <param name="profileNewName"> Profile new name. </param>
        /// <returns> True - profile name has been changed; False - otherwise. </returns>
        public virtual bool RenameSelectedProfile(string profileNewName)
        {
            var defaultProfileName = GetDefaultProfile().Name.ToLower();

            if (SelectedProfile.Name.ToLower() != defaultProfileName && profileNewName.ToLower() != defaultProfileName
                && !GetProfilesList().Any(p => Path.GetFileNameWithoutExtension(p).ToLower() == profileNewName.ToLower()))
            {
                var profileFileExt = FilesManager.GetFileTypeByKind(_profileFileType).Extension;
                var profilesPath = _profilesPath;
                var currentProfileFilePath = Path.Combine(profilesPath, SelectedProfile.Name + profileFileExt);

                if (File.Exists(currentProfileFilePath))
                    File.Delete(currentProfileFilePath);

                SelectedProfile.Name = profileNewName;
                SaveProfile(SelectedProfile);
                return true;
            }

            return false;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Select profile by its name, and load. </summary>
        /// <param name="profileName"> Profile name. </param>
        public virtual void SelectProfile(string profileName)
        {
            SaveSelectedProfile();
            LoadProfile(profileName);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Remove selected profile. </summary>
        public virtual void RemoveSelectedProfile()
        {
            var profileFileExt = FilesManager.GetFileTypeByKind(_profileFileType).Extension;
            var profilesPath = FilesManager.Instance.VisualisationsStoragePath;
            var profileFilePath = Path.Combine(profilesPath, SelectedProfile.Name + profileFileExt);

            if (File.Exists(profileFilePath))
                File.Delete(profileFilePath);

            LoadProfile(GetDefaultProfile().Name);
        }

        #endregion ITEMS MANAGEMENT METHODS

        #region LOAD & SAVE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get profile default instance. </summary>
        /// <returns> Profile default instance. </returns>
        protected virtual T GetDefaultProfile()
        {
            return (T) BaseProfile.DefaultProfile;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Load profile from file by name. </summary>
        /// <param name="profileName"> Profile name that is the same as file name. </param>
        protected virtual void LoadProfile(string profileName)
        {
            var profileFileExt = FilesManager.GetFileTypeByKind(_profileFileType).Extension;
            var profilesPath = _profilesPath;
            var profileFilePath = Path.Combine(profilesPath, profileName + profileFileExt);

            if (File.Exists(profileFilePath))
            {
                var serialized = File.ReadAllText(profileFilePath);
                var profile = JsonConvert.DeserializeObject<T>(serialized);

                if (profile != null)
                {
                    SelectedProfile = profile;
                    return;
                }
            }

            SelectedProfile = GetDefaultProfile();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Save profile to file, which name will be name of the file. </summary>
        /// <param name="profile"> Profile object. </param>
        protected virtual void SaveProfile(T profile)
        {
            var profileFileExt = FilesManager.GetFileTypeByKind(_profileFileType).Extension;
            var profilesPath = _profilesPath;
            var profileFilePath = Path.Combine(profilesPath, profile.Name + profileFileExt);

            var serialized = JsonConvert.SerializeObject(profile, Formatting.Indented);
            File.WriteAllText(profileFilePath, serialized);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Save current profile to file. </summary>
        protected virtual void SaveSelectedProfile()
        {
            if (SelectedProfile != null)
                SaveProfile(SelectedProfile);
        }

        #endregion LOAD & SAVE METHODS

        #region NOTIFY PROPERTIES CHANGED INTERFACE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method for invoking PropertyChangedEventHandler external method. </summary>
        /// <param name="propertyName"> Changed property name. </param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion NOTIFY PROPERTIES CHANGED INTERFACE METHODS

    }
}
