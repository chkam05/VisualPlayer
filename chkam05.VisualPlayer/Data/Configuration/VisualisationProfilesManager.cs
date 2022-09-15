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

        private const string NEW_PROFILE_TEXT = "+ Create New Profile";


        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private VisualisationProfile _visualisationProfile;


        //  GETTERS & SETTERS

        public VisualisationProfile LoadedProfile
        {
            get => _visualisationProfile;
            set
            {
                _visualisationProfile = value;
                OnPropertyChanged(nameof(LoadedProfile));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> VisualisationProfilesManager class constructor. </summary>
        public VisualisationProfilesManager()
        {
            LoadedProfile = VisualisationProfile.DefaultProfile;
        }

        #endregion CLASS METHODS

        #region LOAD & SAVE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Load visualisation profile from saved profile file. </summary>
        /// <param name="profileName"> Visualisation profile name. </param>
        private void LoadProfile(string profileName)
        {
            var profileFileConfig = FilesManager.GetFileTypeByKind(Files.FileKind.JSON);
            var profilesPath = FilesManager.Instance.VisualisationsStoragePath;
            var profileFilePath = Path.Combine(profilesPath, profileName + profileFileConfig.Extension);

            if (File.Exists(profileFilePath))
            {
                var serialized = File.ReadAllText(profileFilePath);
                var profile = JsonConvert.DeserializeObject<VisualisationProfile>(serialized);

                if (profile != null)
                {
                    LoadedProfile = profile;
                    return;
                }
            }

            LoadedProfile = VisualisationProfile.DefaultProfile;
        }

        //  --------------------------------------------------------------------------------
        private void SaveProfile()
        {
            //
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
        private void CreateProfile()
        {
            //
        }

        //  --------------------------------------------------------------------------------
        public void RemoveProfile()
        {
            //
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Select visualisation profile to load. </summary>
        /// <param name="profileName"> Visualisation profile name. </param>
        public void SelectProfile(string profileName)
        {
            if (profileName == NEW_PROFILE_TEXT)
            {
                CreateProfile();
                return;
            }

            LoadProfile(profileName);
        }

        #endregion PROFILES MANAGEMENT METHODS

        #region STATIC METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get list of profiles that will be visible in interface. </summary>
        /// <returns> List of profiles visible in interface. </returns>
        public static List<string> GetProfilesList()
        {
            var profileFileConfig = FilesManager.GetFileTypesByGroup(Files.FileGroup.SETTINGS);
            var profilesPath = FilesManager.Instance.VisualisationsStoragePath;
            var profiles = FilesManager.GetFilesList(profilesPath, profileFileConfig);

            var result = new List<string>();
            result.Add(VisualisationProfile.DEFAULT_PROFILE_NAME);
            result.AddRange(profiles.Select(p => Path.GetFileNameWithoutExtension(p)));
            result.Add(NEW_PROFILE_TEXT);

            return result;
        }

        #endregion STATIC METHODS

    }
}
