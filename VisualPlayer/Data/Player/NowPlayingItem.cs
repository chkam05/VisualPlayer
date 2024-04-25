using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualPlayer.ViewModels.Player;

namespace VisualPlayer.Data.Player
{
    public class NowPlayingItem : SongViewModel
    {

        //  CONST

        public const PackIconKind IconIsPlaying = PackIconKind.Play;


        //  VARIABLES

        private PackIconKind _icon = PackIconKind.MusicNote;
        private bool _isPlaying = false;


        //  GETTERS & SETTERS

        public PackIconKind Icon
        {
            get => _icon;
            set => UpdateProperty(ref _icon, value);
        }

        public bool IsPlaying
        {
            get => _isPlaying;
            set => UpdateProperty(ref _isPlaying, value);
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> NowPlayingItem class constructor. </summary>
        /// <param name="filePath"> File path. </param>
        public NowPlayingItem(string filePath) : base(filePath)
        {
            //
        }

        #endregion CLASS METHODS

    }
}
