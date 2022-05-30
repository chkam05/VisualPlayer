using chkam05.VisualPlayer.Data.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace chkam05.VisualPlayer.Controls.Events
{
    public class PaletteColorUpdateEventArgs : EventArgs
    {

        //  VARIABLES

        public ColorInfo ColorInfo { get; private set; }


        //  GETTERS & SETTERS

        public Color? Color
        {
            get => ColorInfo?.Color;
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> PaletteColorUpdateEventArgs class constructor. </summary>
        /// <param name="colorInfo"> Selected ColorInfo. </param>
        public PaletteColorUpdateEventArgs(ColorInfo colorInfo)
        {
            ColorInfo = colorInfo;
        }

        #endregion CLASS METHODS

    }
}
