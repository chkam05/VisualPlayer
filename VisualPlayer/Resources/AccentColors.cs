using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VisualPlayer.Models.Config;
using VisualPlayer.ViewModels.Config;

namespace VisualPlayer.Resources;

public static class AccentColors
{
    #region COLORS

    public static readonly AccentColor GoldYellow = new("#FFB900", "Gold Yellow");
    public static readonly AccentColor Gold = new("#FF8C00", "Gold");
    public static readonly AccentColor BrightOrange = new("#F7630C", "Bright Orange");
    public static readonly AccentColor DarkOrange = new("#C24D0F", "Dark Orange");
    public static readonly AccentColor Rusty = new("#D53A01", "Rusty");
    public static readonly AccentColor PaleRusty = new("#EF6950", "Pale Rusty");
    public static readonly AccentColor BrickRed = new("#CF3438", "Brick Red");
    public static readonly AccentColor ModerateRed = new("#F94141", "Moderate Red");

    public static readonly AccentColor PaleRed = new("#E74856", "Pale Red");
    public static readonly AccentColor Red = new("#E81123", "Red");
    public static readonly AccentColor LightPink = new("#EA005E", "Light Pink");
    public static readonly AccentColor Rose = new("#BA004E", "Rose");
    public static readonly AccentColor LightPlum = new("#DF0089", "Light Plum");
    public static readonly AccentColor Plum = new("#BA0074", "Plum");
    public static readonly AccentColor LightlyOrchid = new("#C239B3", "Lightly Orchid");
    public static readonly AccentColor Orchid = new("#950084", "Orchid");

    public static readonly AccentColor Blue = new("#0078D7", "Blue");
    public static readonly AccentColor Navy = new("#0063B1", "Navy");
    public static readonly AccentColor PurpleShade = new("#8785CE", "Purple Shade");
    public static readonly AccentColor DarkPurpleShade = new("#6B69D6", "Dark Purple Shade");
    public static readonly AccentColor PastelIris = new("#8562B5", "Pastel Iris");
    public static readonly AccentColor BrightlyIridescent = new("#704BA4", "Brightly Iridescent");
    public static readonly AccentColor LightPurpleRed = new("#AD44BD", "Light Purple Red");
    public static readonly AccentColor PurpleRed = new("#881798", "Purple Red");

    public static readonly AccentColor BrightBlue = new("#0099BC", "Bright Blue");
    public static readonly AccentColor LightBlue = new("#2D7D9A", "Light Blue");
    public static readonly AccentColor SeaFoam = new("#00B7C3", "Sea Foam");
    public static readonly AccentColor Greeny = new("#038387", "Greeny");
    public static readonly AccentColor LightMint = new("#00B294", "Light Mint");
    public static readonly AccentColor DarkMint = new("#018170", "Dark Mint");
    public static readonly AccentColor Peaty = new("#00CC6A", "Peaty");
    public static readonly AccentColor BrightGreen = new("#10893E", "Bright Green");

    public static readonly AccentColor Gray = new("#746F6E", "Gray");
    public static readonly AccentColor GrayBrown = new("#5D5A58", "Gray Brown");
    public static readonly AccentColor SteelBlue = new("#68768A", "Steel Blue");
    public static readonly AccentColor MetalicBlue = new("#515C6B", "Metalic Blue");
    public static readonly AccentColor PaleDarkGreen = new("#567C73", "Pale Dark Green");
    public static readonly AccentColor DarkGreen = new("#47675F", "Dark Green");
    public static readonly AccentColor LightGreen = new("#498205", "Light Green");
    public static readonly AccentColor Green = new("#107C10", "Green");

    public static readonly AccentColor Cloudy = new("#6B6B6B", "Cloudy");
    public static readonly AccentColor Storm = new("#4A4846", "Storm");
    public static readonly AccentColor BlueGray = new("#69797E", "Blue Gray");
    public static readonly AccentColor DarkGray = new("#464F54", "Dark Gray");
    public static readonly AccentColor ShadedGreen = new("#637B63", "Shaded Green");
    public static readonly AccentColor Sage = new("#525E54", "Sage");
    public static readonly AccentColor Desert = new("#847545", "Desert");
    public static readonly AccentColor Moro = new("#766B59", "Moro");

    #endregion COLORS

    #region COLLECTIONS

    public static IReadOnlyList<AccentColor> All { get; } =
    [
        GoldYellow, Gold, BrightOrange, DarkOrange, Rusty, PaleRusty, BrickRed, ModerateRed,
        PaleRed, Red, LightPink, Rose, LightPlum, Plum, LightlyOrchid, Orchid,
        Blue, Navy, PurpleShade, DarkPurpleShade, PastelIris, BrightlyIridescent, LightPurpleRed, PurpleRed,
        BrightBlue, LightBlue, SeaFoam, Greeny, LightMint, DarkMint, Peaty, BrightGreen,
        Gray, GrayBrown, SteelBlue, MetalicBlue, PaleDarkGreen, DarkGreen, LightGreen, Green,
        Cloudy, Storm, BlueGray, DarkGray, ShadedGreen, Sage, Desert, Moro
    ];

    public static ObservableCollection<AccentColorViewModel> ToObservableCollection()
    {
        return new ObservableCollection<AccentColorViewModel>(
            All.Select(color => new AccentColorViewModel(color)));
    }

    #endregion COLLECTIONS
}