using System.Text.Json;
using System.Text.Json.Nodes;
using VisualPlayer.Attributes;
using VisualPlayer.Interfaces;
using VisualPlayer.ViewModels;
using VisualPlayer.VisualizationBase.Plugins;
using VisualPlayer.Visualizations.Registry;
using VisualPlayer.Visualizations.Utils;

namespace VisualPlayer.Visualizations.Config.Base;

/// <summary>Runtime persistence model; plugin editors receive the independent SDK context.</summary>
public class VisualisationProfile : ViewModelBase
{
    private string _pluginId = VisualizationRegistry.NoneId;
    private string _name = "Profile";
    private bool _isDefault, _fillScreen;
    private IVisualizationConfig? _config;
    private VisualizationColorConfig? _color;
    private VisualizationRegistry? _registry;
    private VisualizationRegistration? _registration;
    private string? _configurationError;
    internal JsonObject Source { get; set; } = new();
    public string ProfileId { get; internal set; } = Guid.NewGuid().ToString("N");
    public string? PresetId { get; internal set; }
    public int ConfigVersion { get; internal set; } = 1;

    [NotifyPropertyUpdate]
    public string PluginId
    {
        get => _pluginId;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            if (_pluginId == value) return;
            // Changing the type is an explicit user action, unlike temporary plugin absence.
            _pluginId = value;
            if (_registry is not null) _config = null;
            _registration = null;
            _configurationError = null;
            Source.Remove("config");
            Source.Remove("type");
            ConfigVersion = _registry?.Find(value)?.Descriptor.ConfigVersion ?? 1;
            if (_registry is not null) Resolve(_registry, createDefault: true);
            NotifyPropertyChanged(nameof(PluginId));
        }
    }

    [NotifyPropertyUpdate]
    public string Name
    {
        get => _name;
        set { UpdateProperty(ref _name, value ?? string.Empty); RefreshDisplayName(); }
    }
    [NotifyPropertyUpdate]
    public bool IsDefault
    {
        get => _isDefault;
        set { UpdateProperty(ref _isDefault, value); NotifyPropertyChanged(nameof(CanChangeVisualizationType)); NotifyPropertyChanged(nameof(CanDelete)); }
    }
    [NotifyPropertyUpdate]
    public bool FillScreen { get => _fillScreen; set => UpdateProperty(ref _fillScreen, value); }
    [NotifyPropertyUpdate]
    public IVisualizationConfig? Config { get => _config; set => UpdateProperty(ref _config, value); }
    [NotifyPropertyUpdate]
    public VisualizationColorConfig? Color { get => _color; set => UpdateProperty(ref _color, value); }
    public bool IsAvailable => _registration is not null && _configurationError is null;
    public string? ConfigurationError => _configurationError;
    /// <summary>Available choice for selectors; clearing selection never discards the persisted plugin ID.</summary>
    public VisualizationRegistration? Registration
    {
        get => _registration;
        set { if (value is not null) PluginId = value.Id; }
    }
    public bool CanChangeVisualizationType => !IsDefault;
    public bool CanDelete => !IsDefault;
    public bool SupportsFloatingBarGravity => _registration?.Descriptor.Capabilities.HasFlag(VisualizationCapabilities.FloatingBarGravity) == true;
    public string DisplayName => IsDefault && _registration?.Presets.FirstOrDefault(p => p.Id == PresetId) is { } preset
        ? _registration.Services.Translations[preset.NameKey] : Name;

    public void Resolve(VisualizationRegistry registry, bool createDefault = false)
    {
        var next = registry.Find(PluginId);
        if (!createDefault && ReferenceEquals(_registry, registry) && ReferenceEquals(next, _registration) && IsAvailable) return;
        if (_registration is not null && !ReferenceEquals(next, _registration) && IsAvailable)
        {
            try { Source = ToJson(); ConfigVersion = _registration.Descriptor.ConfigVersion; }
            catch (Exception exception) { _registration.Report("Cannot snapshot configuration; retaining last saved JSON.", exception); }
        }
        if (!ReferenceEquals(next, _registration)) _config = null;
        _registry = registry;
        _registration = next;
        _configurationError = next is null ? $"Plugin unavailable: {PluginId}" : null;
        try
        {
            if (next is not null && PluginId == VisualizationRegistry.NoneId && ConfigVersion != next.Descriptor.ConfigVersion)
                throw new NotSupportedException($"Unsupported configuration version: {ConfigVersion}");
            if (next is not null && PluginId != VisualizationRegistry.NoneId)
            {
                _config ??= createDefault ? next.Plugin.CreateDefaultConfiguration()
                    : next.Plugin.ReadConfiguration(JsonSerializer.SerializeToElement(Source["config"]), ConfigVersion);
                if (_config is null) throw new JsonException("Plugin returned null configuration.");
                _color ??= Source["color"]?.Deserialize<VisualizationColorConfig>(VisualizationProfileSerializer.ModelOptions) ?? new();
                ConfigVersion = next.Descriptor.ConfigVersion;
            }
        }
        catch (Exception exception)
        {
            _config = null;
            _configurationError = exception.Message;
            next?.Report("Cannot read configuration; original JSON retained.", exception);
        }
        NotifyPropertyChanged(nameof(Config));
        NotifyPropertyChanged(nameof(Color));
        NotifyPropertyChanged(nameof(IsAvailable));
        NotifyPropertyChanged(nameof(ConfigurationError));
        NotifyPropertyChanged(nameof(Registration));
        NotifyPropertyChanged(nameof(SupportsFloatingBarGravity));
        RefreshDisplayName();
    }
    public void RefreshDisplayName() => NotifyPropertyChanged(nameof(DisplayName));
    public T GetConfig<T>() where T : class, IVisualizationConfig, new()
        => Config as T ?? throw new InvalidOperationException("The profile does not contain the requested configuration.");
    public VisualizationColorConfig GetColorConfig() => Color ??= new();
    public VisualisationProfile Copy(string name)
    {
        if (_registry is null) throw new InvalidOperationException("Profile is not registered.");
        var copy = VisualizationProfileSerializer.ReadProfile(ToJson(), _registry);
        copy.ProfileId = Guid.NewGuid().ToString("N");
        copy.PresetId = null;
        copy.IsDefault = false;
        copy.Name = name;
        return copy;
    }
    public void OverwriteFrom(VisualisationProfile source)
    {
        if (_registry is null) throw new InvalidOperationException("Profile is not registered.");
        var copy = VisualizationProfileSerializer.ReadProfile(source.ToJson(), _registry);
        Source = copy.Source;
        _pluginId = copy.PluginId;
        _config = copy.Config;
        _color = copy.Color;
        _registration = copy.Registration;
        _configurationError = copy.ConfigurationError;
        ConfigVersion = copy.ConfigVersion;
        FillScreen = copy.FillScreen;
        foreach (var property in new[] { nameof(PluginId), nameof(Config), nameof(Color), nameof(IsAvailable), nameof(ConfigurationError) }) NotifyPropertyChanged(property);
        RefreshDisplayName();
    }
    public JsonObject ToJson()
    {
        var json = (JsonObject)Source.DeepClone();
        json["profileId"] = ProfileId;
        json["name"] = Name;
        json["pluginId"] = PluginId;
        json["isDefault"] = IsDefault;
        json["fillScreen"] = FillScreen;
        if (PresetId is not null) json["presetId"] = PresetId; else json.Remove("presetId");
        if (IsAvailable)
        {
            json.Remove("type");
            if (PluginId == VisualizationRegistry.NoneId) { json["config"] = null; json["color"] = null; }
            else if (Config is not null)
            {
                var serialized = _registration!.Plugin.WriteConfiguration(Config);
                json["config"] = VisualizationProfileSerializer.Merge(json["config"], JsonNode.Parse(serialized.GetRawText()));
                json["color"] = VisualizationProfileSerializer.Merge(json["color"], JsonSerializer.SerializeToNode(Color, VisualizationProfileSerializer.ModelOptions));
            }
            json["configVersion"] = _registration!.Descriptor.ConfigVersion;
        }
        else json["configVersion"] = ConfigVersion;
        return json;
    }
}
