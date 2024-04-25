using chkam05.VisualPlayer.Core.BiQuad;
using chkam05.VisualPlayer.Core.Events;
using chkam05.VisualPlayer.Core.SoundTouch;
using CSCore.DSP;
using CSCore.Streams.Effects;
using CSCore.XAudio2;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Core
{
    public class EffectsManager : INotifyPropertyChanged
    {

        //  CONST

        public const double BAND_WIDTH_VALUE_MAX = 128;
        public const double BAND_WIDTH_VALUE_MIN = 1;
        public const double FREQUENCY_VALUE_MAX = 20000;
        public const double FREQUENCY_VALUE_MIN = 20;
        public const double GAIN_DB_VALUE_MAX = 100;
        public const double GAIN_DB_VALUE_MIN = -100;
        public const double PITCH_VALUE_MAX = 100.0;
        public const double PITCH_VALUE_MIN = -100.0;


        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private BiQuadFilterSource _biQuadFilterSource;
        private BiQuadFilterType _biQuadFilterType;
        private PitchShifter _pitchShifter;
        private double _bandWidth = 1;
        private double _frequency = 20;
        private double _gainDB = 0;
        private int _pitch;


        //  GETTERS & SETTERS

        public BiQuadFilterSource BiQuadFilterSource
        {
            get => _biQuadFilterSource;
            set
            {
                _biQuadFilterSource = value;
                UpdateBiQuadFilterSource();
                OnPropertyChanged(nameof(BiQuadFilterSource));
            }
        }

        public BiQuadFilterType BiQuadFilterType
        {
            get => _biQuadFilterType;
            set
            {
                _biQuadFilterType = value;
                UpdateBiQuadFilterType();
                OnPropertyChanged(nameof(BiQuadFilterType));
            }
        }

        public double BandWidth
        {
            get => _bandWidth;
            set
            {
                _bandWidth = value;
                UpdateFilterBandWidth();
                OnPropertyChanged(nameof(BandWidth));
            }
        }

        public bool BandWidthEnabled
        {
            get => HasFilterProperty(_biQuadFilterSource?.Filter, "BandWidth");
        }

        public double Frequency
        {
            get => _frequency;
            set
            {
                _frequency = value;
                UpdateFilterFrequency();
                OnPropertyChanged(nameof(Frequency));
            }
        }

        public bool FrequencyEnabled
        {
            get => HasFilterProperty(_biQuadFilterSource?.Filter, "Frequency");
        }

        public double GainDB
        {
            get => _gainDB;
            set
            {
                _gainDB = value;
                UpdateFilterGainDB();
                OnPropertyChanged(nameof(GainDB));
            }
        }

        public bool GainDBEnabled
        {
            get
            {
                var filter = _biQuadFilterSource?.Filter;

                return IsFilterTypeOf(filter, typeof(HighShelfFilter)) 
                    || IsFilterTypeOf(filter, typeof(LowShelfFilter))
                    || IsFilterTypeOf(filter, typeof(PeakFilter));
            }
        }

        public PitchShifter PitchShifter
        {
            get => _pitchShifter;
            set
            {
                _pitchShifter = value;
                UpdatePitchShifter();
                OnPropertyChanged(nameof(PitchShifter));
            }
        }

        public int Pitch
        {
            get => _pitch;
            set
            {
                _pitch = (int) Math.Min(Math.Max(value, PITCH_VALUE_MIN), PITCH_VALUE_MAX);
                UpdatePitch();
                OnPropertyChanged(nameof(Pitch));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> EffectsManager class constructor. </summary>
        public EffectsManager()
        {
            
        }

        #endregion CLASS METHODS

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

        #region UPDATE PRESET METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Update BiQuadFilterType. </summary>
        private void UpdateBiQuadFilterSource()
        {
            UpdateBiQuadFilterType();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update BiQuadFilterType. </summary>
        private void UpdateBiQuadFilterType()
        {
            if (_biQuadFilterSource != null)
            {
                var sampleRate = _biQuadFilterSource.WaveFormat.SampleRate;

                switch (_biQuadFilterType)
                {
                    case BiQuadFilterType.None:
                        _biQuadFilterSource.Filter = null;
                        break;

                    case BiQuadFilterType.BandpassFilter:
                        _biQuadFilterSource.Filter = new BandpassFilter(sampleRate, _frequency);
                        break;

                    case BiQuadFilterType.HighpassFilter:
                        _biQuadFilterSource.Filter = new HighpassFilter(sampleRate, _frequency);
                        break;

                    case BiQuadFilterType.HighShelfFilter:
                        _biQuadFilterSource.Filter = new HighShelfFilter(sampleRate, _frequency, _gainDB);
                        break;

                    case BiQuadFilterType.LowpassFilter:
                        _biQuadFilterSource.Filter = new LowpassFilter(sampleRate, _frequency);
                        break;

                    case BiQuadFilterType.LowShelfFilter:
                        _biQuadFilterSource.Filter = new LowShelfFilter(sampleRate, _frequency, _gainDB);
                        break;

                    case BiQuadFilterType.NotchFilter:
                        _biQuadFilterSource.Filter = new NotchFilter(sampleRate, _frequency);
                        break;

                    case BiQuadFilterType.PeakFilter:
                        _biQuadFilterSource.Filter = new PeakFilter(sampleRate, _frequency, _bandWidth, _gainDB);
                        break;
                }
            }

            UpdateFilterPropertiesAvailability();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update band width value. </summary>
        private void UpdateFilterBandWidth()
        {
            SetFilterProperty(_biQuadFilterSource?.Filter, "BandWidth", _bandWidth);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update frequency value. </summary>
        private void UpdateFilterFrequency()
        {
            SetFilterProperty(_biQuadFilterSource?.Filter, "Frequency", _frequency);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update gain DB value. </summary>
        private void UpdateFilterGainDB()
        {
            SetFilterProperty(_biQuadFilterSource?.Filter, "GainDB", _gainDB);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if BiQuad filter have property. </summary>
        /// <param name="filter"> BiQuad filter. </param>
        /// <param name="propertyName"> BiQuad filter property name. </param>
        /// <returns> True - filter contains property; False - otherwsie. </returns>
        private bool HasFilterProperty(CSCore.DSP.BiQuad filter, string propertyName)
        {
            return filter != null 
                ? filter.GetType().GetProperties().Any(p => p.Name == propertyName)
                : false;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if BiQuad filter is type of. </summary>
        /// <param name="filter"> BiQuad filter. </param>
        /// <param name="filterType"> Type of BiQuad filter. </param>
        /// <returns> True - filter is type of; False - otherwise. </returns>
        private bool IsFilterTypeOf(CSCore.DSP.BiQuad filter, Type filterType)
        {
            return filter != null
                ? filter.GetType() == filterType
                : false;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get BiQuad filter property value. </summary>
        /// <param name="filter"> BiQuad filter. </param>
        /// <param name="propertyName"> BiQuad filter property name. </param>
        /// <param name="value"> Value to set. </param>
        private void SetFilterProperty(CSCore.DSP.BiQuad filter, string propertyName, double value)
        {
            if (filter != null)
            {
                var filterType = filter.GetType();

                if (filterType.GetProperties().Any(p => p.Name == propertyName))
                    filterType.GetProperty(propertyName).SetValue(filter, value);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update filter availability properties. </summary>
        private void UpdateFilterPropertiesAvailability()
        {
            OnPropertyChanged(nameof(BandWidthEnabled));
            OnPropertyChanged(nameof(FrequencyEnabled));
            OnPropertyChanged(nameof(GainDBEnabled));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update pitch shifter. </summary>
        private void UpdatePitchShifter()
        {
            UpdatePitch();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update pitch value. </summary>
        private void UpdatePitch()
        {
            if (_pitchShifter != null)
                _pitchShifter.PitchShiftFactor = (float)Math.Pow(2, _pitch / 120.0);
        }

        #endregion UPDATE PRESET METHODS

    }
}
