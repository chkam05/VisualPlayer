using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualPlayer.Data.Enums;

namespace VisualPlayer.Data.Configuration
{
    public class RequestAnimationEventArgs : EventArgs
    {

        //  VARIABLES

        public string AnimationId { get; private set; }
        public AnimationTarget Target { get; private set; }
        public object Value { get; private set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> RequestAnimationEventArgs class constructor. </summary>
        /// <param name="id"> Animation id. </param>
        /// <param name="target"> Animation target. </param>
        /// <param name="value"> Value. </param>
        public RequestAnimationEventArgs(string id, AnimationTarget target, object value)
        {
            AnimationId = id;
            Target = target;
            Value = value;
        }

        #endregion CLASS METHODS

    }
}
