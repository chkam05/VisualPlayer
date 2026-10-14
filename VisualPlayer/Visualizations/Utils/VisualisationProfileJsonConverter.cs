using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using VisualPlayer.Enums;
using VisualPlayer.Interfaces;
using VisualPlayer.Resources;
using VisualPlayer.Visualizations.Config;
using VisualPlayer.Visualizations.Config.Base;

namespace VisualPlayer.Visualizations.Utils;

public class VisualisationProfileJsonConverter : JsonConverter<VisualisationProfile>
{
    public override VisualisationProfile Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var root = document.RootElement;

        var name = root.TryGetProperty("name", out var nameElement)
            ? nameElement.GetString() ?? "Profil wizualizacji"
            : "Profil wizualizacji";

        var defaultProfile = DefaultVisualizationProfiles.Create(name);
        var visualizationType = DeserializeVisualizationType(root, options)
            ?? defaultProfile?.VisualizationType
            ?? VisualizationType.Peaks;

        var config = DeserializeConfig(root, visualizationType, options)
            ?? defaultProfile?.Config;
        var color = DeserializeColor(root, options)
            ?? defaultProfile?.Color;

        if (visualizationType != VisualizationType.None)
            color ??= new VisualizationColorConfig();

        var profile = new VisualisationProfile
        {
            IsDefault = defaultProfile is not null
                || root.TryGetProperty("isDefault", out var isDefaultElement)
                && isDefaultElement.ValueKind is JsonValueKind.True,
            Name = name,
            VisualizationType = visualizationType,
            Color = color,
            Config = config
        };

        return profile;
    }

    public override void Write(Utf8JsonWriter writer, VisualisationProfile value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteBoolean("isDefault", value.IsDefault);
        writer.WriteString("name", value.Name);

        writer.WritePropertyName("type");
        JsonSerializer.Serialize(writer, value.VisualizationType, options);

        writer.WritePropertyName("color");
        JsonSerializer.Serialize(writer, value.Color, options);

        var config = GetConfig(value);

        writer.WritePropertyName("config");
        if (config is null)
            writer.WriteNullValue();
        else
            JsonSerializer.Serialize(writer, config, config.GetType(), options);

        writer.WriteEndObject();
    }


    private static VisualizationColorConfig? DeserializeColor(JsonElement root, JsonSerializerOptions options)
    {
        if (!root.TryGetProperty("color", out var colorElement)
            || colorElement.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        try
        {
            return colorElement.Deserialize<VisualizationColorConfig>(options);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static IVisualizationConfig? DeserializeConfig(JsonElement root, VisualizationType visualizationType, JsonSerializerOptions options)
    {
        if (!root.TryGetProperty("config", out var configElement)
            || configElement.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        try
        {
            return visualizationType switch
            {
                VisualizationType.Peaks => configElement.Deserialize<PeaksVisualizationConfig>(options),
                VisualizationType.Stripes => configElement.Deserialize<StripesVisualizationConfig>(options),
                VisualizationType.PeaksMirrored => configElement.Deserialize<PeaksMirroredVisualizationConfig>(options),
                VisualizationType.StripesMirrored => configElement.Deserialize<StripesMirroredVisualizationConfig>(options),
                VisualizationType.PeaksFloating => configElement.Deserialize<PeaksFloatingVisualizationConfig>(options),
                VisualizationType.PeaksFloatingMirrored => configElement.Deserialize<PeaksFloatingMirroredVisualizationConfig>(options),
                VisualizationType.Oscilloscope => configElement.Deserialize<OscilloscopeVisualizationConfig>(options),
                VisualizationType.OscilloscopeDouble => configElement.Deserialize<OscilloscopeDoubleVisualizationConfig>(options),
                VisualizationType.OscilloscopeFilled => configElement.Deserialize<OscilloscopeFilledVisualizationConfig>(options),
                VisualizationType.OscilloscopeCenter => configElement.Deserialize<OscilloscopeCenterVisualizationConfig>(options),
                VisualizationType.Waveform => configElement.Deserialize<WaveformVisualizationConfig>(options),
                VisualizationType.WaveformMirrored => configElement.Deserialize<WaveformMirroredVisualizationConfig>(options),
                VisualizationType.CircleStripes => configElement.Deserialize<CircleStripesVisualizationConfig>(options),
                VisualizationType.CircleWaveform => configElement.Deserialize<CircleWaveformVisualizationConfig>(options),
                _ => null
            };
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static VisualizationType? DeserializeVisualizationType(JsonElement root, JsonSerializerOptions options)
    {
        if (!root.TryGetProperty("type", out var typeElement))
            return null;

        try
        {
            return typeElement.Deserialize<VisualizationType>(options);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static IVisualizationConfig? GetConfig(VisualisationProfile profile)
    {
        return profile.VisualizationType switch
        {
            VisualizationType.Peaks => profile.GetConfig<PeaksVisualizationConfig>(),
            VisualizationType.Stripes => profile.GetConfig<StripesVisualizationConfig>(),
            VisualizationType.PeaksMirrored => profile.GetConfig<PeaksMirroredVisualizationConfig>(),
            VisualizationType.StripesMirrored => profile.GetConfig<StripesMirroredVisualizationConfig>(),
            VisualizationType.PeaksFloating => profile.GetConfig<PeaksFloatingVisualizationConfig>(),
            VisualizationType.PeaksFloatingMirrored => profile.GetConfig<PeaksFloatingMirroredVisualizationConfig>(),
            VisualizationType.Oscilloscope => profile.GetConfig<OscilloscopeVisualizationConfig>(),
            VisualizationType.OscilloscopeDouble => profile.GetConfig<OscilloscopeDoubleVisualizationConfig>(),
            VisualizationType.OscilloscopeFilled => profile.GetConfig<OscilloscopeFilledVisualizationConfig>(),
            VisualizationType.OscilloscopeCenter => profile.GetConfig<OscilloscopeCenterVisualizationConfig>(),
            VisualizationType.Waveform => profile.GetConfig<WaveformVisualizationConfig>(),
            VisualizationType.WaveformMirrored => profile.GetConfig<WaveformMirroredVisualizationConfig>(),
            VisualizationType.CircleStripes => profile.GetConfig<CircleStripesVisualizationConfig>(),
            VisualizationType.CircleWaveform => profile.GetConfig<CircleWaveformVisualizationConfig>(),
            _ => null
        };
    }
}
