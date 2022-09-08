using chkam05.Tools.ControlsEx.Colors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace chkam05.VisualPlayer.Data.Configuration
{
    public class ColorInfo
    {

        //  VARIABLES

        public Color Color { get; set; }
        public string ColorCode { get; set; }
        public string Name { get; set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> ColorInfo class constructor. </summary>
        public ColorInfo()
        {
            //
        }

        //  --------------------------------------------------------------------------------
        /// <summary> ColorInfo class constructor from ColorPaletteItem object. </summary>
        /// <param name="colorPaletteItem"> ColorPaletteItem. </param>
        public ColorInfo(ColorPaletteItem colorPaletteItem)
        {
            if (colorPaletteItem != null)
            {
                Color = colorPaletteItem.Color;
                ColorCode = colorPaletteItem.ColorCode;
                Name = colorPaletteItem.Name;
            }
        }

        #endregion CLASS METHODS

        #region CAST METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Convert ColorPaletteItems list of objects to ColorInfo list. </summary>
        /// <param name="colorPaletteItems"> List of ColorPaletteItems objects. </param>
        /// <returns></returns>
        public static List<ColorInfo> ToColorPaletteItems(IEnumerable<ColorPaletteItem> colorPaletteItems)
        {
            return colorPaletteItems?.Select(i => new ColorInfo(i)).ToList() ?? new List<ColorInfo>();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Convert ColorInfo object to ColorPaletteItem. </summary>
        /// <returns> ColorPaletteItem. </returns>
        public ColorPaletteItem ToColorPaletteItem()
        {
            return new ColorPaletteItem(Color, Name);
        }

        #endregion CAST METHODS

    }
}
