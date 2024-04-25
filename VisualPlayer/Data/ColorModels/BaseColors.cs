using System.Collections.Generic;
using System.Windows.Media;

namespace VisualPlayer.Data.ColorModels
{
    public static class BaseColors
    {

        //  CONST

        public static readonly Color DefaultAccentColor = Color.FromArgb(255, 0, 120, 215);
        public static readonly Color DefaultAccentColorForeground = Color.FromArgb(255, 255, 255, 255);
        public static readonly Color DefaultAccentColorMouseOver = Color.FromArgb(255, 32, 152, 247);
        public static readonly Color DefaultAccentColorPressed = Color.FromArgb(255, 0, 88, 183);
        public static readonly Color DefaultAccentColorSelected = Color.FromArgb(255, 0, 120, 215);
        public static readonly Color DefaultThemeColor = Color.FromArgb(255, 255, 255, 255);
        public static readonly Color DefaultThemeColorForeground = Color.FromArgb(255, 0, 0, 0);
        public static readonly Color DefaultThemeColorMouseOver = Color.FromArgb(255, 255, 255, 255);
        public static readonly Color DefaultThemeColorPressed = Color.FromArgb(255, 229, 229, 229);
        public static readonly Color DefaultThemeColorSelected = Color.FromArgb(255, 242, 242, 242);
        public static readonly Color DefaultThemeColorShade = Color.FromArgb(255, 219, 219, 219);

        public static readonly ThemeColor GoldYellow = ThemeColor.ThemeColorFromCode("#FFB900", "Gold Yellow");
        public static readonly ThemeColor Gold = ThemeColor.ThemeColorFromCode("#FF8C00", "Gold");
        public static readonly ThemeColor BrightOrange = ThemeColor.ThemeColorFromCode("#F7630C", "Bright Orange");
        public static readonly ThemeColor DarkOrange = ThemeColor.ThemeColorFromCode("#C24D0F", "Dark Orange");
        public static readonly ThemeColor Rusty = ThemeColor.ThemeColorFromCode("#D53A01", "Rusty");
        public static readonly ThemeColor PaleRusty = ThemeColor.ThemeColorFromCode("#EF6950", "Pale Rusty");
        public static readonly ThemeColor BrickRed = ThemeColor.ThemeColorFromCode("#CF3438", "Brick Red");
        public static readonly ThemeColor ModerateRed = ThemeColor.ThemeColorFromCode("#F94141", "Moderate Red");

        public static readonly ThemeColor PaleRed = ThemeColor.ThemeColorFromCode("#E74856", "Pale Red");
        public static readonly ThemeColor Red = ThemeColor.ThemeColorFromCode("#E81123", "Red");
        public static readonly ThemeColor LightPink = ThemeColor.ThemeColorFromCode("#EA005E", "Light Pink");
        public static readonly ThemeColor Rose = ThemeColor.ThemeColorFromCode("#BA004E", "Rose");
        public static readonly ThemeColor LightPlum = ThemeColor.ThemeColorFromCode("#DF0089", "Light Plum");
        public static readonly ThemeColor Plum = ThemeColor.ThemeColorFromCode("#BA0074", "Plum");
        public static readonly ThemeColor LightlyOrchid = ThemeColor.ThemeColorFromCode("#C239B3", "Lightly Orchid");
        public static readonly ThemeColor Orchid = ThemeColor.ThemeColorFromCode("#950084", "Orchid");

        public static readonly ThemeColor Blue = ThemeColor.ThemeColorFromCode("#0078D7", "Blue");
        public static readonly ThemeColor Navy = ThemeColor.ThemeColorFromCode("#0063B1", "Navy");
        public static readonly ThemeColor PurpleShade = ThemeColor.ThemeColorFromCode("#8785CE", "Purple Shade");
        public static readonly ThemeColor DarkPurpleShade = ThemeColor.ThemeColorFromCode("#6B69D6", "Dark Purple Shade");
        public static readonly ThemeColor PastelIris = ThemeColor.ThemeColorFromCode("#8562B5", "Pastel Iris");
        public static readonly ThemeColor BrightlyIridescent = ThemeColor.ThemeColorFromCode("#704BA4", "Brightly Iridescent");
        public static readonly ThemeColor LightPurpleRed = ThemeColor.ThemeColorFromCode("#AD44BD", "Light Purple Red");
        public static readonly ThemeColor PurpleRed = ThemeColor.ThemeColorFromCode("#881798", "Purple Red");

        public static readonly ThemeColor BrightBlue = ThemeColor.ThemeColorFromCode("#0099BC", "Bright Blue");
        public static readonly ThemeColor LightBlue = ThemeColor.ThemeColorFromCode("#2D7D9A", "Light Blue");
        public static readonly ThemeColor SeaFoam = ThemeColor.ThemeColorFromCode("#00B7C3", "Sea Foam");
        public static readonly ThemeColor Greeny = ThemeColor.ThemeColorFromCode("#038387", "Greeny");
        public static readonly ThemeColor LightMint = ThemeColor.ThemeColorFromCode("#00B294", "Light Mint");
        public static readonly ThemeColor DarkMint = ThemeColor.ThemeColorFromCode("#018170", "Dark Mint");
        public static readonly ThemeColor Peaty = ThemeColor.ThemeColorFromCode("#00CC6A", "Peaty");
        public static readonly ThemeColor BrightGreen = ThemeColor.ThemeColorFromCode("#10893E", "Bright Green");

        public static readonly ThemeColor Gray = ThemeColor.ThemeColorFromCode("#746F6E", "Gray");
        public static readonly ThemeColor GrayBrown = ThemeColor.ThemeColorFromCode("#5D5A58", "Gray Brown");
        public static readonly ThemeColor SteelBlue = ThemeColor.ThemeColorFromCode("#68768A", "Steel Blue");
        public static readonly ThemeColor MetalicBlue = ThemeColor.ThemeColorFromCode("#515C6B", "Metalic Blue");
        public static readonly ThemeColor PaleDarkGreen = ThemeColor.ThemeColorFromCode("#567C73", "Pale Dark Green");
        public static readonly ThemeColor DarkGreen = ThemeColor.ThemeColorFromCode("#47675F", "Dark Green");
        public static readonly ThemeColor LightGreen = ThemeColor.ThemeColorFromCode("#498205", "Light Green");
        public static readonly ThemeColor Green = ThemeColor.ThemeColorFromCode("#107C10", "Green");

        public static readonly ThemeColor Cloudy = ThemeColor.ThemeColorFromCode("#6B6B6B", "Cloudy");
        public static readonly ThemeColor Storm = ThemeColor.ThemeColorFromCode("#4A4846", "Storm");
        public static readonly ThemeColor BlueGray = ThemeColor.ThemeColorFromCode("#69797E", "Blue Gray");
        public static readonly ThemeColor DarkGray = ThemeColor.ThemeColorFromCode("#464F54", "Dark Gray");
        public static readonly ThemeColor ShadedGreen = ThemeColor.ThemeColorFromCode("#637B63", "Shaded Green");
        public static readonly ThemeColor Sage = ThemeColor.ThemeColorFromCode("#525E54", "Sage");
        public static readonly ThemeColor Desert = ThemeColor.ThemeColorFromCode("#847545", "Desert");
        public static readonly ThemeColor Moro = ThemeColor.ThemeColorFromCode("#766B59", "Moro");


        //  METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get base colors. </summary>
        /// <returns> List of base colors. </returns>
        public static List<ThemeColor> GetBaseColors(bool? allowRemove = null)
        {
            return new List<ThemeColor>()
            {
                GoldYellow.CloneModified(allowRemove: allowRemove),
                Gold.CloneModified(allowRemove: allowRemove),
                BrightOrange.CloneModified(allowRemove: allowRemove),
                DarkOrange.CloneModified(allowRemove: allowRemove),
                Rusty.CloneModified(allowRemove: allowRemove),
                PaleRusty.CloneModified(allowRemove: allowRemove),
                BrickRed.CloneModified(allowRemove: allowRemove),
                ModerateRed.CloneModified(allowRemove: allowRemove),
                PaleRed.CloneModified(allowRemove: allowRemove),
                Red.CloneModified(allowRemove: allowRemove),
                LightPink.CloneModified(allowRemove: allowRemove),
                Rose.CloneModified(allowRemove: allowRemove),
                LightPlum.CloneModified(allowRemove: allowRemove),
                Plum.CloneModified(allowRemove: allowRemove),
                LightlyOrchid.CloneModified(allowRemove: allowRemove),
                Orchid.CloneModified(allowRemove: allowRemove),
                Blue.CloneModified(allowRemove: allowRemove),
                Navy.CloneModified(allowRemove: allowRemove),
                PurpleShade.CloneModified(allowRemove: allowRemove),
                DarkPurpleShade.CloneModified(allowRemove: allowRemove),
                PastelIris.CloneModified(allowRemove: allowRemove),
                BrightlyIridescent.CloneModified(allowRemove: allowRemove),
                LightPurpleRed.CloneModified(allowRemove: allowRemove),
                PurpleRed.CloneModified(allowRemove: allowRemove),
                BrightBlue.CloneModified(allowRemove: allowRemove),
                LightBlue.CloneModified(allowRemove: allowRemove),
                SeaFoam.CloneModified(allowRemove: allowRemove),
                Greeny.CloneModified(allowRemove: allowRemove),
                LightMint.CloneModified(allowRemove: allowRemove),
                DarkMint.CloneModified(allowRemove: allowRemove),
                Peaty.CloneModified(allowRemove: allowRemove),
                BrightGreen.CloneModified(allowRemove: allowRemove),
                Gray.CloneModified(allowRemove: allowRemove),
                GrayBrown.CloneModified(allowRemove: allowRemove),
                SteelBlue.CloneModified(allowRemove: allowRemove),
                MetalicBlue.CloneModified(allowRemove: allowRemove),
                PaleDarkGreen.CloneModified(allowRemove: allowRemove),
                DarkGreen.CloneModified(allowRemove: allowRemove),
                LightGreen.CloneModified(allowRemove: allowRemove),
                Green.CloneModified(allowRemove: allowRemove),
                Cloudy.CloneModified(allowRemove: allowRemove),
                Storm.CloneModified(allowRemove: allowRemove),
                BlueGray.CloneModified(allowRemove: allowRemove),
                DarkGray.CloneModified(allowRemove: allowRemove),
                ShadedGreen.CloneModified(allowRemove: allowRemove),
                Sage.CloneModified(allowRemove: allowRemove),
                Desert.CloneModified(allowRemove: allowRemove),
                Moro.CloneModified(allowRemove: allowRemove)
            };
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get default colors.</summary>
        /// <returns> List of default colors. </returns>
        public static List<ThemeColor> GetDefaultColors(bool? allowRemove = null)
        {
            return new List<ThemeColor>()
            {
                Blue.CloneModified(allowRemove: allowRemove),
                Greeny.CloneModified(allowRemove: allowRemove),
                GrayBrown.CloneModified(allowRemove: allowRemove),
                Red.CloneModified(allowRemove: allowRemove),
                LightPink.CloneModified(allowRemove: allowRemove)
            };
        }

    }
}
