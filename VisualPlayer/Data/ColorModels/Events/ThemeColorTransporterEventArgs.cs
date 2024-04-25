using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualPlayer.Data.ColorModels.Events
{
    public class ThemeColorTransporterEventArgs : EventArgs
    {

        //  VARIABLES

        public ThemeColor ThemeColor { get; private set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> ThemeColorTransporterEventArgs class constructor. </summary>
        /// <param name="themeColor"> Theme color. </param>
        public ThemeColorTransporterEventArgs(ThemeColor themeColor) : base()
        {
            ThemeColor = themeColor;
        }

        #endregion CLASS METHODS

    }
}
