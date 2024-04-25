using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualPlayer.Data.Enums;

namespace VisualPlayer.Data.Configuration
{
    public class AnimationRequest
    {

        //  VARIABLES

        public string Id { get; set; }
        public bool IsRunning { get; set; }
        public bool OutOfOrder { get; set; }
        public AnimationTarget Target { get; set; }
        public object Value { get; set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> AnimationRequest class constructor. </summary>
        /// <param name="target"> Animation target. </param>
        /// <param name="value"> Value. </param>
        /// <param name="outOfOrder"> Run animation immediately. </param>
        public AnimationRequest(AnimationTarget target, object value, bool outOfOrder = false)
        {
            Id = Guid.NewGuid().ToString().ToLower();
            IsRunning = false;

            Target = target;
            Value = value;
            OutOfOrder = outOfOrder;
        }

        #endregion CLASS METHODS

    }
}
