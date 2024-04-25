using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace chkam05.VisualPlayer.Data.Files
{
    public interface IFileMeta
    {

        //  GETTERS & SETTERS

        string Album { get; set; }
        string Artist { get; set; }
        ImageSource Cover { get; set; }
        string Title { get; set; }

    }
}
