using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VisualPlayer.Controls
{
    public class CustomContextMenuItem : CustomMenuItem
    {

        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Static CustomContextMenuItem class constructor. </summary>
        static CustomContextMenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomContextMenuItem),
                new FrameworkPropertyMetadata(typeof(CustomContextMenuItem)));
        }

        #endregion CLASS METHODS

    }
}
