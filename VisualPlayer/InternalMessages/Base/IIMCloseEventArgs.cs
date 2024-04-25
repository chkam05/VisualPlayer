using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualPlayer.InternalMessages.Enums;

namespace VisualPlayer.InternalMessages.Base
{
    public interface IIMCloseEventArgs
    {

        //  VARIABLES

        IMResult Result { get; }

    }
}
