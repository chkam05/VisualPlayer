using System.Collections.ObjectModel;

namespace VisualPlayer.Visualizations.PeaksBouncing.Resources;

public static class EnglishTranslations
{
    public static IReadOnlyDictionary<string, string> Values { get; } = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
    {
        ["ENUM_VISUALIZATIONTYPE_PEAKSBOUNCING"] = "Peaks - Bouncing",
        ["VIEW_VISUALIZATIONS_DESCENT_RATE_TITLE"] = "Descent rate",
        ["VIEW_VISUALIZATIONS_BOUNCING_DESCENT_RATE_DESC"] = "Specifies how quickly the inner edge of each band falls toward the center, in pixels per second.",
        ["VIEW_VISUALIZATIONS_BOUNCING_UPPER_EDGE_DESCENT_RATE_TITLE"] = "Upper edge descent rate",
        ["VIEW_VISUALIZATIONS_BOUNCING_UPPER_EDGE_DESCENT_RATE_DESC"] = "Specifies how quickly the outer peak edge falls toward the center, in pixels per second. A slower rate creates a wider bouncing band.",
        ["VIEW_VISUALIZATIONS_NUM_TITLE"] = "Number of peaks",
        ["VIEW_VISUALIZATIONS_PEAKS_BAR_HEIGHT_DESC"] = "Specifies the height of a single block making up the strip.",
        ["VIEW_VISUALIZATIONS_PEAKS_BAR_HEIGHT_TITLE"] = "Block height",
        ["VIEW_VISUALIZATIONS_PEAKS_HORIZONTAL_BAR_SPACE_DESC"] = "Specifies the spacing between adjacent bands.",
        ["VIEW_VISUALIZATIONS_PEAKS_HORIZONTAL_BAR_SPACE_TITLE"] = "Horizontal block spacing",
        ["VIEW_VISUALIZATIONS_PEAKS_NUM_DESC"] = "Specifies the number of frequency bands displayed on the screen.",
        ["VIEW_VISUALIZATIONS_PEAKS_VERTICAL_BAR_SPACE_DESC"] = "Specifies the vertical spacing between the blocks of a single strip.",
        ["VIEW_VISUALIZATIONS_PEAKS_VERTICAL_BAR_SPACE_TITLE"] = "Vertical spacing between blocks",
        ["PRESET_DEFAULT"] = "Peaks - Bouncing",
    });
}
