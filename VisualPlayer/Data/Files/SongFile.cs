using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace chkam05.VisualPlayer.Data.Files
{
    public class SongFile : BaseFile, IPlayableFile
    {

        //  VARIABLES

        private ImageSource _cover;
        private bool _isPlaying;
        private string _title;

        public string Album { get; set; }
        public string Artist { get; set; }
        public PackIconKind Icon { get; private set; }

        public ImageSource Cover
        {
            get => _cover;
            set => SetCover(value);
        }

        public bool IsPlaying
        {
            get => _isPlaying;
            set => SetIsPlaying(value);
        }

        public string Title
        {
            get => string.IsNullOrEmpty(_title) ? FileName : _title;
            set => _title = value;
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> SongFile class constructor. </summary>
        /// <param name="filePath"> Path to file. </param>
        /// <param name="loadMetadata"> Load metadata at creation. </param>
        public SongFile(string filePath, bool loadMetadata = true) : base(filePath, loadMetadata)
        {
            UpdateIcon();
        }

        #endregion CLASS METHODS

        #region DATA MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Set cover image. </summary>
        /// <param name="cover"> Cover image source. </param>
        private void SetCover(ImageSource cover)
        {
            _cover = cover;
            UpdateIcon();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set is playing flag. </summary>
        /// <param name="isPlaying"> New is playing flag value. </param>
        private void SetIsPlaying(bool isPlaying)
        {
            _isPlaying = isPlaying;
            UpdateIcon();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update file item icon. </summary>
        private void UpdateIcon()
        {
            if (IsPlaying)
                Icon = PackIconKind.Play;
            else if (_cover != null)
                Icon = PackIconKind.None;
            else
                Icon = PackIconKind.MusicNote;
        }

        #endregion DAT MANAGEMENT METHODS

        #region METADATA MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Fill additional informations about file. </summary>
        public override void FillMetadata()
        {
            UpdateIcon();
        }

        #endregion METADATA MANAGEMENT METHODS

    }
}
