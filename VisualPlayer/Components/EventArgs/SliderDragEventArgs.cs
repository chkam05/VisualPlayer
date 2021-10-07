using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace chkam05.VisualPlayer.Components.EventArgs
{
    public class SliderDragEventArgs
    {

        //  VARIABLES

        public SliderDragAction Action { get; private set; }
        public Point? HandlerPosition { get; private set; }
        public double Position { get; private set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> SliderDragEventArgs class constructor. </summary>
        /// <param name="action"> Slider drag action. </param>
        /// <param name="handlerPosition"> Position of slider handler. </param>
        /// <param name="position"> Position of slider handler in %. </param>
        public SliderDragEventArgs(SliderDragAction action, double position, Point? handlerPosition = null)
        {
            Action = action;
            HandlerPosition = handlerPosition;
            Position = position;
        }

        #endregion CLASS METHODS

    }
}
