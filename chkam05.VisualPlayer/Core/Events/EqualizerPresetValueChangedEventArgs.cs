using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Core.Events
{
    public class EqualizerPresetValueChangedEventArgs
    {

        //  VARIABLES

        public int Index { get; private set; }
        public double Value { get; private set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> EqualizerPresetValueChangedEventArgs class constructor. </summary>
        /// <param name="index"> Preset value index. </param>
        /// <param name="value"> Preset value. </param>
        public EqualizerPresetValueChangedEventArgs(int index, double value)
        {
            Index = index;
            Value = value;
        }

        #endregion CLASS METHODS

    }
}
