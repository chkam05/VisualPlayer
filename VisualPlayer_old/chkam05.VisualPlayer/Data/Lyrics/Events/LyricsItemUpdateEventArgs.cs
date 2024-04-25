using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Data.Lyrics.Events
{
    public class LyricsItemUpdateEventArgs : EventArgs
    {

        //  VARIABLES

        public string PropertyName { get; private set; }
        public object PropertyValue { get; private set; }
        public Type PropertyType { get; private set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> LyricsItemUpdateEventArgs class constructor. </summary>
        /// <param name="name"> Lyrics changed property name. </param>
        /// <param name="value"> Lyrics changed value. </param>
        /// <param name="type"> Lyrics changed property type. </param>
        public LyricsItemUpdateEventArgs(string name, object value, Type type)
        {
            PropertyName = name;
            PropertyValue = value;
            PropertyType = type;
        }

        #endregion CLASS METHODS

    }
}
