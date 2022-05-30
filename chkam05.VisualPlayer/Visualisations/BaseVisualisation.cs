using chkam05.VisualPlayer.Utilities;
using chkam05.VisualPlayer.Visualisations.Data;
using chkam05.VisualPlayer.Visualisations.Spectrum;
using CSCore;
using CSCore.DSP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace chkam05.VisualPlayer.Visualisations
{
    public class BaseVisualisation : IVisualisation
    {

        //  CONST

        private const double MAX_DB_VALUE = 0;
        private const double MIN_DB_VALUE = -90;
        private const double DB_SCALE = (MAX_DB_VALUE - MIN_DB_VALUE);
        private const int SCALE_FACTOR_LINEAR = 9;
        private const int SCALE_FACTOR_SQR = 2;


        //  VARIABLES

        private BeatLevel _beatLevel;
        private Size _drawingAreaSize = new Size(0, 0);
        internal int _fftSize;
        internal bool _initialized = false;
        private Thickness _margin = new Thickness(0);
        private ScalingStrategy _scalingStrategy = ScalingStrategy.SQRT;
        internal ISpectrumProvider _spectrumProvider;

        private int _minFrequency = 20;
        private int _maxFrequency = 20000;
        private int _spectrumSize = 64;

        private int _maxFFTIndex;
        private int _maxFrequencyIndex;
        private int _minFrequencyIndex;

        private int[] _spectrumMaxIndex;
        private int[] _spectrumMaxLogScaleIndex;


        //  GETTERS & SETTERS

        public BeatLevel BeatLevel
        {
            get => _beatLevel;
        }

        public Size DrawingAreaSize
        {
            get => _drawingAreaSize;
            set
            {
                _drawingAreaSize = value;
                PreCalculate();
                UpdateGraphics();
            }
        }

        public FftSize FFTSize
        {
            get => (FftSize)_fftSize;
            set
            {
                if ((int)Math.Log((int)value, 2) % 1 != 0)
                    throw new ArgumentException($"Invalid FFTSize value");

                _fftSize = (int)value;
                _maxFFTIndex = (_fftSize / 2) - 1;
            }
        }

        public bool Initialized
        {
            get => _initialized;
        }

        public ISpectrumProvider ISpectrumProvider
        {
            get => _spectrumProvider;
        }

        public Thickness Margin
        {
            get => _margin;
            set => _margin = new Thickness(
                Math.Max(0, value.Left), Math.Max(0, value.Top), Math.Max(0, value.Right), Math.Max(0, value.Bottom));
        }

        public int MaxFrequency
        {
            get => _maxFrequency;
            set => _maxFrequency = Math.Max(0, value);
        }

        public int MinFrequency
        {
            get => _minFrequency;
            set => _minFrequency = Math.Max(0, value);
        }

        public PropertyInfo[] Properties
        {
            get => ObjectUtilities.GetObjectProperties(this.GetType());
        }

        public ScalingStrategy ScalingStrategy
        {
            get => _scalingStrategy;
            set
            {
                _scalingStrategy = value;

                if (_initialized)
                    MapFrequency();
            }
        }

        public int SpectrumSize
        {
            get => _spectrumSize;
            set
            {
                _spectrumSize = Math.Max(0, value);

                if (_initialized)
                    MapFrequency();
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> BaseVisualisation class constructor. </summary>
        /// <param name="spectrumProvider"> Spectrum provider with FFT data. </param>
        public BaseVisualisation(SpectrumProvider spectrumProvider = null)
        {
            SetupSpectrumProvider(spectrumProvider, true);
            _beatLevel = new BeatLevel(0, _spectrumSize);
        }

        #endregion CLASS METHODS

        #region CALCULATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Pre calculate visualisation graphics before drawing it. </summary>
        public virtual void PreCalculate()
        {
            //
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Map frequency. </summary>
        private void MapFrequency()
        {
            if (_spectrumProvider != null)
            {
                _maxFrequencyIndex = Math.Min(_spectrumProvider.GetFFTBandIndex(MaxFrequency) + 1, _maxFFTIndex);
                _minFrequencyIndex = Math.Min(_spectrumProvider.GetFFTBandIndex(MinFrequency), _maxFFTIndex);

                int frequencyIndexCount = _maxFrequencyIndex - _minFrequencyIndex;
                double linearIndexBucketSize = Math.Round(frequencyIndexCount / (double)_spectrumSize, 3);

                _spectrumMaxIndex = _spectrumMaxIndex.CheckBuffer(_spectrumSize, true);
                _spectrumMaxLogScaleIndex = _spectrumMaxLogScaleIndex.CheckBuffer(_spectrumSize, true);

                double maxLog = Math.Log(_spectrumSize, _spectrumSize);

                for (int i = 1; i < _spectrumSize; i++)
                {
                    int logIndex = (int)
                        ((maxLog - Math.Log((_spectrumSize + 1) - i, (_spectrumSize + 1))) * frequencyIndexCount)
                        + _minFrequencyIndex;

                    _spectrumMaxIndex[i - 1] = _minFrequencyIndex + (int)(i * linearIndexBucketSize);
                    _spectrumMaxLogScaleIndex[i - 1] = logIndex;
                }

                if (_spectrumSize > 0)
                {
                    _spectrumMaxIndex[_spectrumMaxIndex.Length - 1] = _maxFrequencyIndex;
                    _spectrumMaxLogScaleIndex[_spectrumMaxLogScaleIndex.Length - 1] = _maxFrequencyIndex;
                }

                _initialized = true;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get spectrum data. </summary>
        /// <param name="maxValue"> Max value of spectrum point that can be handled. </param>
        /// <param name="fftBuffer"> FFT data buffer. </param>
        /// <param name="scalingStrategy"> Scaling strategy. </param>
        /// <param name="useLogScale"> Scale spectrum using Log. </param>
        /// <param name="useAverage"> Average spectrum points. </param>
        /// <returns> Spectrum data as list of points. </returns>
        internal List<SpectrumLevel> GetSpectrum(double maxValue, float[] fftBuffer,
            ScalingStrategy scalingStrategy = ScalingStrategy.LINEAR, bool useLogScale = false, bool useAverage = false)
        {
            var positions = new List<SpectrumLevel>();

            double firstValue = 0;
            double lastValue = 0;
            double value = 0;

            double localMaxValue = maxValue;
            int positionIndex = 0;
            bool recalculate = true;

            for (int i = _minFrequencyIndex; i <= _maxFrequencyIndex; i++)
            {
                switch (scalingStrategy)
                {
                    case ScalingStrategy.DB:
                        firstValue = (20 * Math.Log10(fftBuffer[i] - MIN_DB_VALUE) / DB_SCALE) * localMaxValue;
                        break;

                    case ScalingStrategy.LINEAR:
                        firstValue = (fftBuffer[i] * SCALE_FACTOR_LINEAR) * localMaxValue;
                        break;

                    case ScalingStrategy.SQRT:
                        firstValue = (Math.Sqrt(fftBuffer[i]) * SCALE_FACTOR_SQR) * localMaxValue;
                        break;
                }

                recalculate = true;
                value = Math.Max(0, Math.Max(firstValue, value));

                while (GetSpectrumFreqCondition(i, positionIndex, useLogScale))
                {
                    if (!recalculate) { value = lastValue; }
                    if (value > maxValue) { value = maxValue; }
                    if (useAverage && positionIndex > 0) { value = (lastValue + value) / 2.0; }

                    var spectrumPosition = new SpectrumLevel
                    {
                        Index = positionIndex,
                        Value = value
                    };

                    positions.Add(spectrumPosition);
                    lastValue = value;
                    positionIndex++;
                    recalculate = false;
                    value = 0.0;
                }
            }

            return positions;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> While condition in get spectrum data method. </summary>
        /// <param name="frequencyIndex"> Index of frequency between spectrum points. </param>
        /// <param name="positionIndex"> Spectrum point index. </param>
        /// <param name="useLogScale"> Scale spectrum using Log. </param>
        /// <returns> T/F. </returns>
        private bool GetSpectrumFreqCondition(int frequencyIndex, int positionIndex, bool useLogScale)
        {
            if (positionIndex <= _spectrumMaxIndex.Length - 1)
            {
                int spectrumMax = useLogScale
                    ? _spectrumMaxLogScaleIndex[positionIndex]
                    : _spectrumMaxIndex[positionIndex];

                return frequencyIndex == spectrumMax;
            }

            return false;
        }

        #endregion CALCULATION METHODS

        #region DRAWING METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Draw visualisation. </summary>
        /// <returns> Bitmap drawer. </returns>
        public virtual BitmapDrawer Draw()
        {
            return null;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Stop drawing visualisation. </summary>
        /// <returns> Bitmap drawer. </returns>
        public virtual BitmapDrawer StopDrawing()
        {
            return null;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update graphics configuration. </summary>
        public virtual void UpdateGraphics()
        {
            //
        }

        #endregion DRAWING METHODS

        #region GET METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get beat level value. </summary>
        /// <returns> Beat level value. </returns>
        public virtual double GetBeatLevel() => 0.0;

        #endregion GET METHODS

        #region SETUP METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Set visualisation property. </summary>
        /// <param name="propertyName"> Property name. </param>
        /// <param name="value"> Value to set. </param>
        /// <returns> True - value set; False - otherwise. </returns>
        public bool SetProperty(string propertyName, object value)
        {
            var property = Properties.FirstOrDefault(p => p.Name == propertyName);

            if (property != null && property.PropertyType == value.GetType())
            {
                property.SetValue(this, value);
                return true;
            }

            return false;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Setup spectrum provider. </summary>
        /// <param name="spectrumProvider"> Spectrum provider. </param>
        /// <param name="remap"> Remap frequency. </param>
        public void SetupSpectrumProvider(SpectrumProvider spectrumProvider, bool remap = false)
        {
            if (spectrumProvider != null)
            {
                _spectrumProvider = spectrumProvider;
                FFTSize = spectrumProvider.FftSize;

                if (_initialized || remap)
                    MapFrequency();
            }
        }

        #endregion SETUP METHODS

    }
}
