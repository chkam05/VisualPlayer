using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using VisualPlayer.Visualizations.Compatibility;
using VisualPlayer.Visualizations.Config.Base;
using VisualPlayer.Visualizations.Registry;

namespace VisualPlayer.Visualizations.Utils;

public sealed class VisualizationProfileSerializer(VisualizationRegistry registry)
{
    public static JsonSerializerOptions ModelOptions { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase, PropertyNameCaseInsensitive = true, WriteIndented = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };
    public VisualisationProfile Deserialize(string json)
        => ReadProfile(JsonNode.Parse(json) as JsonObject ?? throw new JsonException("Expected a profile object."), registry);
    public string Serialize(VisualisationProfile profile) => profile.ToJson().ToJsonString(ModelOptions);
    public static VisualisationProfile ReadProfile(JsonObject json, VisualizationRegistry registry)
    {
        var name = json["name"]?.GetValue<string>() ?? "Profile";
        var legacyId = json.ContainsKey("type") ? LegacyVisualizationIds.FromLegacy(json["type"]) : LegacyVisualizationIds.FromDefaultName(name);
        var id = json["pluginId"]?.GetValue<string>() ?? legacyId ?? "legacy.unknown";
        var isDefault = json["isDefault"]?.GetValue<bool>() ?? false;
        var preset = json["presetId"]?.GetValue<string>() ?? (isDefault && legacyId is not null ? "default" : null);
        var profile = new VisualisationProfile
        {
            PluginId = id, Name = name, IsDefault = isDefault, FillScreen = json["fillScreen"]?.GetValue<bool>() ?? false,
            ConfigVersion = json["configVersion"]?.GetValue<int>() ?? 1, PresetId = preset,
            ProfileId = json["profileId"]?.GetValue<string>() ?? (isDefault && preset is not null ? $"{id}:{preset}" : Guid.NewGuid().ToString("N"))
        };
        profile.Source = (JsonObject)json.DeepClone();
        profile.Resolve(registry);
        return profile;
    }
    public static JsonNode? Merge(JsonNode? original, JsonNode? current)
    {
        if (original is JsonObject oldObject && current is JsonObject newObject)
        {
            var result = (JsonObject)oldObject.DeepClone();
            foreach (var pair in newObject) result[pair.Key] = Merge(oldObject[pair.Key], pair.Value);
            return result;
        }
        if (original is JsonArray oldArray && current is JsonArray newArray && oldArray.Count == newArray.Count)
            return new JsonArray(newArray.Select((node, index) => Merge(oldArray[index], node)).ToArray());
        return current?.DeepClone();
    }
}
