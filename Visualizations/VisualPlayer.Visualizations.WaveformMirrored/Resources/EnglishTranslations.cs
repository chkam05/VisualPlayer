using System.Collections.ObjectModel;

namespace VisualPlayer.Visualizations.WaveformMirrored.Resources;

public static class EnglishTranslations
{
    public static IReadOnlyDictionary<string, string> Values { get; } = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
    {
        ["COMMON_OPACITY_TITLE"] = "Transparency",
        ["ENUM_VISUALIZATIONTYPE_WAVEFORMMIRRORED"] = "Waveform - Mirrored",
        ["VIEW_VISUALIZATIONS_DESCENT_RATE_TITLE"] = "Descent rate",
        ["VIEW_VISUALIZATIONS_DIRECTION_TITLE"] = "Direction",
        ["VIEW_VISUALIZATIONS_LINE_EFFECT_TITLE"] = "Line effect",
        ["VIEW_VISUALIZATIONS_LINE_THICKNESS_TITLE"] = "Line thickness",
        ["VIEW_VISUALIZATIONS_TRANSFORM_TITLE"] = "Transformation",
        ["VIEW_VISUALIZATIONS_WAVEFORM_BEAM_EFFECT_DESC"] = "It adds a subtle glow around the wave line.",
        ["VIEW_VISUALIZATIONS_WAVEFORM_DESCENT_RATE_DESC"] = "Determines the rate at which the wave falls when the frequency level is reduced.",
        ["VIEW_VISUALIZATIONS_WAVEFORM_DIRECTION_DESC"] = "Determines whether the wave grows inwards or outwards.",
        ["VIEW_VISUALIZATIONS_WAVEFORM_FILL_ENABLED_DESC"] = "It fills the area between the wave and its central axis.",
        ["VIEW_VISUALIZATIONS_WAVEFORM_FILL_ENABLED_TITLE"] = "Fill",
        ["VIEW_VISUALIZATIONS_WAVEFORM_LINE_THICKNESS_DESC"] = "Specifies the thickness of the wave edge.",
        ["VIEW_VISUALIZATIONS_WAVEFORM_NUM_DESC"] = "Specifies the number of FFT samples that make up the waveform.",
        ["VIEW_VISUALIZATIONS_WAVEFORM_NUM_TITLE"] = "Number of peaks",
        ["VIEW_VISUALIZATIONS_WAVEFORM_OPACITY_DESC"] = "Specifies the transparency of the filled area.",
        ["VIEW_VISUALIZATIONS_WAVEFORM_SHIFT_DESC"] = "Reverses the order of the lower wave's FFT bands along the horizontal axis.",
        ["VIEW_VISUALIZATIONS_WAVEFORM_SHIFT_TITLE"] = "Shift",
        ["VIEW_VISUALIZATIONS_WAVEFORM_TRANSFORMATION_DESC"] = "It modulates the height of the peaks using a single period of a selected function across the entire width of the visualization.",
        ["VIEW_VISUALIZATIONS_WAVEFORM_TRANSFORMATION_MINIMUM_DESC"] = "Specifies the minimum amplitude of the sine or cosine waveform before multiplication by the FFT level.",
        ["VIEW_VISUALIZATIONS_WAVEFORM_TRANSFORMATION_MINIMUM_TITLE"] = "Minimum transformation",
        ["VIEW_VISUALIZATIONS_WAVEFORM_TRANSFORMATION_TITLE"] = "Type of transformation",
        ["VIEW_VISUALIZATIONS_WAVEFORM_TRANSFORM_DESC"] = "Visualization transformation.",
        ["PRESET_DEFAULT"] = "Waveform - Mirrored",
    });
}
