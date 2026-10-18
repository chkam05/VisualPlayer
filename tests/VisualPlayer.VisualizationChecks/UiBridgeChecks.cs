using Avalonia.Controls;
using Avalonia.Media;
using VisualPlayer.Contexts;
using VisualPlayer.Enums.Config;
using VisualPlayer.Models.Config;
using VisualPlayer.UI.Localization;
using VisualPlayer.ViewModels.Config;
using VisualPlayer.VisualizationBase.Localization;

internal static class UiBridgeChecks
{
    public static void Run()
    {
        // Construct settings without reading the user's files or starting ApplicationContext/audio.
        var config = (ConfigurationContext)Activator.CreateInstance(typeof(ConfigurationContext), nonPublic: true)!;
        var resources = new ResourceDictionary();
        var language = new TranslationService();
        var common = UiCatalogs.Register(language);
        using var bridge = new UiResourceBridge(config, resources, common);
        config.Appearance.AccentColor = new AccentColor("#123456", "Test");
        Check(((ISolidColorBrush)resources["UI.AccentColorBrush"]!).Color == Color.Parse("#123456"), "Accent resource not published.");
        config.Appearance.ColorizationLevel = ColorizationLevel.Poorly;
        Check(ReferenceEquals(resources["UI.Colorized.ThemeShadeBackgroundBrush.AccentColorBrush"], config.Appearance.ThemeShadeBackgroundBrush),
            "Poor colorization did not select the normal brush.");
        config.Appearance.ColorizationLevel = ColorizationLevel.Buttons;
        Check(ReferenceEquals(resources["UI.Colorized.ThemeShadeBackgroundBrush.AccentColorBrush"], config.Appearance.AccentColorBrush),
            "Button colorization did not select the accent brush.");
        config.Appearance.InterfaceOpacity = .6;
        Check(Math.Abs(((ISolidColorBrush)resources["UI.ControlSurfaceBrush"]!).Opacity - .6) < .0001, "Surface opacity changed.");
        var oldAppearance = config.Appearance;
        config.Appearance = new AppearanceConfig();
        var newBrush = resources["UI.AccentColorBrush"];
        oldAppearance.AccentColor = new AccentColor("#FF0000", "Old");
        Check(ReferenceEquals(resources["UI.AccentColorBrush"], newBrush), "Bridge is still subscribed to replaced appearance settings.");
        language.SelectLanguage("pl-PL");
        Check((string)resources["UI.Text.COLORPALETTE_TITLE"]! == "Kolor wiodący", "Host UI language did not propagate.");
        bridge.Dispose();
        config.Appearance.AccentColor = new AccentColor("#FFFFFF", "After disposal");
        language.SelectLanguage("en-US");
        Check(ReferenceEquals(resources["UI.AccentColorBrush"], newBrush), "Bridge leaked an appearance subscription.");
        Check((string)resources["UI.Text.COLORPALETTE_TITLE"]! == "Kolor wiodący", "Bridge leaked a language subscription.");
        Console.WriteLine("PASS: host UI adapter, accent/colorization/opacity, appearance replacement, language propagation and disposal.");
    }
    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
