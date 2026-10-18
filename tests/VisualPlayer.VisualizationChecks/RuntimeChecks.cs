using System.Text.Json;
using System.Text.Json.Nodes;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using VisualPlayer.Contexts;
using VisualPlayer.Core;
using VisualPlayer.Interfaces;
using VisualPlayer.SamplePlugin;
using VisualPlayer.VisualizationBase.Plugins;
using VisualPlayer.Visualizations.Config.Base;
using VisualPlayer.Visualizations.Data;
using VisualPlayer.Visualizations.Registry;
using VisualPlayer.Visualizations.Utils;

internal static class RuntimeChecks
{
    public static void Run(JsonArray baseline, VisualizationRegistry builtIns, IVisualizationHostServices services)
    {
        Check(typeof(VisualizationCore).Assembly.GetName().Name == "VisualPlayer.Visualizations", "Runtime was not extracted.");
        Check(!typeof(VisualizationCore).Assembly.GetReferencedAssemblies().Any(a => a.Name is "VisualPlayer" or "VisualPlayer.UI"), "Runtime depends on host/UI.");
        var serializer = new VisualizationProfileSerializer(builtIns);
        var unknown = serializer.Deserialize("""{"name":"Peaks","type":999,"config":{"future":true}}""");
        Check(!unknown.IsAvailable && unknown.PluginId == "legacy.unknown", "Unknown legacy type silently replaced.");
        var invalid = serializer.Deserialize("""{"pluginId":"visualplayer.peaks","configVersion":99,"config":{"future":[1,2]},"color":{"unknown":7},"extension":"retain"}""");
        var invalidCopy = invalid.Copy("Copy");
        Check(!invalid.IsAvailable && !invalidCopy.IsAvailable, "Unsupported config version replaced by defaults.");
        Check(JsonNode.DeepEquals(invalid.ToJson()["config"], invalidCopy.ToJson()["config"]), "Copy lost unsupported config.");
        Check(invalidCopy.ToJson()["extension"]!.GetValue<string>() == "retain", "Copy lost extension fields.");
        var overwritten = serializer.Deserialize(baseline[7]!.ToJsonString()).Copy("Keep name");
        var overwriteId = overwritten.ProfileId;
        overwritten.OverwriteFrom(invalidCopy);
        Check(!overwritten.IsAvailable && overwritten.Name == "Keep name" && overwritten.ProfileId == overwriteId
            && !overwritten.IsDefault && JsonNode.DeepEquals(overwritten.ToJson()["color"], invalid.ToJson()["color"]), "Overwriting lost unsupported settings or target identity.");
        var broken = serializer.Deserialize("""{"pluginId":"visualplayer.peaks","configVersion":1,"config":{"peaksCount":"bad"}}""");
        Check(!broken.IsAvailable && broken.ToJson()["config"]!["peaksCount"]!.GetValue<string>() == "bad", "Malformed config was replaced.");
        var missing = serializer.Deserialize("""{"pluginId":"visualplayer.peaks","configVersion":1}""");
        Check(!missing.IsAvailable, "Missing saved config silently replaced.");

        var directory = Path.Combine(Path.GetTempPath(), "VisualPlayer-runtime-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        try
        {
            var path = Path.Combine(directory, "visualizations.json");
            var legacy = new JsonObject
            {
                ["selectedProfileName"] = "My preset", ["profiles"] = baseline.DeepClone(),
                ["global"] = new JsonObject { ["frameRateLimit"] = 75, ["futureGlobal"] = true }, ["futureRoot"] = "preserve"
            };
            var custom = (JsonObject)baseline[7]!.DeepClone();
            custom["name"] = "My preset"; custom["isDefault"] = false; custom["fillScreen"] = true;
            custom["config"]!["peaksCount"] = 87; custom["config"]!["futureOption"] = new JsonObject { ["nested"] = 42 };
            custom["color"]!["futureColor"] = true;
            legacy["profiles"]!.AsArray().Add(custom);
            var original = legacy.ToJsonString();
            File.WriteAllText(path, original);
            string selectedId;
            using (var migrated = VisualizationConfigurationContext.Load(path, builtIns))
            {
                selectedId = migrated.SelectedProfileId;
                Check(migrated.SelectedProfile.Name == "My preset" && migrated.SelectedProfile.FillScreen, "Migration lost selection/name/fill.");
                Check(migrated.SelectedProfile.ToJson()["config"]!["peaksCount"]!.GetValue<int>() == 87, "Migration lost user parameters.");
                var editedDefault = migrated.Profiles.First(p => p.PluginId == "visualplayer.peaks" && p.IsDefault);
                editedDefault.FillScreen = true;
                migrated.EnsureProfiles();
                Check(editedDefault.FillScreen, "Preset reconciliation replaced edited default.");
                migrated.Profiles.Add(invalid.Copy("Unsupported version"));
                migrated.Save();
                Check(File.ReadAllText(path + ".pre-plugins.bak") == original, "Migration backup differs from original bytes.");
                migrated.Save();
                Check(File.ReadAllText(path + ".pre-plugins.bak") == original, "Backup overwritten on second save.");
            }
            using (var reloaded = VisualizationConfigurationContext.Load(path, builtIns))
            {
                Check(reloaded.SelectedProfileId == selectedId && reloaded.Profiles.Count == 19, "Reload changed stable selection or duplicated defaults.");
                Check(reloaded.SelectedProfile.ToJson()["config"]!["futureOption"]!["nested"]!.GetValue<int>() == 42, "Unknown nested property lost.");
                Check(reloaded.SelectedProfile.ToJson()["color"]!["futureColor"]!.GetValue<bool>(), "Unknown color property lost.");
                var unsupported = reloaded.Profiles.Single(p => p.Name == "Unsupported version");
                Check(!unsupported.IsAvailable && unsupported.ConfigVersion == 99
                    && JsonNode.DeepEquals(unsupported.ToJson()["config"], invalid.ToJson()["config"]), "Unsupported configuration lost on disk round trip.");
            }
            var saved = JsonNode.Parse(File.ReadAllText(path))!;
            Check(saved["formatVersion"]!.GetValue<int>() == 2 && saved["futureRoot"]!.GetValue<string>() == "preserve"
                && saved["global"]!["futureGlobal"]!.GetValue<bool>(), "Root/global extension lost or format version absent.");

            using var registry = new VisualizationRegistry(services);
            var plugin = new SamplePlugin();
            var entry = registry.Register(plugin, services);
            using var config = new VisualizationConfigurationContext(registry);
            var profile = config.Profiles.Single(p => p.PluginId == plugin.Descriptor.Id);
            ((SampleConfig)profile.Config!).Height = 93;
            config.SelectedProfileId = profile.ProfileId;
            var spectrum = new SpectrumProvider();
            using var core = new VisualizationCore(config, spectrum, () => true);
            var instance = (SampleVisualization)core.CurrentVisualization!;
            var previousGraphicsChanges = instance.GraphicsChangeCount;
            core.UpdateGraphics(new Rect(0, 0, 320, 200), 1.5);
            Check(instance.GraphicsChangeCount == previousGraphicsChanges + 1, "Runtime did not forward graphics scaling.");
            ((SampleConfig)profile.Config).Height = 94;
            Check(instance.ConfigurationChangeCount == 1, "Factory lost live profile config.");
            core.UpdateSpectrum();
            Check(core.GetFrequencyLevel(0, 24000) > 0, "Injected spectrum provider not used.");
            core.Reset();
            Check(core.GetFrequencyLevel(0, 24000) == 0, "Runtime reset did not clear spectrum.");
            config.Global = new VisualizationGlobalConfig { FrameRateLimit = 90 };
            Check(spectrum.ConfigureCount == 2, "Replacing global settings did not reconfigure provider.");
            Check(Math.Abs(core.FrameInterval.TotalSeconds - 1d / 90) < .000001, "Configured frame limit ignored.");
            registry.Unregister(entry.Id);
            Check(instance.DisposeCount == 1 && core.CurrentVisualization is null && !profile.IsAvailable, "Missing plugin not disabled/disposed.");
            Check(profile.ToJson()["config"]!["height"]!.GetValue<int>() == 94, "Removing plugin lost live edits.");
            var copy = profile.Copy("Unavailable copy");
            config.Profiles.Add(copy);
            config.Save(path);
            using (var restored = VisualizationConfigurationContext.Load(path, registry))
            {
                var raw = restored.Profiles.Single(p => p.Name == "Unavailable copy");
                Check(!raw.IsAvailable && raw.ToJson()["config"]!["height"]!.GetValue<int>() == 94, "Missing plugin config lost after disk round trip.");
                registry.Register(plugin, services);
                Check(profile.IsAvailable && ((SampleConfig)profile.Config!).Height == 94, "Re-registering plugin did not restore config.");
                Check(raw.IsAvailable && ((SampleConfig)raw.Config!).Height == 94, "Re-registering did not restore copied profile.");
                Check(core.CurrentVisualization is SampleVisualization, "Runtime did not reactivate restored plugin.");
            }
            var customType = copy.Copy("Change type");
            customType.PluginId = VisualizationRegistry.NoneId;
            customType.PluginId = plugin.Descriptor.Id;
            Check(customType.IsAvailable && ((SampleConfig)customType.Config!).Height == 20, "Explicit type change did not create fresh configuration.");

            var failures = new FailingPlugin();
            registry.Register(failures, services);
            var failingProfile = config.Profiles.Single(p => p.PluginId == failures.Descriptor.Id);
            config.SelectedProfileId = failingProfile.ProfileId;
            Check(Math.Abs(core.FrameInterval.TotalSeconds - 1d / 24) < .000001, "Plugin FPS limit ignored.");
            using (var drawing = new DrawingGroup().Open()) { core.Draw(drawing, Brushes.White); core.Draw(drawing, Brushes.White); }
            Check(core.CurrentVisualization is null && failures.Instance!.DrawCount == 1 && failures.Instance.DisposeCount == 1, "Failed plugin keeps rendering or leaks resources.");
            var before = File.ReadAllText(path);
            failures.FailWrite = true;
            try { config.Save(path); throw new InvalidOperationException("Expected serialization failure."); }
            catch (JsonException) { }
            Check(File.ReadAllText(path) == before && !Directory.EnumerateFiles(directory, "*.tmp").Any(), "Failed save damaged last valid settings.");
        }
        finally
        {
            // The freshly created, unique test directory is the only recursive cleanup target.
            Directory.Delete(directory, recursive: true);
        }
        Console.WriteLine("PASS: runtime independence, non-enum plugin factories, migration/backup/atomic save, stable presets/selection, missing/broken profiles, restore, FPS/reset/disposal/error containment.");
    }
    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
    private sealed class SpectrumProvider : IAudioSpectrumProvider
    {
        public int SpectrumSize => 128;
        public int WaveformSize => 128;
        public int ConfigureCount { get; private set; }
        public void Configure(VisualizationGlobalConfig config) => ConfigureCount++;
        public bool TryGetSpectrum(Span<float> destination) { destination.Fill(.5f); return true; }
        public bool TryGetWaveform(Span<float> destination) { destination.Fill(.2f); return true; }
    }
    private sealed class FailingPlugin : IVisualizationPlugin
    {
        private readonly SamplePlugin _inner = new();
        public bool FailWrite { get; set; }
        public FailingVisualization? Instance { get; private set; }
        public VisualizationDescriptor Descriptor => _inner.Descriptor with { Id = "tests.failure" };
        public IReadOnlyDictionary<string, string> EnglishTranslations => _inner.EnglishTranslations;
        public IReadOnlyList<VisualizationPreset> DefaultPresets => _inner.DefaultPresets;
        public IVisualizationConfig CreateDefaultConfiguration() => _inner.CreateDefaultConfiguration();
        public IVisualizationConfig ReadConfiguration(JsonElement config, int version) => _inner.ReadConfiguration(config, version);
        public JsonElement WriteConfiguration(IVisualizationConfig config)
            => FailWrite ? throw new JsonException("Injected write failure") : _inner.WriteConfiguration(config);
        public IVisualization CreateVisualization(IVisualizationConfig config, VisualizationColorConfig color, IVisualizationHostServices services)
            => Instance = new FailingVisualization();
        public Control? CreateEditor(VisualizationEditorContext context) => null;
    }
    private sealed class FailingVisualization : IVisualization
    {
        public int DrawCount { get; private set; }
        public int DisposeCount { get; private set; }
        public bool HasActiveAnimation => false;
        public int MaximumFrameRate => 24;
        public void Draw(DrawingContext context, IBrush brush) { DrawCount++; throw new InvalidOperationException("Injected draw failure"); }
        public void PreCalculate(in VisualizationFrame frame) { }
        public void Reset() { }
        public void UpdateGraphics(Rect bounds, double scaling) { }
        public void Dispose() => DisposeCount++;
    }
}
