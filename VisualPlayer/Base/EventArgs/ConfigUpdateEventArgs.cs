using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Base.EventArgs
{
    public class ConfigUpdateEventArgs
    {

        //  VARIABLES

        public string Key { get; private set; }
        public object Value { get; private set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> ConfigUpdateEventArgs class constructor. </summary>
        public ConfigUpdateEventArgs(string key, object value)
        {
            Key = key;
            Value = value;
        }

        #endregion CLASS METHODS

    }
}
