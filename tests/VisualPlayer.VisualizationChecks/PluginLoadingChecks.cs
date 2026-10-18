using System.Diagnostics;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json.Nodes;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Themes.Fluent;
using Avalonia.Threading;
using Avalonia.VisualTree;
using VisualPlayer.Contexts;
using VisualPlayer.Controls;
using VisualPlayer.Interfaces;
using VisualPlayer.UI.Localization;
using VisualPlayer.UI.Views;
using VisualPlayer.VisualizationBase.Localization;
using VisualPlayer.Visualizations.Config.Base;
using VisualPlayer.Visualizations.Data;
using VisualPlayer.Visualizations.Loading;
using VisualPlayer.Visualizations.Registry;
using VisualPlayer.Visualizations.Compatibility;

internal static class PluginLoadingChecks
{
    private const string Peaks = "visualplayer.peaks";
    public static void Run(string? publishedPlugins = null)
    {
        var directory = Path.Combine(Path.GetTempPath(), "VisualPlayer-plugin-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        try
        {
            var packagesRoot = publishedPlugins ?? Path.Combine(AppContext.BaseDirectory, "Plugins");
            CopyPackage(packagesRoot, Path.Combine(directory, "system"));
            Child("--system-plugin-cases", directory);
            Console.WriteLine("PASS: all 16 independent packages; each missing-package startup keeps the other 15 drawing and retains missing profile JSON.");
            var source = Path.Combine(publishedPlugins ?? Path.Combine(AppContext.BaseDirectory, "Plugins"), Peaks);
            CopyPackage(source, Path.Combine(directory, "builtin", "peaks"));
            CopyPackage(source, Path.Combine(directory, "user", "10-selected"));
            CopyPackage(source, Path.Combine(directory, "user", "20-second"));
            var mismatch = Path.Combine(directory, "user", "00-version-mismatch");
            CopyPackage(source, mismatch); Change(mismatch, "version", "2.0.0");
            var broken = Path.Combine(directory, "broken", "peaks");
            CopyPackage(source, broken); File.WriteAllText(Path.Combine(broken, "VisualPlayer.Visualizations.Peaks.dll"), "bad binary");
            var changes = new Dictionary<string, (string Field, string Value)>
            {
                ["api"] = ("apiMajor", "99"), ["avalonia"] = ("avaloniaVersion", "99.0.0"),
                ["framework"] = ("targetFramework", "net8.0"), ["path"] = ("languageDirectory", "../Lang"),
                ["absolute"] = ("entryAssembly", "C:/outside.dll"), ["type"] = ("entryType", "Missing.Entry"),
                ["manifest"] = ("manifestVersion", "99")
            };
            foreach (var change in changes)
            {
                var package = Path.Combine(directory, "invalid", change.Key);
                CopyPackage(source, package); Change(package, change.Value.Field, change.Value.Value);
            }
            var noDeps = Path.Combine(directory, "invalid", "no-deps");
            CopyPackage(source, noDeps); File.Delete(Path.Combine(noDeps, "VisualPlayer.Visualizations.Peaks.deps.json"));
            var throwing = Path.Combine(directory, "invalid", "constructor");
            Directory.CreateDirectory(throwing);
            File.Copy(Path.Combine(AppContext.BaseDirectory, "VisualPlayer.SamplePlugin.dll"), Path.Combine(throwing, "VisualPlayer.SamplePlugin.dll"));
            var manifest = JsonNode.Parse(File.ReadAllText(Path.Combine(source, "manifest.json")))!;
            manifest["id"] = "tests.sample"; manifest["entryAssembly"] = "VisualPlayer.SamplePlugin.dll";
            manifest["entryType"] = "VisualPlayer.SamplePlugin.ThrowingPlugin";
            File.WriteAllText(Path.Combine(throwing, "manifest.json"), manifest.ToJsonString());
            File.WriteAllText(Path.Combine(throwing, "VisualPlayer.SamplePlugin.deps.json"), """{"runtimeTarget":{"name":".NETCoreApp,Version=v10.0"},"targets":{".NETCoreApp,Version=v10.0":{}},"libraries":{}}""");
            // Each child closes all package files before the parent removes its unique fixture directory.
            Child("--plugin-cases", directory);
            Child("--plugin-restart", directory);
        }
        finally { Directory.Delete(directory, recursive: true); }
        Console.WriteLine("PASS: external Peaks DLL, shared type identity, private dependency, deterministic selection/fallback, manifest/constructor failures, independent language/editor/color/config and actual process restart.");
    }
    public static void SystemCases(string directory)
    {
        var root = Path.Combine(directory, "system");
        foreach (var host in new[] { typeof(VisualPlayer.App).Assembly, typeof(VisualizationRegistry).Assembly,
                     typeof(IVisualization).Assembly, typeof(VisualisationColorEditor).Assembly })
            Check(!host.GetReferencedAssemblies().Any(a => a.Name!.StartsWith("VisualPlayer.Visualizations.", StringComparison.Ordinal)),
                "A host/shared assembly references a concrete visualization.");
        var baseline = JsonNode.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", "default-profiles.json")))!.AsArray();
        var originals = baseline.Where(p => LegacyVisualizationIds.FromLegacy(p!["type"]) != VisualizationRegistry.NoneId).ToArray();
        Check(originals.Length == 16, "Expected all system visualization fixtures.");
        var spectrum = Enumerable.Repeat(.8f, 128).ToArray();
        foreach (var missing in originals)
        {
            var id = LegacyVisualizationIds.FromLegacy(missing!["type"])!;
            var manifest = Path.Combine(root, id, "manifest.json");
            var hidden = manifest + ".disabled";
            File.Move(manifest, hidden);
            try
            {
                var language = new TranslationService();
                using var registry = new VisualizationRegistry();
                using var loader = Loader(registry, language, UiCatalogs.Register(language));
                loader.Load([new(root, VisualizationPackageSource.BuiltIn)]);
                Check(loader.LoadedPackages.Count == 15 && loader.Diagnostics.Count == 0 && registry.Find(id) is null,
                    $"Removing {id} prevented loading another package: {string.Join("; ", loader.Diagnostics.Select(d => d.Exception))}");
                var settingsPath = Path.Combine(directory, "missing-settings.json");
                File.WriteAllText(settingsPath, new JsonObject
                {
                    ["selectedProfileName"] = missing["name"]!.DeepClone(), ["profiles"] = baseline.DeepClone()
                }.ToJsonString());
                using var settings = VisualizationConfigurationContext.Load(settingsPath, registry);
                var unavailable = settings.SelectedProfile;
                Check(unavailable.PluginId == id && !unavailable.IsAvailable && settings.Profiles.Count == 17, "Missing package lost selection or profile.");
                var copy = unavailable.Copy("Unavailable copy");
                Check(JsonNode.DeepEquals(copy.ToJson()["config"], missing["config"]), "Missing system plugin copy lost configuration.");
                settings.Save();
                using var reloaded = VisualizationConfigurationContext.Load(settingsPath, registry);
                Check(!reloaded.SelectedProfile.IsAvailable && reloaded.SelectedProfile.PluginId == id
                    && JsonNode.DeepEquals(reloaded.SelectedProfile.ToJson()["config"], missing["config"]), "Missing system package configuration lost after save/reload.");
                foreach (var profile in settings.Profiles.Where(p => p.IsAvailable && p.PluginId != VisualizationRegistry.NoneId))
                {
                    var registration = profile.Registration!;
                    var assembly = registration.Plugin.GetType().Assembly;
                    Check(!assembly.GetReferencedAssemblies().Any(a => a.Name is "VisualPlayer" or "VisualPlayer.Visualizations"
                        || a.Name!.StartsWith("VisualPlayer.Visualizations.", StringComparison.Ordinal)), "System plugin depends on host/runtime/another visualization.");
                    Check(AssemblyLoadContext.GetLoadContext(assembly) != AssemblyLoadContext.Default, "System plugin statically loaded.");
                    using var visualization = registration.Plugin.CreateVisualization(profile.Config!, profile.GetColorConfig(), registration.Services);
                    visualization.UpdateGraphics(new Rect(0, 0, 320, 200), 1);
                    var frame = new VisualizationFrame(TimeSpan.FromSeconds(1d / 60), spectrum, spectrum, true);
                    visualization.PreCalculate(frame);
                    using var context = new DrawingGroup().Open();
                    visualization.Draw(context, Brushes.Lime);
                    visualization.Reset();
                }
            }
            finally { File.Move(hidden, manifest); }
        }
        Check(!AssemblyLoadContext.Default.Assemblies.Any(a => a.GetName().Name!.StartsWith("VisualPlayer.Visualizations.", StringComparison.Ordinal)),
            "A system visualization leaked into the default assembly context.");
    }
    public static void Cases(string directory)
    {
        var roots = new[] { new VisualizationPluginRoot(Path.Combine(directory, "builtin"), VisualizationPackageSource.BuiltIn),
            new VisualizationPluginRoot(Path.Combine(directory, "user"), VisualizationPackageSource.User),
            new VisualizationPluginRoot(Path.Combine(directory, "invalid"), VisualizationPackageSource.User) };
        var language = new TranslationService();
        var common = UiCatalogs.Register(language);
        using var registry = new VisualizationRegistry();
        using var loader = Loader(registry, language, common);
        var discovered = loader.Discover(roots);
        Check(discovered.Any(p => p.Manifest.Id == Peaks) && registry.Find(Peaks) is null, "Discovery executed plugin registration.");
        Check(!AssemblyLoadContext.Default.Assemblies.Any(a => a.GetName().Name == "VisualPlayer.Visualizations.Peaks"), "Peaks statically loaded into host.");
        loader.Load(roots.Reverse());
        Check(loader.LoadedPackages.Count == 1 && loader.LoadedPackages[0].Directory.EndsWith("10-selected"), "User priority/version fallback/ordinal selection failed.");
        Check(loader.Diagnostics.Count >= 8 && registry.Find("tests.sample") is null, "Malformed/constructor-failing plugin not diagnosed.");
        var entry = registry.Find(Peaks)!;
        var assembly = entry.Plugin.GetType().Assembly;
        var context = AssemblyLoadContext.GetLoadContext(assembly)!;
        Check(context != AssemblyLoadContext.Default, "Plugin does not have its own load context.");
        Check(!assembly.GetReferencedAssemblies().Any(a => a.Name is "VisualPlayer" or "VisualPlayer.Visualizations"), "Plugin references host/runtime.");
        Check(!typeof(VisualPlayer.App).Assembly.GetReferencedAssemblies().Any(a => a.Name == "VisualPlayer.Visualizations.Peaks"), "Host references Peaks.");
        Check(typeof(VisualPlayer.App).Assembly.GetType("VisualPlayer.Visualizations.PeaksVisualization") is null, "Old implementation remains in app.");
        Check(ReferenceEquals(context.LoadFromAssemblyName(typeof(IVisualization).Assembly.GetName()), typeof(IVisualization).Assembly), "SDK identity duplicated.");
        Check(ReferenceEquals(context.LoadFromAssemblyName(typeof(Control).Assembly.GetName()), typeof(Control).Assembly), "Avalonia identity duplicated.");
        Check(ReferenceEquals(context.LoadFromAssemblyName(typeof(VisualisationColorEditor).Assembly.GetName()), typeof(VisualisationColorEditor).Assembly), "UI identity duplicated.");
        var privateDependency = context.LoadFromAssemblyName(new AssemblyName("MicroCom.Runtime"));
        Check(AssemblyLoadContext.GetLoadContext(privateDependency) == context, "Private dependency escaped package context.");
        using var settings = new VisualizationConfigurationContext(registry);
        var profile = settings.Profiles.Single(p => p.PluginId == Peaks).Copy("External Peaks");
        settings.Profiles.Add(profile); settings.SelectedProfileId = profile.ProfileId;
        var count = profile.Config!.GetType().GetProperty("PeaksCount")!;
        count.SetValue(profile.Config, 87);
        profile.FillScreen = true;
        profile.GetColorConfig().Mode = VisualPlayer.Enums.VisualizationColorMode.Solid;
        profile.Color!.SolidColor = new("#123456", "Test");
        using (var drawing = new DrawingGroup().Open())
        using (var visualization = entry.Plugin.CreateVisualization(profile.Config, profile.Color, entry.Services))
        {
            visualization.UpdateGraphics(new Rect(0, 0, 640, 360), 1.5);
            var frame = new VisualizationFrame(TimeSpan.FromSeconds(1d / 60), Enumerable.Repeat(.8f, 2048).ToArray(), new float[2048], true);
            visualization.PreCalculate(frame); visualization.Draw(drawing, Brushes.Lime);
        }
        var app = Application.Current!;
        app.Styles.Add(new FluentTheme());
        app.Resources.MergedDictionaries.Add((ResourceDictionary)AvaloniaXamlLoader.Load(new Uri("avares://VisualPlayer.UI/Themes/Generic.axaml")));
        using var resources = UiResources.BindTranslations(app.Resources, common);
        var editor = entry.CreateEditor(profile)!;
        var window = new Window { Width = 900, Height = 800, Content = editor };
        window.Show(); Layout(window);
        var title = editor.GetVisualDescendants().OfType<SettingsControl>().First(c => c.Title is string text && text.Length > 0);
        var en = title.Title;
        language.SelectLanguage("pl-PL"); Layout(window);
        Check(!Equals(en, title.Title) && !((string)title.Title!).StartsWith("VIEW_"), "DLL-local Polish editor labels did not refresh.");
        Check((int)count.GetValue(profile.Config)! == 87 && profile.Color.SolidColor.HexCode == "#123456", "Opening/localizing external editor replaced config/colors.");
        settings.Save(Path.Combine(directory, "settings.json"));
        window.Close();
        // Broken user binary must fall back to the built-in package; missing language directory keeps English.
        var fallbackLanguage = new TranslationService("pl-PL");
        var fallbackCommon = UiCatalogs.Register(fallbackLanguage);
        using var fallbackRegistry = new VisualizationRegistry();
        using var fallback = Loader(fallbackRegistry, fallbackLanguage, fallbackCommon);
        fallback.Load([roots[0], new(Path.Combine(directory, "broken"), VisualizationPackageSource.User)]);
        Check(fallback.LoadedPackages.Single().Source == VisualizationPackageSource.BuiltIn && fallback.Diagnostics.Count > 0, "Bad user DLL prevented built-in fallback.");
        var fallbackPackage = Path.Combine(directory, "builtin", "peaks");
        File.Delete(Path.Combine(fallbackPackage, "Lang", "pl-PL.json"));
        var englishLanguage = new TranslationService("pl-PL");
        using var englishRegistry = new VisualizationRegistry();
        using var englishLoader = Loader(englishRegistry, englishLanguage, UiCatalogs.Register(englishLanguage));
        englishLoader.Load([roots[0]]);
        Check(englishRegistry.Find(Peaks)!.Services.Translations["VIEW_VISUALIZATIONS_NUM_TITLE"] == "Number of peaks", "Missing language file does not fall back to DLL English.");
    }
    public static void Restart(string directory)
    {
        var language = new TranslationService("pl-PL");
        using var registry = new VisualizationRegistry();
        using var loader = Loader(registry, language, UiCatalogs.Register(language));
        loader.Load([new(Path.Combine(directory, "user"), VisualizationPackageSource.User)]);
        using var settings = VisualizationConfigurationContext.Load(Path.Combine(directory, "settings.json"), registry);
        var profile = settings.SelectedProfile;
        Check(profile.Name == "External Peaks" && profile.PluginId == Peaks && profile.IsAvailable && profile.FillScreen, "Restart lost external profile/selection.");
        Check((int)profile.Config!.GetType().GetProperty("PeaksCount")!.GetValue(profile.Config)! == 87 && profile.Color!.SolidColor.HexCode == "#123456", "Restart lost plugin configuration/colors.");
        using var visualization = profile.Registration!.Plugin.CreateVisualization(profile.Config, profile.GetColorConfig(), profile.Registration.Services);
        Check(visualization is IVisualization, "Restart did not create DLL visualization.");
        settings.Save();
    }
    private static VisualizationPluginLoader Loader(VisualizationRegistry registry, TranslationService language, TranslationCatalog common)
        => new(registry, language, common, [typeof(VisualisationColorEditor).Assembly]);
    private static void Child(string mode, string directory)
    {
        var start = new ProcessStartInfo(Environment.ProcessPath!) { UseShellExecute = false, CreateNoWindow = true, RedirectStandardOutput = true, RedirectStandardError = true };
        if (string.Equals(Path.GetFileNameWithoutExtension(Environment.ProcessPath),"dotnet", StringComparison.OrdinalIgnoreCase))
            start.ArgumentList.Add(Path.Combine(AppContext.BaseDirectory, "VisualPlayer.VisualizationChecks.dll"));
        start.ArgumentList.Add(mode); start.ArgumentList.Add(directory);
        using var process = Process.Start(start)!;
        var output = process.StandardOutput.ReadToEndAsync(); var errors = process.StandardError.ReadToEndAsync();
        if (!process.WaitForExit(45000)) { process.Kill(entireProcessTree: true); throw new TimeoutException("Plugin child process did not exit."); }
        Check(process.ExitCode == 0, $"{mode}: {output.GetAwaiter().GetResult()} {errors.GetAwaiter().GetResult()}");
    }
    private static void CopyPackage(string source, string destination)
    {
        foreach (var file in Directory.EnumerateFiles(source, "*", SearchOption.AllDirectories))
        {
            var target = Path.Combine(destination, Path.GetRelativePath(source, file));
            Directory.CreateDirectory(Path.GetDirectoryName(target)!); File.Copy(file, target);
        }
    }
    private static void Change(string package, string field, string value)
    {
        var path = Path.Combine(package, "manifest.json"); var json = JsonNode.Parse(File.ReadAllText(path))!;
        json[field] = field is "manifestVersion" or "apiMajor" ? JsonValue.Create(int.Parse(value)) : JsonValue.Create(value);
        File.WriteAllText(path, json.ToJsonString());
    }
    private static void Layout(Window window) { window.UpdateLayout(); Dispatcher.UIThread.RunJobs(); }
    private static void Check(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
}
