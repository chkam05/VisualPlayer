using VisualPlayer.Enums;

namespace VisualPlayer.Converters;

public class VisualizationColorModeToTitleConverter : LocalizedEnumConverter
{
    protected override string? GetTranslationKey(object? value) => value switch
    {
        VisualizationColorMode.Solid => "ENUM_VISUALIZATIONCOLORMODE_SOLID",
        VisualizationColorMode.Gradient => "ENUM_VISUALIZATIONCOLORMODE_GRADIENT",
        VisualizationColorMode.Rainbow => "ENUM_VISUALIZATIONCOLORMODE_RAINBOW",
        _ => null
    };
}
