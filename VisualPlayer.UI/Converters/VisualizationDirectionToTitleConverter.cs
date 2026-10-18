using VisualPlayer.Enums;

namespace VisualPlayer.Converters;

public class VisualizationDirectionToTitleConverter : LocalizedEnumConverter
{
    protected override string? GetTranslationKey(object? value) => value switch
    {
        VisualizationDirection.BottomUp => "ENUM_VISUALIZATIONDIRECTION_BOTTOMUP",
        VisualizationDirection.TopDown => "ENUM_VISUALIZATIONDIRECTION_TOPDOWN",
        _ => null
    };
}
