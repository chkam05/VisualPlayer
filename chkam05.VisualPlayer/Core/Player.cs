using chkam05.VisualPlayer.Core.Events;
using chkam05.VisualPlayer.Data.Files;
using chkam05.VisualPlayer.Data.PlayLists;
using chkam05.VisualPlayer.Data.Static;
using chkam05.VisualPlayer.Utilities;
using chkam05.VisualPlayer.Visualisations.Spectrum;
using CSCore;
using CSCore.Codecs;
using CSCore.DSP;
using CSCore.SoundOut;
using CSCore.Streams;
using CSCore.Streams.Effects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Core
{
    public class Player : IDisposable, INotifyPropertyChanged
    {

        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler OnContiniousUpdate;
        public event EventHandler<PlayerLoadedFileEventArgs> OnLoadedFile;
        public event EventHandler OnStateUpdate;


        //  VARIABLES

        private static Player _instance;
        private BackgroundWorker _updater;
        private int _updaterSleepTime = 10;

        private bool _isLoaded;
        private IPlayable _loadedItem;

        private PitchShifter _pitchShifter;
        private ISoundOut _soundOut;
        private IWaveSource _source = null;
        private float _volume = 0.5f;

        private Repeat _repeat = Repeat.NORMAL;
        private bool _shuffle = false;

        public PlayList PlayList { get; private set; }
        public FftSize SpectrumFFTSize { get; private set; }
        public SpectrumProvider SpectrumProvider { get; private set; }


        //  GETTERS & SETTERS

        public static Player Instnace
        {
            get
            {
                if (_instance == null)
                    _instance = new Player();

                return _instance;
            }
        }

        public bool IsFinishedPlaying
        {
            get => _soundOut != null
                ? TrackPosition.TotalMilliseconds >= TrackLength.TotalMilliseconds
                    && PlaybackState == PlaybackState.Stopped
                : false;
        }

        public IPlayable LoadedItem
        {
            get => _loadedItem;
            private set
            {
                _loadedItem = value;
            }
        }

        public bool IsLoaded
        {
            get => _isLoaded;
            private set
            {
                _isLoaded = value;
                OnPropertyChanged(nameof(IsLoaded));
            }
        }

        public PlaybackState PlaybackState
        {
            get => _soundOut != null ? _soundOut.PlaybackState : PlaybackState.Stopped;
            set => OnPropertyChanged(nameof(PlaybackState));
        }

        public Repeat Repeat
        {
            get => _repeat;
            set
            {
                _repeat = value;
                OnPropertyChanged(nameof(Repeat));
            }
        }

        public bool Shuffle
        {
            get => _shuffle;
            set
            {
                _shuffle = value;
                OnPropertyChanged(nameof(Shuffle));

                if (value)
                    PlayList.Shuffle();
            }
        }

        public TimeSpan TrackLength
        {
            get => _source != null ? _source.GetLength() : TimeSpan.Zero;
        }

        public TimeSpan TrackPosition
        {
            get => _source != null ? _source.GetPosition() : TimeSpan.Zero;
            set => SetTrackPositionMs((long)value.TotalMilliseconds);
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


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Player private class constructor. </summary>
        private Player()
        {
            //  Setup data containers.
            PlayList = new PlayList();

            //  Setup initial data.
            SpectrumFFTSize = FftSize.Fft4096;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            StopUpdater();
            CleanupPlayback();
        }

        #endregion CLASS METHODS

        #region AUDIO MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Load file to audio devide to preform play. </summary>
        /// <param name="filePath"> Fila path to load. </param>
        private void LoadFileToPlay(string filePath)
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

            if (_source != null && _soundOut != null)
            {
                IsLoaded = true;
                OnLoadedFile?.Invoke(this, new PlayerLoadedFileEventArgs(LoadedItem));
            }
        }

        //  --------------------------------------------------------------------------------
        private SpectrumProvider SetupSpectrumProvider(ISampleSource source, FftSize fftSize)
        {
            //  Create and return spectrum provider.
            return new SpectrumProvider(source.WaveFormat.Channels, source.WaveFormat.SampleRate, fftSize);
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

            IsLoaded = false;
        }

        #endregion AUDIO MANAGEMENT METHODS

        #region CONTROL METHODS

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
                    OnPropertyChanged(nameof(PlaybackState));
                    StartUpdater();
                }
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Start playing audio from IPlayable file item. </summary>
        /// <param name="item"> IPlayable file item. </param>
        public void Play(IPlayable item)
        {
            if (PlayList.Select(item))
            {
                if (PlaybackState != PlaybackState.Stopped)
                    Stop();

                LoadedItem = item;

                LoadFileToPlay(item.FilePath);
                Play();
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
                    OnPropertyChanged(nameof(PlaybackState));
                    StopUpdater();
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
                {
                    _soundOut.Stop();
                    OnPropertyChanged(nameof(PlaybackState));
                    StopUpdater();
                }

                _source.SetPosition(TimeSpan.FromSeconds(0));
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Select next song from playlist and play it. </summary>
        public void Next()
        {
            bool autoChange = IsFinishedPlaying;
            bool autoPlay = true;

            if (PlaybackState != PlaybackState.Stopped || autoChange)
                Stop();

            IPlayable next;

            if (Repeat == Repeat.ONE)
            {
                next = PlayList.Selected ?? LoadedItem;
            }
            else
            {
                next = Shuffle ? PlayList.RandomNext() : PlayList.SelectNext();

                if (Repeat == Repeat.ALL && next == null)
                    next = PlayList.Select(0);

                if (Repeat == Repeat.NORMAL && PlayList.SelectedIndex == 0)
                    autoPlay = false;
            }

            if (next != null)
            {
                LoadedItem = next;

                LoadFileToPlay(next.FilePath);

                if (autoPlay)
                    Play();
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Select previous song from playlist and play it. </summary>
        public void Previous()
        {
            if (PlaybackState != PlaybackState.Stopped)
                Stop();

            IPlayable previous;

            if (Repeat == Repeat.ONE)
            {
                previous = PlayList.Selected ?? LoadedItem;
            }
            else
            {
                previous = Shuffle ? PlayList.RandomPrevious() : PlayList.SelectPrevious();

                if (Repeat == Repeat.ALL && previous == null)
                    previous = PlayList.Select(PlayList.Count - 1);
            }

            if (previous != null)
            {
                LoadedItem = previous;

                LoadFileToPlay(previous.FilePath);
                Play();
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Change position of current playing audio. </summary>
        /// <param name="seconds"> New position in seconds of current playing audio. </param>
        public void SetTrackPosition(double seconds)
        {
            if (_soundOut != null)
            {
                int length = (int)TrackLength.TotalSeconds;
                double clampedPosition = Math.Max(0, Math.Min(seconds, length));
                var timeSpan = TimeSpan.FromSeconds(clampedPosition);

                _source.SetPosition(timeSpan);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Change position of current playing audio. </summary>
        /// <param name="miliseconds"> New position in miliseconds of current playing audio. </param>
        public void SetTrackPositionMs(long miliseconds)
        {
            if (_soundOut != null)
            {
                long length = (long)TrackLength.TotalMilliseconds;
                long clampedPosition = Math.Max(0, Math.Min(miliseconds, length));
                var timeSpan = TimeSpan.FromMilliseconds(clampedPosition);

                _source.SetPosition(timeSpan);
            }
        }

        #endregion CONTROL METHODS

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

        #region OPTIONAL MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Switch Repeat mode to next mode. </summary>
        public void SwitchRepeatMode()
        {
            var modes = EnumUtilities.ListOf<Repeat>();
            var modeIndex = modes.IndexOf(Repeat);

            Repeat = modes[(modeIndex + 1) % modes.Count];
        }

        #endregion OPTIONAL MANAGEMENT METHODS

        #region UPDATER METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Setup background updater. </summary>
        private void SetupUpdater()
        {
            _updater = new BackgroundWorker();
            _updater.WorkerSupportsCancellation = true;
            _updater.DoWork += UpdaterWork;
            _updater.RunWorkerCompleted += UpdaterComplete;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Start background updater. </summary>
        private void StartUpdater()
        {
            if (_updater == null)
            {
                SetupUpdater();
            }
            else if (_updater.IsBusy)
            {
                StopUpdater();
                SetupUpdater();
            }

            _updater.RunWorkerAsync();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Stop background updater. </summary>
        private void StopUpdater()
        {
            if (_updater != null && _updater.IsBusy)
            {
                _updater.CancelAsync();
                _updater = null;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Background updater do work method. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Do work event arguments. </param>
        private void UpdaterWork(object sender, DoWorkEventArgs e)
        {
            OnStateUpdate?.Invoke(this, new EventArgs());

            while (PlaybackState == PlaybackState.Playing && OnContiniousUpdate != null)
            {
                if (_updater == null || _updater.CancellationPending)
                    break;

                try
                {
                    OnContiniousUpdate.Invoke(this, new EventArgs());
                    Thread.Sleep(_updaterSleepTime);
                }
                catch (Exception)
                {
                    return;
                }
            }

            if (_updater != null && !_updater.CancellationPending && IsFinishedPlaying)
                Next();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Background updater run worker completed method. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Run worker completed event args. </param>
        private void UpdaterComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            OnStateUpdate?.Invoke(this, new EventArgs());
        }

        #endregion UPDATER METHODS

    }
}
