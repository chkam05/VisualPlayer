using chkam05.VisualPlayer.Data.Config;
using chkam05.VisualPlayer.Utilities.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace chkam05.VisualPlayer.Utilities
{
    public static class ColorUtilities
    {

        //  CONST

        public static List<ColorInfo> StaticColors
        {
            get => new List<ColorInfo>()
            {
                new ColorInfo("#FFB900", "Gold Yellow"),
                new ColorInfo("#FF8C00", "Gold"),
                new ColorInfo("#F7630C", "Bright Orange"),
                new ColorInfo("#C24D0F", "Dark Orange"),
                new ColorInfo("#D53A01", "Rusty"),
                new ColorInfo("#EF6950", "Pale Rusty"),
                new ColorInfo("#CF3438", "Brick Red"),
                new ColorInfo("#F94141", "Moderate Red"),

                new ColorInfo("#E74856", "Pale Red"),
                new ColorInfo("#E81123", "Red"),
                new ColorInfo("#EA005E", "Light Pink"),
                new ColorInfo("#BA004E", "Rose"),
                new ColorInfo("#DF0089", "Light Plum"),
                new ColorInfo("#BA0074", "Plum"),
                new ColorInfo("#C239B3", "Lightly Orchid"),
                new ColorInfo("#950084", "Orchid"),

                new ColorInfo("#0078D7", "Blue"),
                new ColorInfo("#0063B1", "Navy"),
                new ColorInfo("#8785CE", "Purple Shade"),
                new ColorInfo("#6B69D6", "Dark Purple Shade"),
                new ColorInfo("#8562B5", "Pastel Iris"),
                new ColorInfo("#704BA4", "Brightly Iridescent"),
                new ColorInfo("#AD44BD", "Light Purple Red"),
                new ColorInfo("#881798", "Purple Red"),

                new ColorInfo("#0099BC", "Bright Blue"),
                new ColorInfo("#2D7D9A", "Light Blue"),
                new ColorInfo("#00B7C3", "Sea Foam"),
                new ColorInfo("#038387", "Greeny"),
                new ColorInfo("#00B294", "Light Mint"),
                new ColorInfo("#018170", "Dark Mint"),
                new ColorInfo("#00CC6A", "Peaty"),
                new ColorInfo("#10893E", "Bright Green"),

                new ColorInfo("#746F6E", "Gray"),
                new ColorInfo("#5D5A58", "Gray Brown"),
                new ColorInfo("#68768A", "Steel Blue"),
                new ColorInfo("#515C6B", "Metalic Blue"),
                new ColorInfo("#567C73", "Pale Dark Green"),
                new ColorInfo("#47675F", "Dark Green"),
                new ColorInfo("#498205", "Light Green"),
                new ColorInfo("#107C10", "Green"),

                new ColorInfo("#6B6B6B", "Cloudy"),
                new ColorInfo("#4A4846", "Storm"),
                new ColorInfo("#69797E", "Blue Gray"),
                new ColorInfo("#464F54", "Dark Gray"),
                new ColorInfo("#637B63", "Shaded Green"),
                new ColorInfo("#525E54", "Sage"),
                new ColorInfo("#847545", "Desert"),
                new ColorInfo("#766B59", "Moro"),
            };
        }

        public static List<ColorInfo> DefaultColors
        {
            get => new List<ColorInfo>()
            {
                new ColorInfo("#0078D7", "Blue"),
                new ColorInfo("#038387", "Greeny"),
                new ColorInfo("#5D5A58", "Gray Brown"),
                new ColorInfo("#E81123", "Red"),
                new ColorInfo("#EA005E", "Light Pink")
            };
        }


        //  METHODS

        #region COLOR CONVERSION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Convert color to Hexadecimal color code. </summary>
        /// <param name="color"> Color object to convert. </param>
        /// <returns> Hexadecimal color code. </returns>
        public static string ColorToHex(Color color)
        {
            return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Conver Hexadecimal color code to color. </summary>
        /// <param name="colorCode"> Hexadecimal color code. </param>
        /// <param name="defaultResult"> Default result color. </param>
        /// <returns> Color object. </returns>
        public static Color ColorFromHex(string colorCode, Color? defaultResult = null)
        {
            try
            {
                return (Color)ColorConverter.ConvertFromString(colorCode);
            }
            catch (Exception)
            {
                if (defaultResult.HasValue)
                    return defaultResult.Value;
                return Colors.Transparent;
            }
        }

        #endregion COLOR CONVERSION METHODS

        #region COLOR MANIPULATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Change color brightness. </summary>
        /// <param name="color"> Color. </param>
        /// <param name="light"> Color lightness that will be set. </param>
        /// <param name="byAdding"> Use to increase or decrease value, not overwrite it. </param>
        /// <returns> Color with changed brightness. </returns>
        public static Color ChangeColorBrigthess(Color color, int light, bool byAdding = false)
        {
            AHSLColor ahslColor = AHSLColor.FromColor(color);
            ahslColor.L = byAdding ? ahslColor.L + light : light;
            return ahslColor.ToColor();
        }

        //  --------------------------------------------------------------------------------
        //// <summary> Change color saturation. </summary>
        /// <param name="color"> Color. </param>
        /// <param name="saturation"> Color saturation that will be set. </param>
        /// <param name="byAdding"> Use to increase or decrease value, not overwrite it. </param>
        /// <returns> Color with changed saturation. </returns>
        public static Color ChangeColorSaturation(Color color, int saturation, bool byAdding = false)
        {
            AHSLColor ahslColor = AHSLColor.FromColor(color);
            ahslColor.S = byAdding ? ahslColor.S + saturation : saturation;
            return ahslColor.ToColor();
        }

        //  --------------------------------------------------------------------------------
        //// <summary> Change color hue. </summary>
        /// <param name="color"> Color. </param>
        /// <param name="hue"> Color hue that will be set. </param>
        /// <param name="byAdding"> Use to increase or decrease value, not overwrite it. </param>
        /// <returns> Color with changed hue. </returns>
        public static Color ChangeColorHue(Color color, int hue, bool byAdding = false)
        {
            AHSLColor ahslColor = AHSLColor.FromColor(color);
            ahslColor.H = byAdding ? (ahslColor.H + hue) % AHSLColor.HUE_MAX : hue;
            var c = ahslColor.ToColor();
            return ahslColor.ToColor();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Inverse color. </summary>
        /// <param name="color"> Color to inverse. </param>
        /// <param name="includeAlpha"> Inverse alpha channel. </param>
        /// <returns> Inversed color. </returns>
        public static Color InverseColor(Color color, bool includeAlpha = false)
        {
            return Color.FromArgb(
                (byte)(includeAlpha ? 255 - color.A : color.A),
                (byte)(255 - color.R),
                (byte)(255 - color.G),
                (byte)(255 - color.B));
        }

        #endregion COLOR MANIPULATION METHODS

        #region COLOR UPDATE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Update AHSL color by changing its components. </summary>
        /// <param name="color"> AHSL color to update. </param>
        /// <param name="a"> Alpha component. </param>
        /// <param name="h"> Hue. </param>
        /// <param name="s"> Saturation. </param>
        /// <param name="l"> Lightness. </param>
        /// <returns> Updated AHSL color. </returns>
        public static AHSLColor UpdateColor(AHSLColor color, byte? a = null, int? h = null, int? s = null, int? l = null)
        {
            return new AHSLColor(
                a.HasValue ? a.Value : color.A,
                h.HasValue ? (h.Value % AHSLColor.HUE_MAX) : color.H,
                s.HasValue ? s.Value : color.S,
                l.HasValue ? l.Value : color.L);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update color by changing its components. </summary>
        /// <param name="color"> Color to update. </param>
        /// <param name="a"> Alpha component. </param>
        /// <param name="r"> Red component. </param>
        /// <param name="g"> Green component. </param>
        /// <param name="b"> Blue component. </param>
        /// <returns> Updated color. </returns>
        public static Color UpdateColor(Color color, byte? a = null, byte? r = null, byte? g = null, byte? b = null)
        {
            return Color.FromArgb(
                a.HasValue ? a.Value : color.A,
                r.HasValue ? r.Value : color.R,
                g.HasValue ? g.Value : color.G,
                b.HasValue ? b.Value : color.B);
        }

        #endregion COLOR UPDATE METHODS

    }
}
