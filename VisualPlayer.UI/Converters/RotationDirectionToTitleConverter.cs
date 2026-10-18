using VisualPlayer.Enums;

namespace VisualPlayer.Converters;

public class RotationDirectionToTitleConverter : LocalizedEnumConverter
{
    protected override string? GetTranslationKey(object? value) => value switch
    {
        RotationDirection.Clockwise => "ENUM_ROTATIONDIRECTION_CLOCKWISE",
        RotationDirection.CounterClockwise => "ENUM_ROTATIONDIRECTION_COUNTERCLOCKWISE",
        _ => null
    };
}
