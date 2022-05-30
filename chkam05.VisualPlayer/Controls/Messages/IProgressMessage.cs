using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Controls.Messages
{
    public interface IProgressMessage : IMessage
    {

        //  VARIABLES

        double ProgressValue { get; set; }


        //  METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get mapped progress value. </summary>
        /// <param name="maxValue"> Max value to get. </param>
        /// <returns> Mapped progress value. </returns>
        double GetMapProgressValue(double maxValue);

        //  --------------------------------------------------------------------------------
        /// <summary> Set progress value by mapping value. </summary>
        /// <param name="value"> Value to map. </param>
        /// <param name="maxValue"> Max value to set. </param>
        void SetMapProgressValue(double value, double maxValue);

    }
}
