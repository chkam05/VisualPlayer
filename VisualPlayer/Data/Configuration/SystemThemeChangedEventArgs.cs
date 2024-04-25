using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using VisualPlayer.Data.Enums;

namespace VisualPlayer.Data.Configuration
{
    public class SystemThemeChangedEventArgs : EventArgs
    {

        //  VARIABLES

        public Color Color { get; private set; }
        public Theme Theme { get; private set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> SystemThemeChangedEventArgs class constructor. </summary>
        /// <param name="color"> Color. </param>
        /// <param name="theme"> Theme. </param>
        public SystemThemeChangedEventArgs(Color color, Theme theme) : base()
        {
            Color = color;
            Theme = theme;
        }

        #endregion CLASS METHODS

    }
}
