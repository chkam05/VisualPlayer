using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace chkam05.VisualPlayer.Controls.Events
{
    public class SliderValueChangedEventArgs<T> : RoutedEventArgs
    {

        //  VARIABLES

        public bool Final { get; private set; }
        public T Value { get; private set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> SliderValueChangedEventArgs class constructor. </summary>
        /// <param name="value"> Changed value. </param>
        /// <param name="final"> Finally change. </param>

        public SliderValueChangedEventArgs(T value, bool final = false)
        {
            Value = value;
            Final = final;
        }

        #endregion CLASS METHODS

    }
}
