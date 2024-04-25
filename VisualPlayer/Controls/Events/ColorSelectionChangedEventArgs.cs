using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualPlayer.Data.ColorModels;

namespace VisualPlayer.Controls.Events
{
    public class ColorSelectionChangedEventArgs : EventArgs
    {

        //  VARIABLES

        public ThemeColor SelectedColor { get; private set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> ColorSelectionChangedEventArgs class constructor. </summary>
        /// <param name="selectedColor"> Selected theme color. </param>
        public ColorSelectionChangedEventArgs(ThemeColor selectedColor) : base()
        {
            SelectedColor = selectedColor;
        }

        #endregion CLASS METHODS

    }
}
