using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using VisualPlayer.Commands;
using VisualPlayer.Data.Enums;
using VisualPlayer.ViewModels;

namespace VisualPlayer.Data.Player
{
    public class ControlBarDataContext : BaseViewModel
    {

        //  COMMANDS

        public ICommand MuteCommand { get; set; }
        public ICommand NextCommand { get; set; }
        public ICommand PlayPauseCommand { get; set; }
        public ICommand PreviousCommand { get; set; }
        public ICommand RepeatCommand { get; set; }
        public ICommand ShuffleCommand { get; set; }
        public ICommand StopCommand { get; set; }
        public ICommand VolumeCommand { get; set; }


        //  DELEGATES

        public delegate void ControlCommandExecutedEventHandler(object sender, ControlCommandExecutedEventArgs e);


        //  EVENTS

        public ControlCommandExecutedEventHandler ControlCommandExecuted;


        //  VARIABLES

        private string _album = "ALBUM: 2013 #4 - Autumn";
        private string _artist = "ARTIST: Unknown";
        private ImageSource _cover = null;
        private bool _isMute = false;
        private bool _isPlaying = false;
        private string _title = "TITLE: Track No. 1";
        private double _trackLength = 100;
        private double _trackPosition = 50;
        private TimeSpan _trackTime = TimeSpan.Zero;
        private TimeSpan _trackTimeProgress = TimeSpan.Zero;
        private double _volumePosition = 50;


        //  GETTERS & SETTERS

        public string Album
        {
            get => _album;
            set => UpdateProperty(ref _album, value);
        }

        public string Artist
        {
            get => _artist;
            set => UpdateProperty(ref _artist, value);
        }

        public ImageSource Cover
        {
            get => _cover;
            set => UpdateProperty(ref _cover, value);
        }

        public bool IsMute
        {
            get => _isMute;
            set
            {
                UpdateProperty(ref _isMute, value);
                NotifyPropertyChanged(nameof(VolumePosition));
            }
        }

        public bool IsPlaying
        {
            get => _isPlaying;
            set => UpdateProperty(ref _isPlaying, value);
        }

        public string Title
        {
            get => _title;
            set => UpdateProperty(ref _title, value);
        }

        public double TrackLength
        {
            get => _trackLength;
            set => UpdateProperty(ref _trackLength, Math.Max(0, value));
        }

        public double TrackPosition
        {
            get => Math.Min(_trackPosition, _trackLength);
            set => UpdateProperty(ref _trackPosition, Math.Max(0, Math.Min(_trackLength, value)));
        }

        public TimeSpan TrackTime
        {
            get => _trackTime;
            set
            {
                UpdateProperty(ref _trackTime, value);
                NotifyPropertyChanged(nameof(TrackTimeStr));
            }
        }

        public string TrackTimeStr
        {
            get => _trackTime.ToString(@"hh\:mm\:ss");
        }

        public TimeSpan TrackTimeProgress
        {
            get => _trackTimeProgress;
            set
            {
                UpdateProperty(ref _trackTime, value);
                NotifyPropertyChanged(nameof(TrackTimeProgressStr));
            }
        }

        public string TrackTimeProgressStr
        {
            get => _trackTimeProgress.ToString(@"hh\:mm\:ss");
        }

        public double VolumePosition
        {
            get => _isMute ? 0 : _volumePosition;
            set
            {
                UpdateProperty(ref _volumePosition, Math.Max(0d, Math.Min(100d, value)));

                if (_isMute && value > 0d)
                    IsMute = false;
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> ControlBarDataContext class constructor. </summary>
        public ControlBarDataContext()
        {
            //  Setup commands.

            MuteCommand = new RelayCommand(OnMuteCommandExecuted);
            NextCommand = new RelayCommand(OnNextCommandExecuted);
            PlayPauseCommand = new RelayCommand(OnPlayPauseCommandExecuted);
            PreviousCommand = new RelayCommand(OnPreviousCommandExecuted);
            RepeatCommand = new RelayCommand(OnRepeatCommandExecuted);
            ShuffleCommand = new RelayCommand(OnShuffleCommandExecuted);
            StopCommand = new RelayCommand(OnStopCommandExecuted);
            VolumeCommand = new RelayCommand(OnVolumeCommandExecuted);
        }

        #endregion CLASS METHODS

        #region CONTROL COMMANDS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Invoked ControlCommandExecuted. </summary>
        /// <param name="controlCommand"> Control command type. </param>
        private void InvokeControlCommandExecuted(ControlCommand controlCommand)
        {
            ControlCommandExecuted?.Invoke(this, new ControlCommandExecutedEventArgs(controlCommand));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Mute command execution methods. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        private void OnMuteCommandExecuted(object sender)
        {
            IsMute = !IsMute;

            InvokeControlCommandExecuted(ControlCommand.Mute);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Next command execution methods. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        private void OnNextCommandExecuted(object sender)
        {
            InvokeControlCommandExecuted(ControlCommand.Next);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Play/Pause command execution methods. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        private void OnPlayPauseCommandExecuted(object sender)
        {
            InvokeControlCommandExecuted(IsPlaying ? ControlCommand.Pause : ControlCommand.Play);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Previous command execution methods. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        private void OnPreviousCommandExecuted(object sender)
        {
            InvokeControlCommandExecuted(ControlCommand.Previous);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Repeat command execution methods. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        private void OnRepeatCommandExecuted(object sender)
        {
            InvokeControlCommandExecuted(ControlCommand.Repeat);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Shuffle command execution methods. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        private void OnShuffleCommandExecuted(object sender)
        {
            InvokeControlCommandExecuted(ControlCommand.Shuffle);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Stop command execution methods. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        private void OnStopCommandExecuted(object sender)
        {
            InvokeControlCommandExecuted(ControlCommand.Stop);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Volume command execution methods. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        private void OnVolumeCommandExecuted(object sender)
        {
            InvokeControlCommandExecuted(ControlCommand.Volume);
        }

        #endregion CONTROL COMMANDS METHODS

    }
}
