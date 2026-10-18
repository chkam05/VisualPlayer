using System.Collections.ObjectModel;

namespace VisualPlayer.Visualizations.OscilloscopeCenter.Resources;

public static class EnglishTranslations
{
    public static IReadOnlyDictionary<string, string> Values { get; } = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
    {
        ["ENUM_VISUALIZATIONTYPE_OSCILLOSCOPECENTER"] = "Oscilloscope - Center",
        ["VIEW_VISUALIZATIONS_LINE_EFFECT_TITLE"] = "Line effect",
        ["VIEW_VISUALIZATIONS_LINE_THICKNESS_TITLE"] = "Line thickness",
        ["VIEW_VISUALIZATIONS_OSCILLOSCOPE_BEAM_EFFECT_DESC"] = "It adds a soft glow around the lines.",
        ["VIEW_VISUALIZATIONS_OSCILLOSCOPE_CENTER_LINE_THICKNESS_DESC"] = "Specifies the line thickness for the collision oscilloscope.",
        ["VIEW_VISUALIZATIONS_OSCILLOSCOPE_SMOOTH_LINE_DESC"] = "It interpolates samples to reduce sharp breaks in the lines.",
        ["VIEW_VISUALIZATIONS_OSCILLOSCOPE_SMOOTH_LINE_TITLE"] = "Smoothing out lines",
        ["PRESET_DEFAULT"] = "Oscilloscope - Center",
    });
}
