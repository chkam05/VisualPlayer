using System.Collections.ObjectModel;

namespace VisualPlayer.Visualizations.StripesBouncing.Resources;

public static class EnglishTranslations
{
    public static IReadOnlyDictionary<string, string> Values { get; } = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
    {
        ["ENUM_VISUALIZATIONTYPE_STRIPESBOUNCING"] = "Stripes - Bouncing",
        ["VIEW_VISUALIZATIONS_DESCENT_RATE_TITLE"] = "Descent rate",
        ["VIEW_VISUALIZATIONS_BOUNCING_DESCENT_RATE_DESC"] = "Specifies how quickly the inner edge of each band falls toward the center, in pixels per second.",
        ["VIEW_VISUALIZATIONS_BOUNCING_UPPER_EDGE_DESCENT_RATE_TITLE"] = "Upper edge descent rate",
        ["VIEW_VISUALIZATIONS_BOUNCING_UPPER_EDGE_DESCENT_RATE_DESC"] = "Specifies how quickly the outer peak edge falls toward the center, in pixels per second. A slower rate creates a wider bouncing band.",
        ["VIEW_VISUALIZATIONS_NUM_TITLE"] = "Number of peaks",
        ["VIEW_VISUALIZATIONS_PEAKS_NUM_DESC"] = "Specifies the number of frequency bands displayed on the screen.",
        ["VIEW_VISUALIZATIONS_STRIPES_HORIZONTAL_BAR_SPACE_DESC"] = "Specifies the spacing between adjacent bars.",
        ["VIEW_VISUALIZATIONS_STRIPES_HORIZONTAL_BAR_SPACE_TITLE"] = "Horizontal bar spacing",
        ["PRESET_DEFAULT"] = "Stripes - Bouncing",
    });
}
