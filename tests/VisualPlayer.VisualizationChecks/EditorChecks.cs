using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Markup.Xaml;
using Avalonia.Themes.Fluent;
using Avalonia.Threading;
using Avalonia.VisualTree;
using VisualPlayer.Contexts;
using VisualPlayer.Controls;
using VisualPlayer.UI.Localization;
using VisualPlayer.UI.Views;
using VisualPlayer.VisualizationBase.Localization;
using VisualPlayer.VisualizationBase.Plugins;
using VisualPlayer.Visualizations.Registry;

internal static class EditorChecks
{
    public static void Run(VisualizationConfigurationContext settings, TranslationService language, TranslationCatalog common)
    {
        var app = Application.Current!;
        app.Styles.Add(new FluentTheme());
        app.Resources.MergedDictionaries.Add((ResourceDictionary)AvaloniaXamlLoader.Load(new Uri("avares://VisualPlayer.UI/Themes/Generic.axaml")));
        using var translations = UiResources.BindTranslations(app.Resources, common);
        var window = new Window { Width = 900, Height = 800 };
        window.Show();
        foreach (var profile in settings.Profiles.Where(p => p.PluginId != VisualizationRegistry.NoneId))
        {
            var editor = profile.Registration!.CreateEditor(profile)!;
            Check(editor.DataContext is VisualizationEditorContext context && ReferenceEquals(context.Configuration, profile.Config), "Editor depends on runtime profile or replaced live config.");
            window.Content = editor;
            Layout(window);
            var shared = editor.GetVisualDescendants().OfType<VisualisationColorEditor>().Single();
            Check(ReferenceEquals(shared.EditorContext!.Color, profile.Color), "Shared color editor lost profile color.");
            var title = editor.GetVisualDescendants().OfType<SettingsControl>().First(c => c.Title is string text && text.Length > 0);
            Check(!((string)title.Title!).StartsWith("VIEW_"), "Editor displays untranslated key.");
            var englishTitle = title.Title;
            var localBox = editor.GetVisualDescendants().OfType<ComboBox>()
                .FirstOrDefault(box => !box.GetVisualAncestors().Contains(shared));
            Popup? popup = null;
            object? selectedValue = null;
            string? englishOption = null;
            if (localBox is not null)
            {
                var option = localBox.ItemsSource!.Cast<object>().Last();
                selectedValue = option.GetType().GetProperty("Value")!.GetValue(option)!;
                localBox.SelectedValue = selectedValue;
                Layout(window);
                var configProperty = profile.Config!.GetType().GetProperties().Single(p => p.PropertyType == selectedValue.GetType());
                Check(Equals(configProperty.GetValue(profile.Config), selectedValue), "Local enum selection did not update plugin configuration.");
                localBox.IsDropDownOpen = true;
                Layout(window);
                popup = (Popup)typeof(ComboBox).GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                    .First(field => field.FieldType == typeof(Popup)).GetValue(localBox)!;
                englishOption = popup.Child!.GetVisualDescendants().OfType<TextBlock>().First(t => !string.IsNullOrWhiteSpace(t.Text)).Text;
                Check(!englishOption!.StartsWith("ENUM_"), "Dropdown displays an untranslated enum key.");
            }
            var expand = shared.GetVisualDescendants().OfType<SettingsExpandControl>().Single();
            expand.IsExpanded = true;
            var en = expand.Title;
            language.SelectLanguage("pl-PL");
            Layout(window);
            Check(!Equals(expand.Title, en) && expand.IsExpanded, "Language switch failed or reset editor state.");
            Check(!Equals(title.Title, englishTitle), "Plugin-local settings title did not change language.");
            if (popup is not null)
            {
                Check(!popup.Child!.GetVisualDescendants().OfType<TextBlock>().Any(t => t.Text == englishOption), $"Open local dropdown did not refresh language: {profile.PluginId}, '{englishOption}', labels: {string.Join(", ", popup.Child!.GetVisualDescendants().OfType<TextBlock>().Select(t => t.Text))}");
                Check(Equals(localBox!.SelectedValue, selectedValue), "Relocalization changed selected enum value.");
                localBox.IsDropDownOpen = false;
            }
            language.SelectLanguage("en-US");
            window.Content = null;
        }
        CheckDynamicSelection(settings, window);
        window.Close();
        Console.WriteLine("PASS: all 16 built-in editors use SDK contexts/live colors without application singleton; common language changes preserve expanded state.");
    }
    private static void CheckDynamicSelection(VisualizationConfigurationContext settings, Window window)
    {
        var original = settings.Registry.Find("visualplayer.peaks")!;
        var copy = settings.Profiles.First(p => p.PluginId == original.Id).Copy("Selector test");
        settings.Profiles.Add(copy);
        var combo = new ComboBox
        {
            ItemsSource = settings.Registry.Available
        };
        combo.Bind(ComboBox.SelectedItemProperty, new ReflectionBinding("Registration") { Source = copy, Mode = BindingMode.TwoWay });
        window.Content = combo;
        Layout(window);
        Check(ReferenceEquals(combo.SelectedItem, original), "Dynamic selector did not select plugin by ID.");
        settings.Registry.Unregister(original.Id);
        Layout(window);
        Check(copy.PluginId == original.Id && !copy.IsAvailable, "Removing a combo item changed the saved plugin ID.");
        settings.Registry.Register(original.Plugin, original.Services);
        Layout(window);
        Check(copy.IsAvailable && ReferenceEquals(combo.SelectedItem, settings.Registry.Find(original.Id)), "Dynamic selector did not restore selection.");
        combo.SelectedItem = settings.Registry.Find("visualplayer.stripes");
        Layout(window);
        Check(copy.PluginId == "visualplayer.stripes" && copy.IsAvailable, "Dynamic selection did not create chosen plugin configuration.");
        settings.Profiles.Remove(copy);
    }
    private static void Layout(Window window)
    {
        window.UpdateLayout();
        Dispatcher.UIThread.RunJobs();
    }
    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
