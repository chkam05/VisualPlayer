using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Core.BiQuad
{
    public enum BiQuadFilterType
    {
        None = 0,
        BandpassFilter = 1,
        HighpassFilter = 2,
        HighShelfFilter = 3,
        LowpassFilter = 4,
        LowShelfFilter = 5,
        NotchFilter = 6,
        PeakFilter = 7
    }
}
