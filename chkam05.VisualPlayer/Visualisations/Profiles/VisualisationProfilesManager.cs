using chkam05.VisualPlayer.Data.Configuration;
using chkam05.VisualPlayer.Data.Files;
using chkam05.VisualPlayer.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Visualisations.Profiles
{
    public class VisualisationProfilesManager : INotifyPropertyChanged
    {

        //  CONST

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
        private void LoadProfile(string profileName)
        {
            var profileFileExt = FilesManager.GetFileTypeByKind(FileKind.JSON).Extension;
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
        public void SaveCurrentProfile()
        {
            if (Profile != null)
                SaveProfile(Profile.Name);
        }

        //  --------------------------------------------------------------------------------
        private void SaveProfile(string profileName)
        {
            var profileFileExt = FilesManager.GetFileTypeByKind(FileKind.JSON).Extension;
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
        public void CreateProfile()
        {
            SaveCurrentProfile();

            string filePrefix = $"{NEW_PROFILE_NAME} ";
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

            Profile.Name = $"{filePrefix}{profileNameCounter + 1}";
            SaveProfile(Profile.Name);
        }

        //  --------------------------------------------------------------------------------
        public bool RenameCurrentProfile(string newProfileName)
        {
            var defaultProfileName = VisualisationProfile.DEFAULT_PROFILE_NAME.ToLower();

            if (Profile.Name.ToLower() != defaultProfileName && newProfileName.ToLower() != defaultProfileName
                && !GetProfilesList().Any(p => Path.GetFileNameWithoutExtension(p).ToLower() == newProfileName.ToLower()))
            {
                var profileFileExt = FilesManager.GetFileTypeByKind(FileKind.JSON).Extension;
                var profilesPath = FilesManager.Instance.VisualisationsStoragePath;
                var currentProfileFilePath = Path.Combine(profilesPath, Profile.Name + profileFileExt);

                if (File.Exists(currentProfileFilePath))
                    File.Delete(currentProfileFilePath);

                Profile.Name = newProfileName;
                SaveProfile(newProfileName);
                return true;
            }

            return false;
        }

        //  --------------------------------------------------------------------------------
        public void RemoveCurrentProfile()
        {
            var profileFileExt = FilesManager.GetFileTypeByKind(FileKind.JSON).Extension;
            var profilesPath = FilesManager.Instance.VisualisationsStoragePath;
            var profileFilePath = Path.Combine(profilesPath, Profile.Name + profileFileExt);

            if (File.Exists(profileFilePath))
                File.Delete(profileFilePath);

            LoadProfile(VisualisationProfile.DEFAULT_PROFILE_NAME);
        }

        //  --------------------------------------------------------------------------------
        public void SelectProfile(string profileName)
        {
            SaveCurrentProfile();
            LoadProfile(profileName);
        }

        #endregion PROFILES MANAGEMENT METHODS

        #region STATIC METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get list of profiles files paths. </summary>
        /// <returns> List of profiles files paths. </returns>
        private static List<string> GetProfilesList()
        {
            var profileFileConfig = FilesManager.GetFileTypesByGroup(FileGroup.SETTINGS);
            var profilesPath = FilesManager.Instance.VisualisationsStoragePath;
            var profiles = FilesManager.GetFilesList(profilesPath, profileFileConfig);

            return profiles;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get list of profiles that will be visible in interface. </summary>
        /// <returns> List of profiles visible in interface. </returns>
        public static List<string> GetProfilesNamesList()
        {
            var profiles = GetProfilesList();
            var result = new List<string>();

            if (!profiles.Any(p => Path.GetFileNameWithoutExtension(p) == VisualisationProfile.DEFAULT_PROFILE_NAME))
                result.Add(VisualisationProfile.DEFAULT_PROFILE_NAME);

            result.AddRange(profiles.Select(p => Path.GetFileNameWithoutExtension(p)));

            return result;
        }

        #endregion STATIC METHODS

    }
}
