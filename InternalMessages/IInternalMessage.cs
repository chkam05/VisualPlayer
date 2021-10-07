using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace chkam05.InternalMessages
{
    public interface IInternalMessage
    {

        //  VARIABLES

        string Message { get; set; }
        string Title { get; set; }
        PackIconKind Icon { get; set; }
        Brush IconColor { get; set; }


        //  METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Close message and load previous message if was showen. </summary>
        /// <returns> Previous showen message or NULL. </returns>
        IInternalMessage Close();

        //  --------------------------------------------------------------------------------
        /// <summary> Check if interface type is particular type of internal message. </summary>
        /// <param name="type"> Internal message type to check. </param>
        /// <returns> True - interface is type of internal message type; False - otherwise. </returns>
        bool IsTypeOf(Type type);

    }
}
