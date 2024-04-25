using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Data.Configuration.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ConfigPropertyUpdateAttrib : Attribute
    {

        //  VARIABLES

        public bool AllowUpdate { get; set; } = true;


        //  METHODS

        #region CLASS METHODS

        public ConfigPropertyUpdateAttrib()
        {
            //
        }

        #endregion CLASS METHODS

    }
}
