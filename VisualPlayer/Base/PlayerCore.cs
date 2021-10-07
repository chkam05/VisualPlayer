using chkam05.Visualisations.Spectrum;
using chkam05.VisualPlayer.Data.Files;
using chkam05.VisualPlayer.Data.Lists;
using chkam05.VisualPlayer.Data.States;
using chkam05.VisualPlayer.Utilities.Maths;
using CSCore;
using CSCore.Codecs;
using CSCore.DSP;
using CSCore.SoundOut;
using CSCore.Streams;
using CSCore.Streams.Effects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Base
{
    public class PlayerCore : IDisposable
    {
        
        //  CONST

        private const double BACK_REPEAT_TIME = 3000;


        //  VARIABLES

        private static PlayerCore _instance;

        private PitchShifter _pitchShifter;
        private ISoundOut _soundOut;
        private IWaveSource _source = null;
        private float _volume = 0.5f;

        private double _backRepeatTime = 3000;
        private Permutation _permutation;
        private RepeatMode _repeat = RepeatMode.NO;
        private bool _shuffle = false;


        /// <summary> Auto start playing after selecting file from playlist. </summary>
        public bool AutoPlayAfterLoad { get; set; } = true;

        /// <summary> Auto start playing after adding file to playlist. </summary>
        public bool AutoPlayAfterAdd { get; set; } = false;

        public NowPlaying<SongFile> PlayList { get; private set; }
        public FftSize SpectrumFFTSize { get; private set; } = FftSize.Fft4096;
        public SpectrumProvider SpectrumProvider { get; private set; }


        #region GETTERS & SETTERS

        public double BackRepeatTimeInMiliseconds
        {
            get => _backRepeatTime;
            set => _backRepeatTime = Math.Max(BACK_REPEAT_TIME, value);
        }

        public double BackRepeatTimeInSeconds
        {
            get => _backRepeatTime / 1000;
            set => _backRepeatTime = Math.Max(BACK_REPEAT_TIME, value * 1000);
        }

        [System.ComponentModel.Browsable(false)]
        public bool IsDisposed { get; }

        public bool IsFileLoaded
        {
            get => _soundOut != null && _source != null;
        }

        public bool IsFinishedPlaying
        {
            get => _soundOut != null
                ? TrackPosition.TotalMilliseconds >= TrackLength.TotalMilliseconds
                    && PlaybackState == PlaybackState.Stopped
                : false;
        }

        public PlaybackState PlaybackState
        {
            get => _soundOut != null ? _soundOut.PlaybackState : PlaybackState.Stopped;
        }

        public RepeatMode Repeat
        {
            get => _repeat;
            set => _repeat = value;
        }

        public bool Shuffle
        {
            get => _shuffle;
            set
            {
                _shuffle = value;

                if (_shuffle && (_permutation == null || _permutation.Size != PlayList.Count))
                    _permutation = new Permutation(PlayList.Count);
            }
        }

        public TimeSpan TrackLength
        {
            get => _source != null ? _source.GetLength() : TimeSpan.Zero;
        }

        public TimeSpan TrackPosition
        {
            get => _source != null ? _source.GetPosition() : TimeSpan.Zero;
            set => SetTrackPosition((int) value.TotalSeconds);
        }

        public int Volume
        {
            get => Math.Max(0, Math.Min((int)(_volume * 100), 100));
            set
            {
                _volume = Math.Max(0.0f, Math.Min(value / 100f, 1.0f));

                if (_soundOut != null)
                    _soundOut.Volume = _volume;
            }
        }

        #endregion GETTERS & SETTERS


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> PlayerCore class constructor. </summary>
        private PlayerCore()
        {
            //  Setup playlist.
            PlayList = new NowPlaying<SongFile>();
            SetupPlayListInteraction();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get or create instance of PlayerCore class. </summary>
        public static PlayerCore Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new PlayerCore();

                return _instance;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Cleanup and free all used resources and variables. </summary>
        public void Dispose()
        {
            //  Cleanup playback.
            CleanupPlayback();
        }

        #endregion CLASS METHODS

        #region AUDIO MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Load file to audio device and begin play. </summary>
        /// <param name="filePath"> File path to load and play. </param>
        /// <param name="autoStart"> Auto start playing music after load. </param>
        private void LoadFile(string filePath, bool autoStart = true)
        {
            if (!File.Exists(filePath))
                return;

            PitchShifter pitchShifter;

            //  Load audio file and initialize device with source.
            ISampleSource sampleSource = CodecFactory.Instance.GetCodec(filePath)
                .ToSampleSource()
                .AppendSource(src => new PitchShifter(src), out pitchShifter);

            SpectrumProvider = SetupSpectrumProvider(sampleSource, SpectrumFFTSize);

            var waveSource = SetupSampleSource(sampleSource, SpectrumProvider);
            var soundOutput = SetupAudioDevice(waveSource);

            //  Cleanup playback.
            CleanupPlayback();

            //  Setup source and audio output class variables.
            _pitchShifter = pitchShifter;
            _source = waveSource;
            _soundOut = soundOutput;

            if (autoStart)
            {
                _soundOut.Volume = _volume;
                _soundOut.Play();
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Setup audio stream source and transform it to raw byte data. </summary>
        /// <param name="source"> Audio stream source as samples. </param>
        /// <returns> Raw byte data of audio stream source interface. </returns>
        private IWaveSource SetupSampleSource(ISampleSource source, SpectrumProvider spectrumProvider = null)
        {
            //  Create "SingleBlockNotificationStream" to capture samples.
            var notificationSource = new SingleBlockNotificationStream(source);

            //  Setup SpectrumProvider.
            if (spectrumProvider != null)
            {
                //  Pass the intercepted samples as input data to the spectrumprovider.
                //  (which will calculate a fft based on them)
                notificationSource.SingleBlockRead += (s, a) => spectrumProvider.Add(a.Left, a.Right);
            }

            //  Create WaveForm source.
            return notificationSource.ToWaveSource(16);
        }

        //  --------------------------------------------------------------------------------
        private SpectrumProvider SetupSpectrumProvider(ISampleSource source, FftSize fftSize)
        {
            //  Create and return spectrum provider.
            return new SpectrumProvider(source.WaveFormat.Channels, source.WaveFormat.SampleRate, fftSize);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Setup audio playback device and provide audio data to it. </summary>
        /// <param name="waveSource"> Raw byte data of audio stream source. </param>
        /// <returns> Audio playback device interface. </returns>
        private ISoundOut SetupAudioDevice(IWaveSource waveSource)
        {
            //  Initialize output audio playback device.
            ISoundOut soundOut = new WasapiOut();

            //  Pass WaveForm source into device.
            soundOut.Initialize(waveSource);

            //  Return initialized device handler with loaded source.
            return soundOut;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Stop currently playing music, cleanup device and remove audio data. </summary>
        private void CleanupPlayback()
        {
            //  Disconnect output audio device.
            if (_soundOut != null)
            {
                _soundOut.Stop();
                _soundOut.Dispose();
                _soundOut = null;
            }

            //  Clear WaveForm source.
            if (_source != null)
            {
                _source.Dispose();
                _source = null;
            }
        }

        #endregion AUDIO MANAGEMENT METHODS

        #region INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Change position of current playing audio. </summary>
        /// <param name="positionInSeconds"> New position in seconds of current playing audio. </param>
        public void SetTrackPosition(int positionInSeconds)
        {
            if (_soundOut != null)
            {
                int length = (int) TrackLength.TotalSeconds;
                int clampedPosition = Math.Max(0, Math.Min(positionInSeconds, length));
                var timeSpan = TimeSpan.FromSeconds(clampedPosition);

                _source.SetPosition(timeSpan);
            }
        }

        #endregion INTERACTION METHODS

        #region PLAYBACK METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Start playing audio from current position. </summary>
        public void Play()
        {
            if (_soundOut != null)
            {
                if (PlaybackState == PlaybackState.Playing)
                {
                    return;
                }
                else
                {
                    _soundOut.Volume = _volume;
                    _soundOut.Play();
                }
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Stop playing audio and start playing from beginning. </summary>
        public void Replay()
        {
            if (_soundOut != null)
            {
                if (PlaybackState == PlaybackState.Playing)
                    _soundOut.Stop();

                _source.SetPosition(TimeSpan.FromSeconds(0));
                _soundOut.Volume = _volume;
                _soundOut.Play();
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Pause current playing audio. </summary>
        public void Pause()
        {
            if (_soundOut != null)
            {
                if (PlaybackState == PlaybackState.Stopped || PlaybackState == PlaybackState.Paused)
                {
                    return;
                }
                else
                {
                    _soundOut.Pause();
                }
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Stop playing audio and set position to beginning. </summary>
        public void Stop()
        {
            if (_soundOut != null)
            {
                if (PlaybackState != PlaybackState.Stopped)
                    _soundOut.Stop();

                _source.SetPosition(TimeSpan.FromSeconds(0));
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Select next song from playlist by specific criteria (random, shuffle, playingFinished). </summary>
        /// <returns> Newly selected song. </returns>
        public SongFile Next()
        {
            bool autoChange = IsFinishedPlaying;

            if (PlaybackState != PlaybackState.Stopped || autoChange)
                Stop();

            int currentIndex = PlayList.SelectedItemIndex;
            int nextIndex = _shuffle
                ? _permutation[currentIndex]
                : ((currentIndex + 1) % PlayList.Count);

            if (_repeat == RepeatMode.SINGLE && autoChange)
                return SelectFromPlayList(currentIndex);

            switch (_repeat)
            {
                case RepeatMode.SINGLE:
                case RepeatMode.NO:
                    if (nextIndex > 0)
                        return SelectFromPlayList(nextIndex);
                    break;

                case RepeatMode.ALL:
                    return SelectFromPlayList(nextIndex);
            }

            return null;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Select previous song from playlist by specific criteria (shuffle, back repeat time). </summary>
        /// <returns> Previously selected song. </returns>
        public SongFile Previous()
        {
            if (PlaybackState != PlaybackState.Stopped)
                Stop();

            int currentIndex = PlayList.SelectedItemIndex;

            if (TrackPosition.TotalMilliseconds > _backRepeatTime)
                return SelectFromPlayList(currentIndex);

            if (currentIndex > 0)
            {
                int nextIndex = _shuffle
                    ? _permutation.GetValueIndex(currentIndex)
                    : (currentIndex - 1);

                if (currentIndex != nextIndex)
                    return SelectFromPlayList(nextIndex);
            }

            return PlayList[currentIndex];
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Select song file from playlist and start playing. </summary>
        /// <param name="index"> Index of song file from playlist. </param>
        /// <returns> Selected song file. </returns>
        public SongFile SelectFromPlayList(int index)
        {
            var selectedSong = PlayList.SelectItem(index);

            if (selectedSong != null)
                LoadFile(selectedSong.FilePath, AutoPlayAfterLoad);

            return selectedSong;
        }

        #endregion PLAYBACK METHODS

        #region SETUP METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Setup playlist interaction methods. </summary>
        private void SetupPlayListInteraction()
        {
            //  Setup on add single file method.
            PlayList.OnAddItem += (s, f) =>
            {
                if (_shuffle)
                    _permutation.AddElement();

                if (AutoPlayAfterAdd)
                    SelectFromPlayList(PlayList.IndexOf(f));
            };

            //  Setup on add multiple files method.
            PlayList.OnAddItems += (s, f) =>
            {
                if (_shuffle)
                    _permutation.AddRange(f.Count());

                if (AutoPlayAfterLoad)
                    SelectFromPlayList(PlayList.IndexOf(f.FirstOrDefault()));
            };

            //  Setup on remove single file method.
            PlayList.OnRemoveItem += (s, f) =>
            {
                if (_shuffle)
                    _permutation.RemoveElement();
            };

            //  Setup on remove multiple files method.
            PlayList.OnRemoveItems += (s, f) =>
            {
                if (_shuffle)
                    _permutation.RemoveRange(f.Count());
            };
        }

        #endregion SETUP METHODS

        #region UTILITY METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get procentage position of playing audio track. </summary>
        /// <returns> Procentage position of playing audio track. </returns>
        public double ProcentageTrackPosition()
        {
            double length = TrackLength.TotalSeconds;
            double position = Math.Max(0, Math.Min(TrackPosition.TotalSeconds, length));

            return ((position * 100) / length);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Change position of current playing audio by procentage position. </summary>
        /// <param name="procentagePosition"> New position in precent of current playing audio. </param>
        public void SetProcentageTrackPosition(double procentagePosition)
        {
            double length = TrackLength.TotalSeconds;
            double position = (procentagePosition * length) / 100;

            SetTrackPosition((int)position);
        }

        #endregion UTILITY METHODS

    }
}
