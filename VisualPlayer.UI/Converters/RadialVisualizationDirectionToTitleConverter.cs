using VisualPlayer.Enums;

namespace VisualPlayer.Converters;

public class RadialVisualizationDirectionToTitleConverter : LocalizedEnumConverter
{
    protected override string? GetTranslationKey(object? value) => value switch
    {
        RadialVisualizationDirection.InsideOut => "ENUM_RADIALVISUALIZATIONDIRECTION_INSIDEOUT",
        RadialVisualizationDirection.OutsideIn => "ENUM_RADIALVISUALIZATIONDIRECTION_OUTSIDEIN",
        _ => null
    };
}
