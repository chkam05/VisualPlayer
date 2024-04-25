using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Core.SoundTouch
{
    public interface ISoundTouch : IDisposable
    {

        //  METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Clear. </summary>
        void Clear();

        //  --------------------------------------------------------------------------------
        /// <summary> Flush. </summary>
        void Flush();

        // --------------------------------------------------------------------------------
        /// <summary> Get number of samples. </summary>
        /// <returns> Number of samples. </returns>
        int NumberOfSamples();

        //  --------------------------------------------------------------------------------
        /// <summary> Put samples. </summary>
        /// <param name="samples"> Samples array. </param>
        /// <param name="numberOfSamples"> Number of samples. </param>
        void PutSamples(float[] samples, int numberOfSamples);

        //  --------------------------------------------------------------------------------
        /// <summary> Receive samples. </summary>
        /// <param name="outBuffer"> Output buffer array. </param>
        /// <param name="maxSamples"> Max number of samples. </param>
        /// <returns></returns>
        int ReceiveSamples(float[] outBuffer, int maxSamples);

        //  --------------------------------------------------------------------------------
        /// <summary> Set channels. </summary>
        /// <param name="numberOfChannels"> Number of channels. </param>
        void SetChannels(int numberOfChannels);

        //  --------------------------------------------------------------------------------
        /// <summary> Set pitch semi tones. </summary>
        /// <param name="newPitch"> Pitch value. </param>
        void SetPitchSemiTones(float newPitch);

        //  --------------------------------------------------------------------------------
        /// <summary> Set sample rate. </summary>
        /// <param name="sampleRate"> Sample rate. </param>
        void SetSampleRate(int sampleRate);

        //  --------------------------------------------------------------------------------
        /// <summary> Set tempo change. </summary>
        /// <param name="newTempo"> Tempo value. </param>
        void SetTempoChange(float newTempo);

    }
}
