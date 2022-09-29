using chkam05.VisualPlayer.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Data.Configuration
{
    public class VisualisationProfilesManager : INotifyPropertyChanged
    {

        //  CONST

        private const string CREATE_PROFILE_TEXT = "+ Create New Profile";
        private const string NEW_PROFILE_NAME = "New Profile";


        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private VisualisationProfile _profile;


        //  GETTERS & SETTERS

        public VisualisationProfile Profile
        {
            get => _profile;
            set
            {
                _profile = value;
                OnPropertyChanged(nameof(Profile));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> VisualisationProfilesManager class constructor. </summary>
        public VisualisationProfilesManager()
        {
            Profile = VisualisationProfile.DefaultProfile;
        }

        #endregion CLASS METHODS

        #region LOAD & SAVE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Load visualisation profile from saved profile file. </summary>
        /// <param name="profileName"> Visualisation profile name. </param>
        private void LoadProfile(string profileName)
        {
            var profileFileExt = FilesManager.GetFileTypeByKind(Files.FileKind.JSON).Extension;
            var profilesPath = FilesManager.Instance.VisualisationsStoragePath;
            var profileFilePath = Path.Combine(profilesPath, profileName + profileFileExt);

            if (File.Exists(profileFilePath))
            {
                var serialized = File.ReadAllText(profileFilePath);
                var profile = JsonConvert.DeserializeObject<VisualisationProfile>(serialized);

                if (profile != null)
                {
                    Profile = profile;
                    return;
                }
            }

            Profile = VisualisationProfile.DefaultProfile;
        }

        //  --------------------------------------------------------------------------------
        public void SaveProfile()
        {
            SaveProfile(Profile.Name);
        }

        //  --------------------------------------------------------------------------------
        public void SaveProfile(string profileName)
        {
            var profileFileExt = FilesManager.GetFileTypeByKind(Files.FileKind.JSON).Extension;
            var profilesPath = FilesManager.Instance.VisualisationsStoragePath;
            var profileFilePath = Path.Combine(profilesPath, profileName + profileFileExt);

            var serialized = JsonConvert.SerializeObject(Profile, Formatting.Indented);
            File.WriteAllText(profileFilePath, serialized);
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

        #region PROFILES MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Create new profile and load it. </summary>
        private void CreateProfile()
        {
            string filePrefix = $"{NEW_PROFILE_NAME} ";
            int index = 0;
            var newProfiles = GetProfilesFilesList()
                .Select(p => Path.GetFileNameWithoutExtension(p))
                .Where(p => p.StartsWith(filePrefix));

            if (newProfiles.Any())
            {
                index = newProfiles.Select(p =>
                {
                    if (int.TryParse(p.Substring(filePrefix.Length), out int lastIndex))
                        return lastIndex;
                    return 0;
                }).Max();
            }

            Profile.Name = $"{filePrefix}{index+1}";
            SaveProfile(Profile.Name);
            OnPropertyChanged(nameof(Profile));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Rename current loaded profile if is not default. </summary>
        /// <param name="profileName"> New profile name. </param>
        /// <returns> True - profile name changed; False - otherwise. </returns>
        public bool RenameProfile(string profileName)
        {
            if (Profile.Name == VisualisationProfile.DEFAULT_PROFILE_NAME 
                || profileName.ToLower() == VisualisationProfile.DEFAULT_PROFILE_NAME.ToLower())
                return false;

            if (!GetProfilesFilesList().Any(p => Path.GetFileNameWithoutExtension(p).ToLower() != profileName.ToLower()))
            {
                var profileFileExt = FilesManager.GetFileTypeByKind(Files.FileKind.JSON).Extension;
                var profilesPath = FilesManager.Instance.VisualisationsStoragePath;
                var currentProfileFilePath = Path.Combine(profilesPath, Profile.Name + profileFileExt);

                if (File.Exists(currentProfileFilePath))
                    File.Delete(currentProfileFilePath);

                Profile.Name = profileName;
                SaveProfile(profileName);
                return true;
            }

            return false;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Remove current loaded profile and back to default. </summary>
        public void RemoveProfile()
        {
            var profileFileExt = FilesManager.GetFileTypeByKind(Files.FileKind.JSON).Extension;
            var profilesPath = FilesManager.Instance.VisualisationsStoragePath;
            var profileFilePath = Path.Combine(profilesPath, Profile.Name + profileFileExt);

            if (File.Exists(profileFilePath))
                File.Delete(profileFilePath);

            SelectProfile(VisualisationProfile.DEFAULT_PROFILE_NAME);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Select visualisation profile to load. </summary>
        /// <param name="profileName"> Visualisation profile name. </param>
        public void SelectProfile(string profileName)
        {
            if (profileName == CREATE_PROFILE_TEXT)
            {
                CreateProfile();
                return;
            }

            LoadProfile(profileName);
        }

        #endregion PROFILES MANAGEMENT METHODS

        #region STATIC METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get list of profiles files paths. </summary>
        /// <returns> List of profiles files paths. </returns>
        private static List<string> GetProfilesFilesList()
        {
            var profileFileConfig = FilesManager.GetFileTypesByGroup(Files.FileGroup.SETTINGS);
            var profilesPath = FilesManager.Instance.VisualisationsStoragePath;
            var profiles = FilesManager.GetFilesList(profilesPath, profileFileConfig);

            return profiles;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get list of profiles that will be visible in interface. </summary>
        /// <returns> List of profiles visible in interface. </returns>
        public static List<string> GetProfilesList()
        {
            var profiles = GetProfilesFilesList();
            var result = new List<string>();

            if (!profiles.Any(p => Path.GetFileNameWithoutExtension(p) == VisualisationProfile.DEFAULT_PROFILE_NAME))
                result.Add(VisualisationProfile.DEFAULT_PROFILE_NAME);

            result.AddRange(profiles.Select(p => Path.GetFileNameWithoutExtension(p)));
            result.Add(CREATE_PROFILE_TEXT);

            return result;
        }

        #endregion STATIC METHODS

    }
}
