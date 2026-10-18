using System.ComponentModel;
using System.Text.Json;
using Avalonia;
using VisualPlayer.Attributes;
using VisualPlayer.Contexts;
using VisualPlayer.Interfaces;
using VisualPlayer.SamplePlugin;
using VisualPlayer.ViewModels;
using VisualPlayer.VisualizationBase.Localization;
using VisualPlayer.VisualizationBase.Plugins;
using VisualPlayer.Visualizations.Config.Base;
using VisualPlayer.Visualizations.Data;

internal static class ContractChecks
{
    public static void Run()
    {
        var sdk = typeof(IVisualization).Assembly;
        Check(sdk.GetName().Name == "VisualPlayer.VisualizationBase", "Visualization contract must belong to SDK.");
        foreach (var type in new[] { typeof(IVisualizationConfig), typeof(VisualizationFrame), typeof(ViewModelBase),
                     typeof(NotifyPropertyUpdateAttribute), typeof(VisualizationColorConfig), typeof(IVisualizationPlugin) })
            Check(type.Assembly == sdk, $"Shared type duplicated outside SDK: {type}");
        Check(typeof(TranslationContext).IsAssignableTo(typeof(ITranslationScope)), "Host translation adapter missing.");
        var references = typeof(SamplePlugin).Assembly.GetReferencedAssemblies().Select(a => a.Name).ToArray();
        Check(!references.Contains("VisualPlayer") && !references.Contains("VisualPlayer.UI")
            && !references.Contains("VisualPlayer.Visualizations"), "Sample plugin depends on host/runtime/UI.");
        Check(!sdk.GetReferencedAssemblies().Any(a => a.Name is "VisualPlayer" or "VisualPlayer.UI" or "VisualPlayer.Visualizations"),
            "SDK has a reversed dependency.");

        IVisualizationPlugin plugin = new SamplePlugin();
        var first = (SampleConfig)plugin.CreateDefaultConfiguration();
        var second = (SampleConfig)plugin.CreateDefaultConfiguration();
        first.Height = 80;
        Check(second.Height == 20, "Default configurations share mutable state.");
        JsonElement ownedJson;
        using (var document = JsonDocument.Parse("{\"height\":91}"))
        {
            first = (SampleConfig)plugin.ReadConfiguration(document.RootElement, 1);
            ownedJson = plugin.WriteConfiguration(first);
        }
        Check(ownedJson.GetProperty("height").GetInt32() == 91, "Serialized JSON does not own its memory.");
        Check(((SampleConfig)plugin.ReadConfiguration(ownedJson, 1)).Height == 91, "Plugin config round trip failed.");
        ExpectFailure<NotSupportedException>(() => plugin.ReadConfiguration(ownedJson, 99));
        using (var badJson = JsonDocument.Parse("{\"height\":\"invalid\"}"))
            ExpectFailure<JsonException>(() => plugin.ReadConfiguration(badJson.RootElement, 1));

        var services = new TestServices();
        var color = new VisualizationColorConfig();
        var context = new VisualizationEditorContext(first, color, services);
        Check(ReferenceEquals(context.Configuration, first) && ReferenceEquals(context.Color, color), "Editor lost live settings.");
        var editor = plugin.CreateEditor(context)!;
        Check(ReferenceEquals(editor.DataContext, first), "Editor factory replaced configuration.");
        using var visualization = (SampleVisualization)plugin.CreateVisualization(first, color, services);
        first.Height++;
        Check(visualization.ConfigurationChangeCount == 1, "Config event was not forwarded.");
        visualization.UpdateGraphics(new Rect(0, 0, 100, 100), 0);
        visualization.UpdateGraphics(new Rect(0, 0, 100, 100), double.NaN);
        Check(visualization.GraphicsChangeCount == 1, "Render scaling normalization changed.");
        visualization.Dispose();
        visualization.Dispose();
        first.Height++;
        Check(visualization.DisposeCount == 1 && visualization.ConfigurationChangeCount == 1, "Dispose leaks subscriptions or runs twice.");

        var preset = plugin.DefaultPresets.Single();
        var presetConfig = (SampleConfig)plugin.ReadConfiguration(preset.Configuration, preset.ConfigVersion);
        presetConfig.Height = 100;
        Check(((SampleConfig)plugin.ReadConfiguration(plugin.DefaultPresets.Single().Configuration, 1)).Height == 20,
            "Preset configuration shares edited state.");
        Check(services.Translations["NAME"] == "Local" && services.CommonTranslations["NAME"] == "Common", "Translation scopes collide.");
        var model = new ObservableProbe();
        var notified = 0;
        model.PropertyChanged += (_, _) => notified++;
        model.Value = 2;
        model.Value = 2;
        Check(notified == 1, "Moved MVVM attribute identity/notifications changed.");
        Console.WriteLine("PASS: SDK-only plugin factories, config JSON, editor context, type identity, MVVM and disposal.");
    }

    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    private static void ExpectFailure<TException>(Action action) where TException : Exception
    {
        try { action(); }
        catch (TException) { return; }
        throw new InvalidOperationException($"Expected {typeof(TException).Name}.");
    }

    private sealed class ObservableProbe : ViewModelBase
    {
        private int _value;
        [NotifyPropertyUpdate]
        public int Value { get => _value; set => UpdateProperty(ref _value, value); }
    }

    private sealed class TestServices : IVisualizationHostServices
    {
        public ITranslationScope Translations { get; } = new TestTranslations("Local");
        public ITranslationScope CommonTranslations { get; } = new TestTranslations("Common");
        public void ReportDiagnostic(VisualizationDiagnosticLevel level, string message, Exception? exception = null) { }
    }

    private sealed class TestTranslations(string value) : ITranslationScope
    {
        public event EventHandler? LanguageChanged { add { } remove { } }
        public event PropertyChangedEventHandler? PropertyChanged { add { } remove { } }
        public string CurrentLanguageCode => "en-US";
        public string this[string key] => key == "NAME" ? value : key;
        public string Format(string key, params object?[] arguments) => string.Format(this[key], arguments);
    }
}
