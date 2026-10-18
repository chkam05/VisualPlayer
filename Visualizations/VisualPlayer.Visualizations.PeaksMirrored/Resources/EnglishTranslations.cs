using System.Collections.ObjectModel;

namespace VisualPlayer.Visualizations.PeaksMirrored.Resources;

public static class EnglishTranslations
{
    public static IReadOnlyDictionary<string, string> Values { get; } = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
    {
        ["ENUM_VISUALIZATIONTYPE_PEAKSMIRRORED"] = "Peaks - Mirrored",
        ["VIEW_VISUALIZATIONS_DESCENT_RATE_TITLE"] = "Descent rate",
        ["VIEW_VISUALIZATIONS_DIRECTION_TITLE"] = "Direction",
        ["VIEW_VISUALIZATIONS_NUM_TITLE"] = "Number of peaks",
        ["VIEW_VISUALIZATIONS_PEAKS_BAR_HEIGHT_DESC"] = "Specifies the height of a single block making up the strip.",
        ["VIEW_VISUALIZATIONS_PEAKS_BAR_HEIGHT_TITLE"] = "Block height",
        ["VIEW_VISUALIZATIONS_PEAKS_DESCENT_RATE_DESC"] = "Specifies the falling speed of the main bars in pixels per second.",
        ["VIEW_VISUALIZATIONS_PEAKS_DIRECTION_DESC"] = "Specifies whether the visualization grows inward or outward.",
        ["VIEW_VISUALIZATIONS_PEAKS_FLOATING_BAR_DESCENT_RATE_DESC"] = "Specifies the falling speed of the additional block in pixels per second.",
        ["VIEW_VISUALIZATIONS_PEAKS_FLOATING_BAR_DESCENT_RATE_TITLE"] = "Speed ​​of the additional block",
        ["VIEW_VISUALIZATIONS_PEAKS_FLOATING_DESC"] = "Settings for the slower-decaying peak value block.",
        ["VIEW_VISUALIZATIONS_PEAKS_HORIZONTAL_BAR_SPACE_DESC"] = "Specifies the spacing between adjacent bands.",
        ["VIEW_VISUALIZATIONS_PEAKS_HORIZONTAL_BAR_SPACE_TITLE"] = "Horizontal block spacing",
        ["VIEW_VISUALIZATIONS_PEAKS_NUM_DESC"] = "Specifies the number of frequency bands displayed on the screen.",
        ["VIEW_VISUALIZATIONS_PEAKS_SHOW_FLOATING_BAR_DESC"] = "It displays a slower-falling peak value indicator above each bar.",
        ["VIEW_VISUALIZATIONS_PEAKS_SHOW_FLOATING_BAR_TITLE"] = "Show the descending block.",
        ["VIEW_VISUALIZATIONS_PEAKS_TITLE_FALLING_BLOCK"] = "Falling block",
        ["VIEW_VISUALIZATIONS_PEAKS_USE_FLOATING_BAR_GRAVITY_DESC"] = "It allows the marker to accelerate downwards, fall off the screen, and reappear at the current level.",
        ["VIEW_VISUALIZATIONS_PEAKS_USE_FLOATING_BAR_GRAVITY_TITLE"] = "Marker gravity",
        ["VIEW_VISUALIZATIONS_PEAKS_VERTICAL_BAR_SPACE_DESC"] = "Specifies the vertical spacing between the blocks of a single strip.",
        ["VIEW_VISUALIZATIONS_PEAKS_VERTICAL_BAR_SPACE_TITLE"] = "Vertical spacing between blocks",
        ["PRESET_DEFAULT"] = "Peaks - Mirrored",
    });
}
