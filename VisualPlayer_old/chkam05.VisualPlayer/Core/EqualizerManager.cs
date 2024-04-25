using chkam05.VisualPlayer.Core.Data;
using chkam05.VisualPlayer.Core.Events;
using chkam05.VisualPlayer.Data.Configuration;
using chkam05.VisualPlayer.Data.Files;
using chkam05.VisualPlayer.Utilities;
using chkam05.VisualPlayer.Visualisations.Profiles;
using CSCore.Streams.Effects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Core
{
    public class EqualizerManager : INotifyPropertyChanged
    {

        //  CONST

        private const string NEW_PRESET_NAME = "New Preset";
        public static readonly string[] DEFAULT_PRESETS_NAMES = new string[]
        {
            nameof(EqualizerPreset.Default),
            nameof(EqualizerPreset.Bass),
            nameof(EqualizerPreset.Dance),
            nameof(EqualizerPreset.Live),
            nameof(EqualizerPreset.Pop),
            nameof(EqualizerPreset.Power),
            nameof(EqualizerPreset.Rock),
            nameof(EqualizerPreset.Treble),
            nameof(EqualizerPreset.Vocal),
            nameof(EqualizerPreset.Xplode1),
            nameof(EqualizerPreset.Xplode2),
            nameof(EqualizerPreset.Xplode3),
        };


        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private bool _enabled;
        private Equalizer _equalizer;
        private EqualizerPreset _preset;


        //  GETTERS & SETTERS

        public bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
                OnEqualizerChange();
                OnPropertyChanged(nameof(Enabled));
            }
        }

        public Equalizer Equalizer
        {
            get => _equalizer;
            set
            {
                _equalizer = value;
                OnEqualizerChange();
                OnPropertyChanged(nameof(Equalizer));
            }
        }

        public EqualizerPreset Preset
        {
            get => _preset;
            set
            {
                if (_preset != null)
                    _preset.PresetValueChanged -= OnPresetComponentChange;

                if (value != null)
                    _preset = value;
                else
                    _preset = EqualizerPreset.Default;

                _preset.PresetValueChanged += OnPresetComponentChange;
                OnEqualizerChange();
                OnPropertyChanged(nameof(Preset));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> EqualizerManager class constructor. </summary>
        public EqualizerManager()
        {
            Preset = EqualizerPreset.Default;
        }

        #endregion CLASS METHODS

        #region LOAD & SAVE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get default equalizer preset by name. </summary>
        /// <param name="presetName"> Default preset name. </param>
        /// <returns> Default preset. </returns>
        private EqualizerPreset GetDefaultPreset(string presetName)
        {
            if (DEFAULT_PRESETS_NAMES.Contains(presetName))
            {
                switch (presetName)
                {
                    case nameof(EqualizerPreset.Bass):
                        return EqualizerPreset.Bass;

                    case nameof(EqualizerPreset.Dance):
                        return EqualizerPreset.Dance;

                    case nameof(EqualizerPreset.Live):
                        return EqualizerPreset.Live;

                    case nameof(EqualizerPreset.Pop):
                        return EqualizerPreset.Pop;

                    case nameof(EqualizerPreset.Power):
                        return EqualizerPreset.Power;

                    case nameof(EqualizerPreset.Rock):
                        return EqualizerPreset.Rock;

                    case nameof(EqualizerPreset.Treble):
                        return EqualizerPreset.Treble;

                    case nameof(EqualizerPreset.Vocal):
                        return EqualizerPreset.Vocal;

                    case nameof(EqualizerPreset.Xplode1):
                        return EqualizerPreset.Xplode1;

                    case nameof(EqualizerPreset.Xplode2):
                        return EqualizerPreset.Xplode2;

                    case nameof(EqualizerPreset.Xplode3):
                        return EqualizerPreset.Xplode3;

                    case nameof(EqualizerPreset.Default):
                    default:
                        return EqualizerPreset.Default;
                }
            }

            return EqualizerPreset.Default;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Load equalizer preset from file or from defaults. </summary>
        /// <param name="presetName"></param>
        private void LoadPreset(string presetName)
        {
            var presetFileExt = FilesManager.GetFileTypeByKind(FileKind.JSON).Extension;
            var presetsPath = FilesManager.Instance.EqualizerStoragePath;
            var presetFilePath = Path.Combine(presetsPath, presetName + presetFileExt);

            if (File.Exists(presetFilePath))
            {
                var serialized = File.ReadAllText(presetFilePath);
                var preset = JsonConvert.DeserializeObject<EqualizerPreset>(serialized);

                if (preset != null)
                {
                    Preset = preset;
                    return;
                }
            }

            Preset = GetDefaultPreset(presetName);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Save current loaded equalizer preset. </summary>
        public void SaveCurrentPreset()
        {
            if (Preset != null)
                SavePreset(Preset.Name);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Save equalizer preset to file. </summary>
        /// <param name="profileName"> Equalizer preset name. </param>
        private void SavePreset(string presetName)
        {
            var presetFileExt = FilesManager.GetFileTypeByKind(FileKind.JSON).Extension;
            var presetsPath = FilesManager.Instance.EqualizerStoragePath;
            var presetFilePath = Path.Combine(presetsPath, presetName + presetFileExt);

            var serialized = JsonConvert.SerializeObject(Preset, Formatting.Indented);
            File.WriteAllText(presetFilePath, serialized);
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

        #region PRESETS MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Create new equalizer preset. </summary>
        public void CreatePreset()
        {
            SaveCurrentPreset();

            string filePrefix = $"{NEW_PRESET_NAME} ";
            int presetNameCounter = 0;
            var presetsWithNewName = GetPresetsList()
                .Select(p => Path.GetFileNameWithoutExtension(p))
                .Where(p => p.StartsWith(filePrefix));

            if (presetsWithNewName.Any())
            {
                presetNameCounter = presetsWithNewName.Select(p =>
                {
                    if (int.TryParse(p.Substring(filePrefix.Length), out int lastIndex))
                        return lastIndex;
                    return 0;
                }).Max();
            }

            Preset.Name = $"{filePrefix}{presetNameCounter + 1}";
            SavePreset(Preset.Name);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Rename current equalizer preset. </summary>
        /// <param name="newPresetName"> New equalizer preset name. </param>
        /// <returns> True - equalizer preset name has been changed; False - otherwise. </returns>
        public bool RenameCurrentPreset(string newPresetName)
        {
            var defaultProfileNames = DEFAULT_PRESETS_NAMES.Select(p => p.ToLower());

            if (!defaultProfileNames.Contains(Preset.Name.ToLower()) && !defaultProfileNames.Contains(newPresetName.ToLower())
                && !GetPresetsList().Any(p => Path.GetFileNameWithoutExtension(p).ToLower() == newPresetName.ToLower()))
            {
                var presetFileExt = FilesManager.GetFileTypeByKind(FileKind.JSON).Extension;
                var presetsPath = FilesManager.Instance.EqualizerStoragePath;
                var currentPresetFilePath = Path.Combine(presetsPath, Preset.Name + presetFileExt);

                if (File.Exists(currentPresetFilePath))
                    File.Delete(currentPresetFilePath);

                Preset.Name = newPresetName;
                SavePreset(newPresetName);
                return true;
            }

            return false;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Remove current loaded equalizer preset. </summary>
        public void RemoveCurrentPreset()
        {
            var presetName = Preset.Name;
            var presetFileExt = FilesManager.GetFileTypeByKind(FileKind.JSON).Extension;
            var presetsPath = FilesManager.Instance.EqualizerStoragePath;
            var profileFilePath = Path.Combine(presetsPath, presetName + presetFileExt);

            if (File.Exists(profileFilePath))
                File.Delete(profileFilePath);

            if (DEFAULT_PRESETS_NAMES.Contains(presetName))
                LoadPreset(presetName);
            else
                LoadPreset(DEFAULT_PRESETS_NAMES.First());
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Reset current preset. </summary>
        public void ResetCurrentPreset()
        {
            var preset = GetDefaultPreset(Preset.Name);

            if (preset.Name != Preset.Name)
                preset.Name = Preset.Name;

            Preset = preset;
            SaveCurrentPreset();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Select equalizer preset. </summary>
        /// <param name="presetName"> Equalizer preset name. </param>
        public void SelectPreset(string presetName)
        {
            SaveCurrentPreset();
            LoadPreset(presetName);
        }

        #endregion PRESETS MANAGEMENT METHODS

        #region STATIC METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get list of presets files paths. </summary>
        /// <returns> List of presets files paths. </returns>
        private static List<string> GetPresetsList()
        {
            var presetFileConfig = FilesManager.GetFileTypesByGroup(FileGroup.SETTINGS);
            var presetsPath = FilesManager.Instance.EqualizerStoragePath;
            var presets = FilesManager.GetFilesList(presetsPath, presetFileConfig);

            return presets;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get list of presets that will be visible in interface. </summary>
        /// <returns> List of presets visible in interface. </returns>
        public static List<string> GetPresetsNamesList()
        {
            var presets = GetPresetsList();
            var result = new List<string>();

            result.AddRange(DEFAULT_PRESETS_NAMES);

            result.AddRange(presets
                .Select(p => Path.GetFileNameWithoutExtension(p))
                .Where(p => !DEFAULT_PRESETS_NAMES.Contains(p)));

            return result;
        }

        #endregion STATIC METHODS

        #region SETUP METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Apply equalizer configuration from configurationManager. </summary>
        /// <param name="configManager"></param>
        public void ApplyConfiguration(ConfigManager configManager)
        {
            Enabled = configManager.EqualizerEnabled;

            if (Preset.Name != configManager.EqualizerPresetName)
                SelectPreset(configManager.EqualizerPresetName);
        }

        #endregion SETUP METHODS

        #region UPDATE PRESET METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Update equalizer with preset data. </summary>
        private void OnEqualizerChange()
        {
            if (_equalizer != null && _preset != null)
            {
                for (int i = 0; i < 10; i++)
                {
                    EqualizerFilter filter = _equalizer.SampleFilters[i];
                    filter.AverageGainDB = _enabled ? Preset.GetValue(i) : 0;
                }
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update equalizer with single modified data. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Equalizer Preset Value Changed Event Arguments. </param>
        private void OnPresetComponentChange(object sender, EqualizerPresetValueChangedEventArgs e)
        {
            if (_equalizer != null && _preset != null)
            {
                EqualizerFilter filter = _equalizer.SampleFilters[e.Index];
                filter.AverageGainDB = _enabled ? e.Value : 0;
                SaveCurrentPreset();
            }
        }

        #endregion UPDATE PRESET METHODS

    }
}
