using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using VisualPlayer.Attributes;
using VisualPlayer.ViewModels;
using VisualPlayer.Visualizations.Config.Base;
using VisualPlayer.Visualizations.Registry;
using VisualPlayer.Visualizations.Utils;

namespace VisualPlayer.Contexts;

/// <summary>Registry/path-injected runtime settings, independent of application directories.</summary>
public sealed class VisualizationConfigurationContext : ViewModelBase, IDisposable
{
    public const int FileFormatVersion = 2;
    private VisualizationGlobalConfig _global = new();
    private string _selectedProfileId = string.Empty;
    private JsonObject _source = new();
    private bool _requiresMigration;
    private string? _filePath;
    public VisualizationRegistry Registry { get; }
    public ObservableCollection<VisualisationProfile> Profiles { get; } = [];
    [NotifyPropertyUpdate]
    public VisualizationGlobalConfig Global { get => _global; set => UpdateProperty(ref _global, value ?? new()); }
    [NotifyPropertyUpdate]
    public string SelectedProfileId
    {
        get => _selectedProfileId;
        set
        {
            if (_selectedProfileId == value) return;
            UpdateProperty(ref _selectedProfileId, value);
            NotifyPropertyChanged(nameof(SelectedProfileName));
            NotifyPropertyChanged(nameof(SelectedProfile));
        }
    }
    public string SelectedProfileName
    {
        get => SelectedProfile.Name;
        set { if (Profiles.FirstOrDefault(p => p.Name.Equals(value, StringComparison.OrdinalIgnoreCase)) is { } profile) SelectedProfileId = profile.ProfileId; }
    }
    public VisualisationProfile SelectedProfile => Profiles.FirstOrDefault(p => p.ProfileId == SelectedProfileId) ?? Profiles.First();
    public VisualizationConfigurationContext(VisualizationRegistry registry)
    {
        Registry = registry;
        Registry.Changed += RegistryChanged;
        Registry.LanguageChanged += LanguageChanged;
        EnsureProfiles();
    }
    public void EnsureProfiles()
    {
        foreach (var profile in Profiles) profile.Resolve(Registry);
        foreach (var entry in Registry.Available)
        foreach (var preset in entry.Presets)
        {
            if (Profiles.Any(p => p.IsDefault && p.PluginId == entry.Id && p.PresetId == preset.Id)) continue;
            var json = new JsonObject
            {
                ["profileId"] = $"{entry.Id}:{preset.Id}", ["pluginId"] = entry.Id, ["presetId"] = preset.Id,
                ["name"] = entry.Plugin.EnglishTranslations.GetValueOrDefault(preset.NameKey) ?? entry.Name,
                ["isDefault"] = true, ["configVersion"] = preset.ConfigVersion, ["fillScreen"] = preset.FillScreen,
                ["config"] = JsonNode.Parse(preset.Configuration.GetRawText()),
                ["color"] = JsonSerializer.SerializeToNode(preset.Color, VisualizationProfileSerializer.ModelOptions)
            };
            Profiles.Add(VisualizationProfileSerializer.ReadProfile(json, Registry));
        }
        if (!Profiles.Any(p => p.ProfileId == SelectedProfileId)) SelectedProfileId = Profiles.First(p => p.PluginId == VisualizationRegistry.NoneId).ProfileId;
    }
    private void RegistryChanged(object? sender, EventArgs e)
    {
        EnsureProfiles();
        NotifyPropertyChanged(nameof(Profiles));
        NotifyPropertyChanged(nameof(SelectedProfile));
    }
    private void LanguageChanged(object? sender, EventArgs e)
    {
        foreach (var profile in Profiles) profile.RefreshDisplayName();
    }
    public static VisualizationConfigurationContext Load(string filePath, VisualizationRegistry registry)
    {
        var context = new VisualizationConfigurationContext(registry) { _filePath = Path.GetFullPath(filePath) };
        try
        {
            if (!File.Exists(filePath)) return context;
            var source = JsonNode.Parse(File.ReadAllText(filePath)) as JsonObject ?? throw new JsonException("Expected visualization settings.");
            if ((source["formatVersion"]?.GetValue<int>() ?? 1) > FileFormatVersion) throw new NotSupportedException("Newer visualization settings format.");
            context._source = (JsonObject)source.DeepClone();
            context._requiresMigration = (source["formatVersion"]?.GetValue<int>() ?? 1) < FileFormatVersion;
            context.Global = source["global"]?.Deserialize<VisualizationGlobalConfig>(VisualizationProfileSerializer.ModelOptions) ?? new();
            context.Profiles.Clear();
            foreach (var node in source["profiles"] as JsonArray ?? [])
            {
                if (node is not JsonObject json) throw new JsonException("Invalid visualization profile.");
                context.Profiles.Add(VisualizationProfileSerializer.ReadProfile(json, registry));
            }
            var selectedId = source["selectedProfileId"]?.GetValue<string>();
            if (selectedId is not null) context._selectedProfileId = selectedId;
            else if (source["selectedProfileName"]?.GetValue<string>() is { } name)
                context._selectedProfileId = context.Profiles.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase))?.ProfileId ?? string.Empty;
            context.EnsureProfiles();
            return context;
        }
        catch { context.Dispose(); throw; }
    }
    public void Save()
    {
        if (_filePath is null) throw new InvalidOperationException("No visualization settings file configured.");
        Save(_filePath);
    }
    public void Save(string filePath)
    {
        // Serialize entirely before touching the last known-good file.
        var source = (JsonObject)_source.DeepClone();
        source["formatVersion"] = FileFormatVersion;
        source["global"] = VisualizationProfileSerializer.Merge(source["global"], JsonSerializer.SerializeToNode(Global, VisualizationProfileSerializer.ModelOptions));
        source["selectedProfileId"] = SelectedProfileId;
        source["selectedProfileName"] = SelectedProfileName;
        source["profiles"] = new JsonArray(Profiles.Select(p => (JsonNode)p.ToJson()).ToArray());
        var text = source.ToJsonString(VisualizationProfileSerializer.ModelOptions);
        var path = Path.GetFullPath(filePath);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        if (_requiresMigration && File.Exists(path) && !File.Exists(path + ".pre-plugins.bak")) File.Copy(path, path + ".pre-plugins.bak");
        var temporary = Path.Combine(Path.GetDirectoryName(path)!, $".{Path.GetFileName(path)}.{Guid.NewGuid():N}.tmp");
        try
        {
            using (var stream = new FileStream(temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                stream.Write(Encoding.UTF8.GetBytes(text));
                stream.Flush(flushToDisk: true);
            }
            File.Move(temporary, path, overwrite: true);
            _source = source;
            _filePath = path;
            _requiresMigration = false;
        }
        finally { if (File.Exists(temporary)) File.Delete(temporary); }
    }
    public void Dispose()
    {
        Registry.Changed -= RegistryChanged;
        Registry.LanguageChanged -= LanguageChanged;
    }
}
