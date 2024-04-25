using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualPlayer.Data.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class NotifyPropertyUpdateAttribute : Attribute
    {

        //  VARIABLES

        public bool AllowNotify { get; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> NotifyPropertyUpdateAttribute class constructor. </summary>
        /// <param name="allowNotify"> Allow notify property update in user interface. </param>
        public NotifyPropertyUpdateAttribute(bool allowNotify)
        {
            AllowNotify = allowNotify;
        }

        #endregion CLASS METHODS

    }
}
