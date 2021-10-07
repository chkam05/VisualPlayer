using chkam05.VisualPlayer.Base.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Data.Configuration
{
    public interface IAppConfig
    {

        //  EVENTS

        event EventHandler<ConfigUpdateEventArgs> OnConfigUpdate;


        //  METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get value from class property. </summary>
        /// <param name="propertyName"> Property name. </param>
        /// <returns> Value of this property. </returns>
        object Get(string propertyName);

        //  --------------------------------------------------------------------------------
        /// <summary> Check if class contains particular property name. </summary>
        /// <param name="propertyName"> Property name to check. </param>
        /// <returns> True - object contains property; False - otherwise. </returns>
        bool Has(string propertyName);

        //  --------------------------------------------------------------------------------
        /// <summary> Set class property value. </summary>
        /// <param name="propertyName"> Property name. </param>
        /// <param name="value"> Value to set to selected property. </param>
        bool Set(string propertyName, object value);

    }
}
