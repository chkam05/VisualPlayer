using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Controls.Data
{
    public enum MarqueeBehaviour
    {
        //  <- [ ... ] <-
        RIGHT_OUT_TO_LEFT_OUT = 0,

        //  <- [ ... <- ]
        RIGHT_IN_TO_LEFT_OUT = 1,

        //  <- [ <- ... ]
        RIGHT_TO_LEFT_OUT = 2,

        //  [ <- ... <- ]
        RIGHT_TO_LEFT = 3,

        //  [ <- ... <- ]
        //  [ -> ... -> ]
        RIGHT_TO_LEFT_TO_RIGHT = 4,

        //  -> [ ... ] ->
        LEFT_OUT_TO_RIGHT_OUT = 5,

        //  [ -> ... ] ->
        LEFT_IN_TO_RIGHT_OUT = 6,

        //  [ -> ... -> ]
        LEFT_TO_RIGHT = 7,

        //  [ -> ... -> ]
        //  [ <- ... <- ]
        LEFT_TO_RIGHT_TO_LEFT = 8
    }
}
