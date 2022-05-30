using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Components.Events
{
    public class ExtendedTextBoxContentUpdatedEventArgs : EventArgs
    {

        //  VARIABLES

        public bool Programmatically { get; private set; }
        public string PreviousText { get; private set; }
        public string Text { get; private set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> ExtendedTextBoxContentUpdatedEventArgs class constructor. </summary>
        /// <param name="text"> Text. </param>
        /// <param name="previousText"> Previous text. </param>
        /// <param name="programmatically"> Change maded programmatically. </param>
        public ExtendedTextBoxContentUpdatedEventArgs(string text, string previousText, bool programmatically)
        {
            Programmatically = programmatically;
            PreviousText = previousText;
            Text = text;
        }

        #endregion CLASS METHODS

    }
}
