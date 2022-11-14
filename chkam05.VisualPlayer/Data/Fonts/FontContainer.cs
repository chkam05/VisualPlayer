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

        public FontFamily FontFamily { get; set; }
        public string Name { get; set; }
        public string SubName { get; set; }
        public bool IsAppFont { get; set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> FontContainer class constructor for Json serialization. </summary>
        public FontContainer()
        {
            //
        }

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

        #region OBJECT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Returns a string that represents the current object. </summary>
        /// <returns> A string that represents the current object. </returns>
        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Name))
            {
                if (!string.IsNullOrEmpty(SubName))
                    return $"{Name} {SubName}";

                return Name;
            }

            return base.ToString();
        }

        #endregion OBJECT METHODS

    }
}
