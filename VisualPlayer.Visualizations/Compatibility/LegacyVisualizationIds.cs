using System.Text.Json.Nodes;
using VisualPlayer.Enums;

namespace VisualPlayer.Visualizations.Compatibility;

/// <summary>The historical enum is a migration detail; new plugins never extend it.</summary>
public static class LegacyVisualizationIds
{
    private static readonly (VisualizationType Type, string Id, string Name)[] Entries =
    [
        (VisualizationType.None, "visualplayer.none", "None"),
        (VisualizationType.CircleStripes, "visualplayer.circle-stripes", "Circle - Stripes"),
        (VisualizationType.CircleWaveform, "visualplayer.circle-waveform", "Circle - Waveform"),
        (VisualizationType.Oscilloscope, "visualplayer.oscilloscope", "Oscilloscope"),
        (VisualizationType.OscilloscopeCenter, "visualplayer.oscilloscope-center", "Oscilloscope - Center"),
        (VisualizationType.OscilloscopeDouble, "visualplayer.oscilloscope-double", "Oscilloscope - Double"),
        (VisualizationType.OscilloscopeFilled, "visualplayer.oscilloscope-filled", "Oscilloscope - Filled"),
        (VisualizationType.Peaks, "visualplayer.peaks", "Peaks"),
        (VisualizationType.PeaksFloating, "visualplayer.peaks-floating", "Peaks - Floating"),
        (VisualizationType.PeaksFloatingMirrored, "visualplayer.peaks-floating-mirrored", "Peaks - Floating Mirrored"),
        (VisualizationType.PeaksMirrored, "visualplayer.peaks-mirrored", "Peaks - Mirrored"),
        (VisualizationType.Stripes, "visualplayer.stripes", "Stripes"),
        (VisualizationType.StripesMirrored, "visualplayer.stripes-mirrored", "Stripes - Mirrored"),
        (VisualizationType.Waveform, "visualplayer.waveform", "Waveform"),
        (VisualizationType.WaveformMirrored, "visualplayer.waveform-mirrored", "Waveform - Mirrored"),
        (VisualizationType.PeaksBouncing, "visualplayer.peaks-bouncing", "Peaks - Bouncing"),
        (VisualizationType.StripesBouncing, "visualplayer.stripes-bouncing", "Stripes - Bouncing")
    ];
    public static string GetId(VisualizationType type)
        => Entries.FirstOrDefault(entry => entry.Type == type).Id ?? $"legacy.unknown.{(int)type}";
    public static VisualizationType? GetType(string id)
        => Entries.Where(entry => entry.Id == id).Select(entry => (VisualizationType?)entry.Type).FirstOrDefault();
    public static string? FromLegacy(JsonNode? value)
    {
        if (value is not JsonValue scalar) return null;
        if (scalar.TryGetValue<int>(out var number)) return Entries.FirstOrDefault(entry => (int)entry.Type == number).Id;
        if (scalar.TryGetValue<string>(out var name) && Enum.TryParse<VisualizationType>(name, true, out var type) && Enum.IsDefined(type)) return GetId(type);
        return null;
    }
    public static string? FromDefaultName(string? name)
        => Entries.FirstOrDefault(entry => string.Equals(entry.Name, name, StringComparison.OrdinalIgnoreCase)).Id;
}
