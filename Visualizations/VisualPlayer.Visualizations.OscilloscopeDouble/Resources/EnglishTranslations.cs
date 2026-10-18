using System.Collections.ObjectModel;

namespace VisualPlayer.Visualizations.OscilloscopeDouble.Resources;

public static class EnglishTranslations
{
    public static IReadOnlyDictionary<string, string> Values { get; } = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
    {
        ["ENUM_VISUALIZATIONTYPE_OSCILLOSCOPEDOUBLE"] = "Oscilloscope - Double",
        ["VIEW_VISUALIZATIONS_LINE_EFFECT_TITLE"] = "Line effect",
        ["VIEW_VISUALIZATIONS_LINE_THICKNESS_TITLE"] = "Line thickness",
        ["VIEW_VISUALIZATIONS_OSCILLOSCOPE_BEAM_EFFECT_DESC"] = "It adds a soft glow around the lines.",
        ["VIEW_VISUALIZATIONS_OSCILLOSCOPE_DOUBLE_LINE_THICKNESS_DESC"] = "Specifies the line thickness for the dual-trace oscilloscope display.",
        ["VIEW_VISUALIZATIONS_OSCILLOSCOPE_SMOOTH_LINE_DESC"] = "It interpolates samples to reduce sharp breaks in the lines.",
        ["VIEW_VISUALIZATIONS_OSCILLOSCOPE_SMOOTH_LINE_TITLE"] = "Smoothing out lines",
        ["PRESET_DEFAULT"] = "Oscilloscope - Double",
    });
}
