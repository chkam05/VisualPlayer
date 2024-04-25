using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace VisualPlayer.Controls
{
    public interface IControlBar
    {

        //  METHODS

        /// <summary> Get outer grid. </summary>
        /// <returns> Outer grid. </returns>
        Grid GetOuterGrid();

        /// <summary> Get volume control button. </summary>
        /// <returns> Volume control button. </returns>
        CustomButton GetVolumeControlButton();


    }
}
