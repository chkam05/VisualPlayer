using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Data.Lyrics
{
    public interface ILyricsController
    {

        //  VARIABLES

        string Text { get; set; }


        //  METHODS

        void ShowInterface();
        void HideInterface();

    }
}
