using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Core.SoundTouch
{
    public class SoundTouchBase : ISoundTouch
    {

        //  CONST

        private const string SOUND_TOUCH_DLL = "SoundTouch.dll";


        //  DLL IMPORT

        [DllImport(SOUND_TOUCH_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "soundtouch_createInstance")]
        private static extern IntPtr CreateInstance();

        [DllImport(SOUND_TOUCH_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "soundtouch_destroyInstance")]
        private static extern void DestroyInstance(IntPtr handle);

        [DllImport(SOUND_TOUCH_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "soundtouch_setTempoChange")]
        private static extern void SetTempoChange(IntPtr handle, float newTempo);

        [DllImport(SOUND_TOUCH_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "soundtouch_setPitch")]
        private static extern void SetPitch(IntPtr handle, float newPitch);

        [DllImport(SOUND_TOUCH_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "soundtouch_setPitchSemiTones")]
        private static extern void SetPitchSemiTones(IntPtr handle, float newPitch);

        [DllImport(SOUND_TOUCH_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "soundtouch_setChannels")]
        private static extern void SetChannels(IntPtr handle, uint numChannels);

        [DllImport(SOUND_TOUCH_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "soundtouch_setSampleRate")]
        private static extern void SetSampleRate(IntPtr handle, uint srate);

        [DllImport(SOUND_TOUCH_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "soundtouch_flush")]
        private static extern void Flush(IntPtr handle);

        [DllImport(SOUND_TOUCH_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "soundtouch_putSamples")]
        private static extern void PutSamples(IntPtr handle, float[] samples, uint numSamples);

        [DllImport(SOUND_TOUCH_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "soundtouch_receiveSamples")]
        private static extern uint ReceiveSamples(IntPtr handle, float[] outBuffer, uint maxSamples);

        [DllImport(SOUND_TOUCH_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "soundtouch_numSamples")]
        private static extern uint NumberOfSamples(IntPtr handle);

        [DllImport(SOUND_TOUCH_DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "soundtouch_clear")]
        private static extern uint Clear(IntPtr handle);


        //  VARIABLES

        private bool _isDisposed;
        private IntPtr _soundTouchHandle;


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> SoundTouchBase class constructor. </summary>
        public SoundTouchBase()
        {
            _soundTouchHandle = CreateInstance();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Performs application-defined tasks associated with freeing, releasing, 
        /// or resetting unmanaged resources. </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Performs application-defined tasks associated with freeing, releasing, 
        /// or resetting unmanaged resources. </summary>
        /// <param name="isDisposing"> Is disposing. </param>
        protected virtual void Dispose(bool isDisposing)
        {
            if (_isDisposed)
            {
                return;
            }

            if (isDisposing)
            {
                DestroyInstance(_soundTouchHandle);
                _soundTouchHandle = IntPtr.Zero;
            }

            _isDisposed = true;
        }

        #endregion CLASS METHODS

        #region LIBRARY METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Clear. </summary>
        public void Clear()
        {
            Clear(_soundTouchHandle);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Flush. </summary>
        public void Flush()
        {
            Flush(_soundTouchHandle);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get number of samples. </summary>
        /// <returns> Number of samples. </returns>
        public int NumberOfSamples()
        {
            return (int)NumberOfSamples(_soundTouchHandle);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Put samples. </summary>
        /// <param name="samples"> Samples array. </param>
        /// <param name="numberOfSamples"> Number of samples. </param>
        public void PutSamples(float[] samples, int numberOfSamples)
        {
            PutSamples(_soundTouchHandle, samples, (uint)numberOfSamples);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Receive samples. </summary>
        /// <param name="outBuffer"> Output buffer array. </param>
        /// <param name="maxSamples"> Max number of samples. </param>
        /// <returns></returns>
        public int ReceiveSamples(float[] outBuffer, int maxSamples)
        {
            return (int)ReceiveSamples(_soundTouchHandle, outBuffer, (uint)maxSamples);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set channels. </summary>
        /// <param name="numberOfChannels"> Number of channels. </param>
        public void SetChannels(int numberOfChannels)
        {
            SetChannels(_soundTouchHandle, (uint)numberOfChannels);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set pitch semi tones. </summary>
        /// <param name="newPitch"> Pitch value. </param>
        public void SetPitchSemiTones(float newPitch)
        {
            SetPitchSemiTones(_soundTouchHandle, newPitch);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set sample rate. </summary>
        /// <param name="sampleRate"> Sample rate. </param>
        public void SetSampleRate(int sampleRate)
        {
            SetSampleRate(_soundTouchHandle, (uint)sampleRate);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set tempo change. </summary>
        /// <param name="newTempo"> Tempo value. </param>
        public void SetTempoChange(float newTempo)
        {
            SetTempoChange(_soundTouchHandle, newTempo);
        }

        #endregion LIBRARY METHODS

    }
}
