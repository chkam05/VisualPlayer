using System.Text.Json;
using System.Text.Json.Nodes;
using Avalonia;
using Avalonia.Headless;
using Avalonia.Media;
using VisualPlayer.Contexts;
using VisualPlayer.Enums;
using VisualPlayer.Resources;
using VisualPlayer.Visualizations.Compatibility;
using VisualPlayer.Visualizations.Data;
using VisualPlayer.Visualizations.Registry;
using VisualPlayer.Visualizations.Loading;
using VisualPlayer.Visualizations.Utils;
using VisualPlayer.UI.Localization;
using VisualPlayer.VisualizationBase.Localization;

try
{
    AppBuilder.Configure<Application>().UseHeadless(new AvaloniaHeadlessPlatformOptions()).SetupWithoutStarting();
    if (args is ["--plugin-cases", var casesDirectory]) { PluginLoadingChecks.Cases(casesDirectory); return; }
    if (args is ["--system-plugin-cases", var systemDirectory]) { PluginLoadingChecks.SystemCases(systemDirectory); return; }
    if (args is ["--plugin-restart", var restartDirectory]) { PluginLoadingChecks.Restart(restartDirectory); return; }
    if (args is ["--plugin-published", var publishedPlugins]) { PluginLoadingChecks.Run(publishedPlugins); return; }
    var language = new TranslationService();
    var texts = language.RegisterScope("host", EnglishTranslations.Values);
    using (var polish = File.OpenRead(Path.Combine(AppContext.BaseDirectory, "Lang", "pl-PL.json"))) texts.LoadLanguage(polish);
    var common = UiCatalogs.Register(language);
    var services = new VisualizationHostServices(texts, common);
    using var registry = new VisualizationRegistry(services);
    using var loader = new VisualizationPluginLoader(registry, language, common, [typeof(VisualPlayer.UI.Views.VisualisationColorEditor).Assembly]);
    loader.Load([new(Path.Combine(AppContext.BaseDirectory, "Plugins"), VisualizationPackageSource.BuiltIn)]);
    Check(loader.LoadedPackages.Count == 16, "System packages failed to load: " + string.Join("; ", loader.Diagnostics.Select(d => d.Exception?.ToString() ?? d.Message)));
    using var settings = new VisualizationConfigurationContext(registry);
    var serializer = new VisualizationProfileSerializer(registry);
    var baseline = JsonNode.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", "default-profiles.json")))!.AsArray();
    Check(settings.Profiles.Count == 17, "Expected 17 default profiles.");
    foreach (var original in baseline)
    {
        var legacy = original!.AsObject();
        var expected = LegacyVisualizationIds.FromLegacy(legacy["type"])!;
        var preset = settings.Profiles.Single(p => p.PluginId == expected);
        Check(preset.IsAvailable, $"Default unavailable: {expected}, {preset.ConfigurationError}");
        var current = preset.ToJson();
        foreach (var property in new[] { "name", "isDefault", "fillScreen", "config", "color" })
            Check(JsonNode.DeepEquals(current[property], legacy[property]), $"Default {expected}.{property} differs from pre-migration fixture.");
        foreach (var numeric in new[] { false, true })
        {
            var source = (JsonObject)legacy.DeepClone();
            if (numeric) source["type"] = (int)LegacyVisualizationIds.GetType(expected)!;
            var restored = serializer.Deserialize(source.ToJsonString());
            Check(restored.PluginId == expected && restored.IsAvailable, $"Legacy type migration failed: {source["type"]}");
            var result = restored.ToJson();
            foreach (var property in new[] { "name", "isDefault", "fillScreen", "config", "color" })
                Check(JsonNode.DeepEquals(result[property], legacy[property]), $"Legacy settings changed: {expected}.{property}");
        }
    }
    var spectrum = Enumerable.Range(0, 2048).Select(i => (float)((Math.Sin(i * .03) + 1) / 2)).ToArray();
    var waveform = Enumerable.Range(0, 2048).Select(i => (float)Math.Sin(i * .05)).ToArray();
    foreach (var profile in settings.Profiles.Where(p => p.PluginId != VisualizationRegistry.NoneId))
    {
        var entry = profile.Registration!;
        using var visualization = entry.Plugin.CreateVisualization(profile.Config!, profile.GetColorConfig(), entry.Services);
        foreach (var scaling in new[] { 1d, 1.5d, 2d })
        {
            visualization.UpdateGraphics(new Rect(0, 0, 640, 360), scaling);
            foreach (var playing in new[] { true, false })
            {
                var frame = new VisualizationFrame(TimeSpan.FromSeconds(1d / 60), spectrum, waveform, playing);
                visualization.PreCalculate(frame);
                var drawing = new DrawingGroup();
                using var context = drawing.Open();
                visualization.Draw(context, Brushes.Lime);
            }
            visualization.Reset();
        }
    }
    RuntimeChecks.Run(baseline, registry, services);
    EditorChecks.Run(settings, language, common);
    ContractChecks.Run();
    UiBridgeChecks.Run();
    PluginLoadingChecks.Run();
    Console.WriteLine("PASS: 16 registered visualizations at three scales; 17 original presets; all legacy numeric/string types retain configuration and colors.");
}
catch (Exception exception)
{
    Console.Error.WriteLine(exception);
    Environment.ExitCode = 1;
}

static void Check(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}
