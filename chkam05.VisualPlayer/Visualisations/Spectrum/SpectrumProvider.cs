using CSCore.DSP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Visualisations.Spectrum
{
    public class SpectrumProvider : FftProvider, ISpectrumProvider
    {

        //  VARIABLES

        private List<object> _contexts = new List<object>();
        private int _sampleRate;


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> SpectrumProvider class constructor. </summary>
        /// <param name="channels"> Channels count. </param>
        /// <param name="sampleRate"> Sample rate. </param>
        /// <param name="fftSize"> FFT data size constans used for FFT calculations. </param>
        public SpectrumProvider(int channels, int sampleRate, FftSize fftSize) : base(channels, fftSize)
        {
            if (sampleRate <= 0)
                throw new ArgumentOutOfRangeException($"{nameof(sampleRate)} cannot be equal or less than zero");

            _sampleRate = sampleRate;
        }

        #endregion CLASS METHODS

        #region FFT DATA GET METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get FFT band index. </summary>
        /// <param name="frequency"> Frequency. </param>
        /// <returns> Index of band with specified frequency. </returns>
        public int GetFFTBandIndex(float frequency)
        {
            int fftSize = (int)FftSize;
            double singleChannel = _sampleRate / 2.0;
            double bandIndex = (frequency / singleChannel) * (fftSize / 2);

            return (int)bandIndex;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get FFT data. </summary>
        /// <param name="fftResultBuffer"> Result buffer of FFT data. </param>
        /// <param name="context"> Context. </param>
        /// <returns> True - new data gets; False - otherwise. </returns>
        public bool GetFFTData(float[] fftResultBuffer, object context)
        {
            if (_contexts.Contains(context))
                return false;

            _contexts.Add(context);
            GetFftData(fftResultBuffer);
            return true;
        }

        #endregion FFT DATA GET METHODS

        #region FFT PROVIDER METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Adds multiple samples to the CSCore.DSP.FftProvider. </summary>
        /// <param name="samples"> Float Array which contains samples. </param>
        /// <param name="size"> Number of samples to add to the CSCore.DSP.FftProvider. </param>
        public override void Add(float[] samples, int size)
        {
            base.Add(samples, size);

            if (size > 0)
                _contexts.Clear();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Adds a left and a right sample to the CSCore.DSP.FftProvider. </summary>
        /// <param name="leftSample"> The sample of the left channel. </param>
        /// <param name="rightSample"> The sample of the right channel. </param>
        public override void Add(float leftSample, float rightSample)
        {
            //  The left and the right sample will be merged together.
            base.Add(leftSample, rightSample);

            _contexts.Clear();
        }

        #endregion FFT PROVIDER METHODS

    }
}
