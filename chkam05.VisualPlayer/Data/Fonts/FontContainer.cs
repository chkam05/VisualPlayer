using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace chkam05.VisualPlayer.Data.Fonts
{
    public class FontContainer
    {

        //  VARIABLES

        public FontFamily FontFamily { get; private set; }
        public string Name { get; private set; }
        public string SubName { get; private set; }
        public bool IsAppFont { get; private set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> FontContainer class constructor. </summary>
        /// <param name="fontFamily"> Font family. </param>
        /// <param name="isAppFont"> Is application font. </param>
        public FontContainer(FontFamily fontFamily, bool isAppFont = false)
        {
            FontFamily = fontFamily;
            Name = fontFamily.FamilyNames.First().Value;
            IsAppFont = isAppFont;

            if (fontFamily.Source.Contains("./#"))
                SubName = fontFamily.Source
                    .Replace("./#", "")
                    .Replace(Name, "")
                    .Trim();
        }

        #endregion CLASS METHODS

    }
}
