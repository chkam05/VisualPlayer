using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Data.Files
{
    public interface IPlayable : IFile, IFileMeta
    {

        //  GETTERS & SETTERS

        bool NowPlaying { get; set; }

    }
}
