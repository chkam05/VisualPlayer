using chkam05.Visualisations.Spectrum;
using CSCore;
using CSCore.DSP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace chkam05.Visualisations.Base
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

        internal Canvas _canvas;
        internal bool _enabled;
        internal int _fftSize;
        internal ISpectrumProvider _spectrumProvider;

        private int _minFrequency = 20;
        private int _maxFrequency = 20000;
        private int _spectrumSize = 64;

        private int _maxFFTIndex;
        private int _maxFrequencyIndex;
        private int _minFrequencyIndex;

        private int[] _spectrumMaxIndex;
        private int[] _spectrumMaxLogScaleIndex;


        #region GETTERS & SETTERS

        public Canvas Canvas
        {
            get => _canvas;
        }

        public bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
                Stop();
            }
        }

        public double CanvasHeight
        {
            get => _canvas.ActualHeight;
        }

        public double CanvasWidth
        {
            get => _canvas.ActualWidth;
        }

        public ISpectrumProvider SpectrumProvider
        {
            get => _spectrumProvider;
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

        public int SpectrumSize
        {
            get => _spectrumSize;
            set => _spectrumSize = Math.Max(0, value);
        }

        #endregion GETTERS & SETTERS


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> BaseVisualisation class constructor. </summary>
        /// <param name="canvas"> Canvas where visualisation will be presented. </param>
        /// <param name="spectrumProvider"> Spectrum provider with FFT data. </param>
        public BaseVisualisation(Canvas canvas, SpectrumProvider spectrumProvider = null)
        {
            _canvas = canvas;
            SetSpectrumProvider(spectrumProvider, false);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Cleanup and free all used resources and variables. </summary>
        public virtual void Dispose()
        {
            //
        }

        #endregion CLASS METHODS

        #region DRAW METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Draw visualisation on canvas. </summary>
        public virtual void Draw()
        {
            //
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Stop visualisation working. </summary>
        public virtual void Stop()
        {
            //
        }

        #endregion DRAW METHODS

        #region MAPPING METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Map frequency. </summary>
        private void MapFrequency()
        {
            if (_spectrumProvider != null)
            {
                _maxFrequencyIndex = Math.Min(_spectrumProvider.GetFFTBandIndex(MaxFrequency) + 1, _maxFFTIndex);
                _minFrequencyIndex = Math.Min(_spectrumProvider.GetFFTBandIndex(MinFrequency), _maxFFTIndex);

                int frequencyIndexCount = _maxFrequencyIndex - _minFrequencyIndex;
                double linearIndexBucketSize = Math.Round(frequencyIndexCount / (double) _spectrumSize, 3);

                _spectrumMaxIndex = _spectrumMaxIndex.CheckBuffer(_spectrumSize, true);
                _spectrumMaxLogScaleIndex = _spectrumMaxLogScaleIndex.CheckBuffer(_spectrumSize, true);

                double maxLog = Math.Log(_spectrumSize, _spectrumSize);

                for (int i = 1; i <_spectrumSize; i++)
                {
                    int logIndex = (int)
                        ((maxLog - Math.Log((_spectrumSize + 1) - i, (_spectrumSize + 1))) * frequencyIndexCount)
                        + _minFrequencyIndex;

                    _spectrumMaxIndex[i - 1] = _minFrequencyIndex + (int) (i * linearIndexBucketSize);
                    _spectrumMaxLogScaleIndex[i - 1] = logIndex;
                }

                if (_spectrumSize > 0)
                {
                    _spectrumMaxIndex[_spectrumMaxIndex.Length - 1] = _maxFrequencyIndex;
                    _spectrumMaxLogScaleIndex[_spectrumMaxLogScaleIndex.Length - 1] = _maxFrequencyIndex;
                }
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
        internal List<SpectrumPosition> GetSpectrum(double maxValue, float[] fftBuffer, 
            ScalingStrategy scalingStrategy = ScalingStrategy.LINEAR, bool useLogScale = false, bool useAverage = false)
        {
            var positions = new List<SpectrumPosition>();

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

                    var spectrumPosition = new SpectrumPosition
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

        #endregion MAPPING METHODS

        #region SETUP METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Set spectrum provider. </summary>
        /// <param name="spectrumProvider"> Spectrum provider with FFT data. </param>
        /// <param name="recalculate"> Remap frequency. </param>
        public void SetSpectrumProvider(SpectrumProvider spectrumProvider, bool recalculate = true)
        {
            if (spectrumProvider != null)
            {
                _spectrumProvider = spectrumProvider;
                FFTSize = spectrumProvider.FftSize;

                if (recalculate)
                    MapFrequency();
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set new canvas. </summary>
        /// <param name="canvas"> Canvas where visualisation will be presented. </param>
        /// <param name="recalculate"> Remap frequency. </param>
        public virtual void UpdateGraphics(Canvas canvas = null, bool recalculate = true)
        {
            if (canvas != null)
                _canvas = canvas;

            if (recalculate)
                MapFrequency();
        }

        #endregion SETUP METHODS

    }
}
