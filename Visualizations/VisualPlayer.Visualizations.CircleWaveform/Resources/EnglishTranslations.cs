using System.Collections.ObjectModel;

namespace VisualPlayer.Visualizations.CircleWaveform.Resources;

public static class EnglishTranslations
{
    public static IReadOnlyDictionary<string, string> Values { get; } = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
    {
        ["COMMON_OPACITY_TITLE"] = "Transparency",
        ["ENUM_VISUALIZATIONTYPE_CIRCLEWAVEFORM"] = "Circle - Waveform",
        ["VIEW_VISUALIZATIONS_CIRCLE_STRIPES_CENTER_OFFSET_X_DESC"] = "Shifts the center of the circle horizontally relative to the center of the visualization area.",
        ["VIEW_VISUALIZATIONS_CIRCLE_STRIPES_CENTER_OFFSET_X_TITLE"] = "X-shift",
        ["VIEW_VISUALIZATIONS_CIRCLE_STRIPES_CENTER_OFFSET_Y_DESC"] = "Shifts the center of the circle vertically relative to the center of the visualization area.",
        ["VIEW_VISUALIZATIONS_CIRCLE_STRIPES_CENTER_OFFSET_Y_TITLE"] = "Y-shift",
        ["VIEW_VISUALIZATIONS_CIRCLE_STRIPES_INNER_RADIUS_TITLE"] = "Inner radius",
        ["VIEW_VISUALIZATIONS_CIRCLE_STRIPES_MIRROR_DESC"] = "It mirrors the sequence of FFT bands onto the second half of the circle.",
        ["VIEW_VISUALIZATIONS_CIRCLE_STRIPES_MIRROR_TITLE"] = "Mirror",
        ["VIEW_VISUALIZATIONS_CIRCLE_STRIPES_ROTATION_ANIMATION_DESC"] = "Controls the continuous rotation of the visualization.",
        ["VIEW_VISUALIZATIONS_CIRCLE_STRIPES_ROTATION_ANIMATION_TITLE"] = "Rotation animation",
        ["VIEW_VISUALIZATIONS_CIRCLE_STRIPES_ROTATION_DESC"] = "Specifies the initial visualization angle. The setting is locked during the rotation animation.",
        ["VIEW_VISUALIZATIONS_CIRCLE_STRIPES_ROTATION_DIRECTION_DESC"] = "Specifies the direction of the animated rotation.",
        ["VIEW_VISUALIZATIONS_CIRCLE_STRIPES_ROTATION_DIRECTION_TITLE"] = "Direction of rotation",
        ["VIEW_VISUALIZATIONS_CIRCLE_STRIPES_ROTATION_ENABLED_DESC"] = "Enables continuous rotation of the visualization.",
        ["VIEW_VISUALIZATIONS_CIRCLE_STRIPES_ROTATION_SPEED_DESC"] = "Specifies the number of degrees of rotation per second.",
        ["VIEW_VISUALIZATIONS_CIRCLE_STRIPES_ROTATION_SPEED_TITLE"] = "Rotation speed",
        ["VIEW_VISUALIZATIONS_CIRCLE_STRIPES_ROTATION_TITLE"] = "Rotation angle",
        ["VIEW_VISUALIZATIONS_CIRCLE_WAVEFORM_BEAM_EFFECT_DESC"] = "It adds a subtle glow around the outline of the wave.",
        ["VIEW_VISUALIZATIONS_CIRCLE_WAVEFORM_DESCENT_RATE_DESC"] = "It determines the rate at which the wave height decreases.",
        ["VIEW_VISUALIZATIONS_CIRCLE_WAVEFORM_FILL_ENABLED_DESC"] = "It fills the entire area inside the closed wave.",
        ["VIEW_VISUALIZATIONS_CIRCLE_WAVEFORM_INNER_RADIUS_DESC"] = "Specifies the radius of the circle at which the wave begins or ends.",
        ["VIEW_VISUALIZATIONS_CIRCLE_WAVEFORM_LINE_THICKNESS_DESC"] = "Specifies the thickness of the closed wave's outline.",
        ["VIEW_VISUALIZATIONS_CIRCLE_WAVEFORM_NUM_DESC"] = "Specifies the number of FFT points forming a closed wave distributed along a circle.",
        ["VIEW_VISUALIZATIONS_CIRCLE_WAVEFORM_ROTATION_ENABLED_TITLE"] = "Enable animation",
        ["VIEW_VISUALIZATIONS_DESCENT_RATE_TITLE"] = "Descent rate",
        ["VIEW_VISUALIZATIONS_LINE_EFFECT_TITLE"] = "Line effect",
        ["VIEW_VISUALIZATIONS_LINE_THICKNESS_TITLE"] = "Line thickness",
        ["VIEW_VISUALIZATIONS_TRANSFORM_TITLE"] = "Transformation",
        ["VIEW_VISUALIZATIONS_WAVEFORM_FILL_ENABLED_TITLE"] = "Fill",
        ["VIEW_VISUALIZATIONS_WAVEFORM_NUM_TITLE"] = "Number of peaks",
        ["VIEW_VISUALIZATIONS_WAVEFORM_OPACITY_DESC"] = "Specifies the transparency of the filled area.",
        ["VIEW_VISUALIZATIONS_WAVEFORM_TRANSFORM_DESC"] = "Visualization transformation.",
        ["PRESET_DEFAULT"] = "Circle - Waveform",
    });
}
