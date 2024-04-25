using CSCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Core.SoundTouch
{
    public class SoundTouchSource : SampleAggregatorBase
    {

        //  CONST

        public const double PITCH_VALUE_MAX = 12.0;
        public const double PITCH_VALUE_MIN = -12.0;
        public const double TEMPO_VALUE_MAX = 52.0;
        public const double TEMPO_VALUE_MIN = -52.0;


        //  VARIABLES

        private bool _isDisposed;

        private readonly int _latency;
        private readonly float[] _sourceReadBuffer;
        private readonly float[] _soundTouchReadBuffer;
        private readonly object _lockObject;

        private bool _seekRequested;

        private ISampleSource _sampleSource;
        private ISoundTouch _soundTouch;


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> SoundTouchSource class constructor. </summary>
        /// <param name="sampleSource"> Sample source interface. </param>
        /// <param name="latency"> Latency. </param>
        public SoundTouchSource(ISampleSource sampleSource, int latency) 
            : this(sampleSource, latency, new SoundTouchBase())
        {
        }

        //  --------------------------------------------------------------------------------
        /// <summary> SoundTouchSource class constructor. </summary>
        /// <param name="sampleSource"> Sample source interface. </param>
        /// <param name="latency"> Latency. </param>
        /// <param name="soundTouch"> SoundTouch interface. </param>
        public SoundTouchSource(ISampleSource sampleSource, int latency, ISoundTouch soundTouch)
            : base(sampleSource)
        {
            _sampleSource = sampleSource;
            _latency = latency;
            _soundTouch = soundTouch;

            _soundTouch.SetChannels(_sampleSource.WaveFormat.Channels);
            _soundTouch.SetSampleRate(_sampleSource.WaveFormat.SampleRate);

            _sourceReadBuffer = new float[(_sampleSource.WaveFormat.SampleRate * _sampleSource.WaveFormat.Channels * (long)_latency) / 1000];
            _soundTouchReadBuffer = new float[_sourceReadBuffer.Length * 10];

            _lockObject = new object();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Performs application-defined tasks associated with freeing, releasing, 
        /// or resetting unmanaged resources. </summary>
        public new void Dispose()
        {
            base.Dispose();

            Dispose(true);
            GC.SuppressFinalize(this);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Performs application-defined tasks associated with freeing, releasing, 
        /// or resetting unmanaged resources. </summary>
        /// <param name="isDisposing"> Is currently disposing. </param>
        protected virtual new void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            if (_isDisposed)
            {
                return;
            }

            if (isDisposing)
            {
                if (_sampleSource != null)
                {
                    _sampleSource.Dispose();
                    _sampleSource = null;
                }

                if (_soundTouch != null)
                {
                    _soundTouch.Dispose();
                    _soundTouch = null;
                }
            }

            _isDisposed = true;
        }

        #endregion CLASS METHODS

        #region BUFFER WORKFLOW METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Read sample from buffer. </summary>
        /// <param name="buffer"> Buffer array. </param>
        /// <param name="offset"> Offset in bufer. </param>
        /// <param name="count"> Buffer length to read. </param>
        /// <returns> Count of samples readed. </returns>
        public override int Read(float[] buffer, int offset, int count)
        {
            lock (_lockObject)
            {
                if (_seekRequested)
                {
                    _soundTouch.Clear();
                    _seekRequested = false;
                }

                var samplesRead = 0;
                var endOfSource = false;

                while (samplesRead < count)
                {
                    if (_soundTouch.NumberOfSamples() == 0)
                    {
                        var readFromSource = _sampleSource.Read(_sourceReadBuffer, 0, _sourceReadBuffer.Length);
                        if (readFromSource == 0)
                        {
                            endOfSource = true;
                            _soundTouch.Flush();
                        }

                        _soundTouch.PutSamples(_sourceReadBuffer, readFromSource / _sampleSource.WaveFormat.Channels);
                    }

                    var desiredSampleFrames = (count - samplesRead) / _sampleSource.WaveFormat.Channels;
                    var received = _soundTouch.ReceiveSamples(_soundTouchReadBuffer, desiredSampleFrames) * _sampleSource.WaveFormat.Channels;

                    for (int n = 0; n < received; n++)
                    {
                        buffer[offset + samplesRead++] = _soundTouchReadBuffer[n];
                    }

                    if (received == 0 && endOfSource)
                    {
                        break;
                    }
                }

                return samplesRead;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Request seek. </summary>
        public void Seek()
        {
            _seekRequested = true;
        }

        #endregion BUFFER WORKFLOW METHODS

        #region EFFECTS MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Set pitch. </summary>
        /// <param name="pitch"> Pitch value. </param>
        public void SetPitch(float pitch)
        {
            if (pitch > 6.0f || pitch < -6.0f)
            {
                pitch = 0.0f;
            }

            _soundTouch.SetPitchSemiTones(pitch);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set tempo. </summary>
        /// <param name="tempo"> Tempo value. </param>
        public void SetTempo(float tempo)
        {
            if (tempo > 52.0f || tempo < -52.0f)
            {
                tempo = 0.0f;
            }

            _soundTouch.SetTempoChange(tempo);
        }

        #endregion EFFECTS MANAGEMENT METHODS

    }
}
