using System.Collections.ObjectModel;

namespace VisualPlayer.Visualizations.PeaksFloating.Resources;

public static class EnglishTranslations
{
    public static IReadOnlyDictionary<string, string> Values { get; } = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
    {
        ["ENUM_VISUALIZATIONTYPE_PEAKSFLOATING"] = "Peaks - Floating",
        ["VIEW_VISUALIZATIONS_DESCENT_RATE_TITLE"] = "Descent rate",
        ["VIEW_VISUALIZATIONS_DIRECTION_TITLE"] = "Direction",
        ["VIEW_VISUALIZATIONS_NUM_TITLE"] = "Number of peaks",
        ["VIEW_VISUALIZATIONS_PEAKS_BAR_HEIGHT_DESC"] = "Specifies the height of a single block making up the strip.",
        ["VIEW_VISUALIZATIONS_PEAKS_BAR_HEIGHT_TITLE"] = "Block height",
        ["VIEW_VISUALIZATIONS_PEAKS_DESCENT_RATE_DESC"] = "Specifies the falling speed of the main bars in pixels per second.",
        ["VIEW_VISUALIZATIONS_PEAKS_DIRECTION_DESC"] = "Specifies whether the visualization grows inward or outward.",
        ["VIEW_VISUALIZATIONS_PEAKS_HORIZONTAL_BAR_SPACE_DESC"] = "Specifies the spacing between adjacent bands.",
        ["VIEW_VISUALIZATIONS_PEAKS_HORIZONTAL_BAR_SPACE_TITLE"] = "Horizontal block spacing",
        ["VIEW_VISUALIZATIONS_PEAKS_NUM_DESC"] = "Specifies the number of frequency bands displayed on the screen.",
        ["PRESET_DEFAULT"] = "Peaks - Floating",
    });
}
