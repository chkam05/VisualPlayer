using System.ComponentModel;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using VisualPlayer.Interfaces;
using VisualPlayer.VisualizationBase.Plugins;
using VisualPlayer.Visualizations.Config.Base;
using VisualPlayer.Visualizations.Data;

namespace VisualPlayer.SamplePlugin;

// Deliberately uses only the SDK, without the application, runtime or UI library.
public sealed class SamplePlugin : IVisualizationPlugin
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public VisualizationDescriptor Descriptor { get; } = new()
    {
        Id = "tests.sample",
        NameKey = "NAME",
        PluginVersion = new Version(1, 0, 0),
        ConfigVersion = 1,
        Capabilities = VisualizationCapabilities.Spectrum | VisualizationCapabilities.Colors
    };

    public IReadOnlyDictionary<string, string> EnglishTranslations { get; }
        = new System.Collections.ObjectModel.ReadOnlyDictionary<string, string>(
            new Dictionary<string, string> { ["NAME"] = "Sample" });

    public IReadOnlyList<VisualizationPreset> DefaultPresets => new[]
    {
        new VisualizationPreset
        {
            Id = "default", NameKey = "NAME", ConfigVersion = 1,
            Configuration = WriteConfiguration(CreateDefaultConfiguration()),
            Color = new VisualizationColorConfig()
        }
    };

    public IVisualizationConfig CreateDefaultConfiguration() => new SampleConfig();

    public IVisualizationConfig ReadConfiguration(JsonElement configuration, int configVersion)
    {
        if (configVersion != Descriptor.ConfigVersion)
            throw new NotSupportedException($"Unsupported configuration version: {configVersion}");
        return configuration.Deserialize<SampleConfig>(JsonOptions) ?? throw new JsonException("Null configuration.");
    }

    public JsonElement WriteConfiguration(IVisualizationConfig configuration)
        => JsonSerializer.SerializeToElement(RequireConfig(configuration), JsonOptions);

    public IVisualization CreateVisualization(IVisualizationConfig configuration,
        VisualizationColorConfig color, IVisualizationHostServices services)
        => new SampleVisualization(RequireConfig(configuration));

    public Control? CreateEditor(VisualizationEditorContext context)
        => new TextBlock { Text = context.Services.Translations["NAME"], DataContext = RequireConfig(context.Configuration) };

    private static SampleConfig RequireConfig(IVisualizationConfig configuration)
        => configuration as SampleConfig ?? throw new ArgumentException("Wrong plugin configuration.", nameof(configuration));
}

// No dependency on the host's ViewModelBase is required by the new generic base.
public sealed class SampleConfig : IVisualizationConfig
{
    private int _height = 20;
    public event PropertyChangedEventHandler? PropertyChanged;
    public int Height
    {
        get => _height;
        set
        {
            var height = Math.Clamp(value, 1, 256);
            if (_height == height) return;
            _height = height;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Height)));
        }
    }
}

public sealed class SampleVisualization : VisualPlayer.Visualizations.VisualizationBase<SampleConfig>
{
    public int ConfigurationChangeCount { get; private set; }
    public int GraphicsChangeCount { get; private set; }
    public int DisposeCount { get; private set; }
    public override bool HasActiveAnimation => false;
    public SampleVisualization(SampleConfig configuration) : base(configuration) { }
    public override void PreCalculate(in VisualizationFrame frame) { }
    public override void Reset() { }
    public override void Draw(DrawingContext context, IBrush brush)
        => context.DrawRectangle(brush, null, new Rect(0, 0, 10, Config.Height));
    protected override void OnConfigChanged(string? propertyName) => ConfigurationChangeCount++;
    protected override void OnGraphicsChanged() => GraphicsChangeCount++;
    protected override void DisposeCore() => DisposeCount++;
}
