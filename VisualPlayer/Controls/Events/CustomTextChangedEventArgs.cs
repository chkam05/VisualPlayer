using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualPlayer.Controls.Events
{
    public class CustomTextChangedEventArgs : EventArgs
    {

        //  VARIABLES

        public string Text { get; private set; }
        public bool UIModified { get; private set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> CustomTextChangedEventArgs class constructor. </summary>
        /// <param name="text"> Text. </param>
        /// <param name="uiModified"> Is modified from user interface. </param>
        public CustomTextChangedEventArgs(string text, bool uiModified = false)
        {
            Text = text;
            UIModified = uiModified;
        }

        #endregion CLASS METHODS

    }
}
