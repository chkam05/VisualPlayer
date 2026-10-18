using System.ComponentModel;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Markup.Xaml;
using Avalonia.LogicalTree;
using Avalonia.Platform;
using Avalonia.Media;
using Avalonia.Themes.Fluent;
using Avalonia.Threading;
using Avalonia.VisualTree;
using VisualPlayer.Components;
using VisualPlayer.Controls;
using VisualPlayer.Enums;
using VisualPlayer.Interfaces;
using VisualPlayer.SampleUiPlugin;
using VisualPlayer.UI.Localization;
using VisualPlayer.UI.Views;
using VisualPlayer.VisualizationBase.Localization;
using VisualPlayer.VisualizationBase.Plugins;
using VisualPlayer.Visualizations.Config.Base;

try
{
    AppBuilder.Configure<Application>().UseHeadless(new AvaloniaHeadlessPlatformOptions()).SetupWithoutStarting();
    var app = Application.Current!;
    app.Styles.Add(new FluentTheme());
    var resources = (ResourceDictionary)AvaloniaXamlLoader.Load(new Uri("avares://VisualPlayer.UI/Themes/Generic.axaml"));
    app.Resources.MergedDictionaries.Add(resources);
    var translations = new TranslationService();
    var common = UiCatalogs.Register(translations);
    using var binding = UiResources.BindTranslations(app.Resources, common);
    var first = translations.RegisterScope("first", new Dictionary<string,string> { ["NAME"] = "First", ["MISSING_PL"] = "Fallback" });
    var second = translations.RegisterScope("second", new Dictionary<string,string> { ["NAME"] = "Second" });
    first.AddLanguage("pl-PL", new Dictionary<string,string> { ["NAME"] = "Pierwszy", ["MISSING_PL"] = " " });
    second.AddLanguage("pl-PL", new Dictionary<string,string> { ["NAME"] = "Drugi" });
    var config = new TestConfig();
    var color = new VisualizationColorConfig();
    var services = new TestServices(first, common);
    var context = new VisualizationEditorContext(config, color, services);
    var editor = new SampleEditor(context);
    var window = new Window { Width = 900, Height = 1200, Content = editor };
    window.Show();
    Layout(window);
    var shared = editor.GetVisualDescendants().OfType<VisualisationColorEditor>().Single();
    var expand = shared.GetVisualDescendants().OfType<SettingsExpandControl>().Single();
    expand.IsExpanded = true;
    Layout(window);
    Check(ReferenceEquals(shared.EditorContext!.Color, color), "Shared editor replaced the live color model.");
    Check(editor.FindControl<TextBlock>("LocalTitle")!.Text == "First", "Local plugin translation binding failed.");
    var before = expand.Title;
    translations.SelectLanguage("pl-PL");
    Layout(window);
    Check(editor.FindControl<TextBlock>("LocalTitle")!.Text == "Pierwszy", "Open plugin editor did not change language.");
    Check(expand.Title != before && expand.Title == common["VIEW_VISUALIZATIONS_COLOR_TITLE_COLOR"], "Shared editor title did not change language.");
    Check(first["NAME"] == "Pierwszy" && second["NAME"] == "Drugi", "Plugin catalogs collide.");
    Check(first["MISSING_PL"] == "Fallback", "Blank/missing localized value must use English.");
    Check(first["UNKNOWN"] == "UNKNOWN" && first.LoadErrors.Any(e => e.Contains("first") && e.Contains("UNKNOWN")), "Missing-key diagnostics lack catalog identity.");
    var configEvents = 0;
    color.PropertyChanged += (_, _) => configEvents++;
    translations.SelectLanguage("en-US");
    Layout(window);
    Check(configEvents == 0, "Language change modified configuration.");

    var setting = shared.GetVisualDescendants().OfType<SettingsControl>().First(s => s is not SettingsExpandControl);
    app.Resources["UI.ThemeShadeBackgroundBrush"] = Brushes.Red;
    app.Resources["UI.ThemeBackgroundBrush"] = Brushes.Red;
    Layout(window);
    Check(setting.Background is ISolidColorBrush background && background.Color == Colors.Red, "Open editor did not respond to dynamic theme resource.");
    var modeBox = shared.GetVisualDescendants().OfType<ComboBox>().First();
    modeBox.SelectedValue = VisualizationColorMode.Gradient;
    Layout(window);
    Check(color.Mode == VisualizationColorMode.Gradient, "Color mode does not write back to live configuration.");
    var gradient = shared.GetVisualDescendants().OfType<GradientControl>().Single();
    Check(ReferenceEquals(gradient.Gradient, color.Gradient), "Gradient editor lost configuration.");
    var selectedLabels = modeBox.GetVisualDescendants().OfType<TextBlock>().Select(t => t.Text).ToArray();
    Check(selectedLabels.Contains(common["ENUM_VISUALIZATIONCOLORMODE_GRADIENT"]), "Enum converter did not receive the catalog.");
    translations.SelectLanguage("pl-PL");
    Layout(window);
    Check(modeBox.GetVisualDescendants().OfType<TextBlock>().Any(t => t.Text == common["ENUM_VISUALIZATIONCOLORMODE_GRADIENT"]), "Selected enum label did not refresh language.");
    Check(color.Mode == VisualizationColorMode.Gradient, "Enum relocalization changed the selected value.");
    var gradientBox = gradient.GetVisualDescendants().OfType<ComboBox>().Single();
    Check(gradientBox.GetVisualDescendants().OfType<TextBlock>().Any(t => t.Text == common["ENUM_GRADIENTTYPE_LINEAR"]), "Gradient enum label did not refresh language.");
    gradientBox.IsDropDownOpen = true;
    Layout(window);
    var popup = (Popup)typeof(ComboBox).GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).First(field => field.FieldType == typeof(Popup)).GetValue(gradientBox)!;
    translations.SelectLanguage("en-US");
    Layout(window);
    Check(popup.Child!.GetVisualDescendants().OfType<TextBlock>().Any(t => t.Text == "Linear"), "Open gradient dropdown did not refresh language.");
    translations.SelectLanguage("pl-PL");
    Layout(window);
    Check(popup.Child!.GetVisualDescendants().OfType<TextBlock>().Any(t => t.Text == common["ENUM_GRADIENTTYPE_LINEAR"]), "Open gradient dropdown did not show Polish labels.");
    gradientBox.IsDropDownOpen = false;
    var palette = shared.GetVisualDescendants().OfType<ColorPaletteControl>().First();
    Check(palette.PaletteItems.Count > 0, "Shared palette is empty.");
    var pointCount = gradient.Gradient.Points.Count;
    gradient.GetVisualDescendants().OfType<Button>().First(b => b.Name == "PART_AddPointButton").RaiseEvent(new Avalonia.Interactivity.RoutedEventArgs(Button.ClickEvent));
    Check(gradient.Gradient.Points.Count == pointCount + 1, "Gradient add-point action was lost during extraction.");
    var oldPoints = gradient.Gradient.Points;
    gradient.Gradient.Points = new(oldPoints.Select(point => point.Copy()));
    var preview = gradient.GradientBrush;
    oldPoints[0].Position = .2;
    Check(ReferenceEquals(preview, gradient.GradientBrush), "Gradient editor is still subscribed to replaced points.");
    window.Content = null;
    var detachedPreview = gradient.GradientBrush;
    gradient.Gradient.Points[0].Position = .3;
    Check(ReferenceEquals(detachedPreview, gradient.GradientBrush), "Detached gradient editor still processes model events.");
    window.Content = editor;
    Layout(window);
    Check(!ReferenceEquals(detachedPreview, gradient.GradientBrush), "Reattached gradient editor did not rebuild.");
    var replacement = new VisualizationColorConfig { Mode = VisualizationColorMode.Rainbow };
    shared.EditorContext = new VisualizationEditorContext(config, replacement, services);
    Layout(window);
    Check(replacement.Mode == VisualizationColorMode.Rainbow && color.Mode == VisualizationColorMode.Gradient, "Changing editor context corrupted color mode.");
    var numeric = new VisualPlayer.Components.NumericUpDown { Value = 5, Minimum = 0, Maximum = 10 };
    numeric.Value = 100;
    Check(numeric.Value == 10, "NumericUpDown clamping changed.");
    Check(typeof(ScrollTextBlock).Assembly.GetName().Name == "VisualPlayer.UI", "ScrollTextBlock was not extracted.");
    var scrolling = editor.GetVisualDescendants().OfType<ScrollTextBlock>().Single();
    Check(scrolling.GetVisualDescendants().OfType<Canvas>().Any(), "ScrollTextBlock template was not loaded from UI resources.");
    scrolling.Text = new string('W', 1000);
    Layout(window);
    var scrollTimer = (DispatcherTimer)typeof(ScrollTextBlock).GetField("_animationTimer",
        System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(scrolling)!;
    Check(scrollTimer.IsEnabled, "Extracted ScrollTextBlock does not animate overflow text.");
    scrolling.Text = "Short";
    Layout(window);
    Check(!scrollTimer.IsEnabled, "Extracted ScrollTextBlock lost AnimateIfTextTooLarge behavior.");
    Check(!typeof(SampleEditor).Assembly.GetReferencedAssemblies().Any(a => a.Name is "VisualPlayer" or "VisualPlayer.Visualizations"), "External editor references host/runtime.");
    Check(!AppDomain.CurrentDomain.GetAssemblies().Any(a => a.GetName().Name == "VisualPlayer"), "Standalone UI test initialized the application assembly.");
    foreach (var font in new[] { "Oxanium.ttf", "led_counter-7.ttf", "ledsled.ttf" })
    {
        using var stream = AssetLoader.Open(new Uri("avares://VisualPlayer.UI/Assets/Fonts/" + font));
        Check(stream.Length > 0, "Missing packaged font: " + font);
    }

    using (var malformed = new MemoryStream(Encoding.UTF8.GetBytes("{not-json}")))
    {
        try { first.LoadLanguage(malformed); throw new Exception("Invalid language accepted."); }
        catch (System.Text.Json.JsonException) { }
    }
    first.LoadDirectory(Path.Combine(AppContext.BaseDirectory, "missing-language-directory"));
    Check(first["MISSING_PL"] == "Fallback", "Missing directory destroyed fallback.");
    var late = translations.RegisterScope("late", new Dictionary<string,string> { ["NAME"] = "Late" });
    Check(late.CurrentLanguageCode == "pl-PL", "New catalog does not inherit the selected language.");
    window.Close();
    Console.WriteLine("PASS: SDK/UI-only editor, shared resources and fonts, live language/theme, enum labels, independent catalogs, fallback, live color/gradient editing and components.");
}
catch (Exception exception)
{
    Console.Error.WriteLine(exception);
    Environment.ExitCode = 1;
}

static void Layout(Window window) { Dispatcher.UIThread.RunJobs(); window.UpdateLayout(); }
static void Check(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }

sealed class TestConfig : IVisualizationConfig
{
    public event PropertyChangedEventHandler? PropertyChanged { add { } remove { } }
}
sealed class TestServices(ITranslationScope local, ITranslationScope common) : IVisualizationHostServices
{
    public ITranslationScope Translations => local;
    public ITranslationScope CommonTranslations => common;
    public void ReportDiagnostic(VisualizationDiagnosticLevel level, string message, Exception? exception = null) { }
}
