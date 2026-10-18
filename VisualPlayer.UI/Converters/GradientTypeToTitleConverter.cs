using VisualPlayer.Enums;

namespace VisualPlayer.Converters;

public class GradientTypeToTitleConverter : LocalizedEnumConverter
{
    protected override string? GetTranslationKey(object? value) => value switch
    {
        GradientType.Linear => "ENUM_GRADIENTTYPE_LINEAR",
        GradientType.Radial => "ENUM_GRADIENTTYPE_RADIAL",
        GradientType.Conic => "ENUM_GRADIENTTYPE_CONIC",
        _ => null
    };
}
