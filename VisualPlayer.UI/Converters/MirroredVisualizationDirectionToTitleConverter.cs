using VisualPlayer.Enums;

namespace VisualPlayer.Converters;

public class MirroredVisualizationDirectionToTitleConverter : LocalizedEnumConverter
{
    protected override string? GetTranslationKey(object? value) => value switch
    {
        MirroredVisualizationDirection.Inside => "ENUM_MIRROREDVISUALIZATIONDIRECTION_INSIDE",
        MirroredVisualizationDirection.Outside => "ENUM_MIRROREDVISUALIZATIONDIRECTION_OUTSIDE",
        _ => null
    };
}
