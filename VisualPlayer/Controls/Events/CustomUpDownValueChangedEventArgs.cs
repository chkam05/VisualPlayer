using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualPlayer.Controls.Events
{
    public class CustomUpDownValueChangedEventArgs : EventArgs
    {

        //  VARIABLES

        public double Value { get; private set; }
        public bool UIModified { get; private set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> CustomUpDownValueChangedEventArgs class constructor. </summary>
        /// <param name="value"> Value. </param>
        /// <param name="uiModified"> Is modified from user interface. </param>
        public CustomUpDownValueChangedEventArgs(double value, bool uiModified = false)
        {
            Value = value;
            UIModified = uiModified;
        }

        #endregion CLASS METHODS

    }
}
