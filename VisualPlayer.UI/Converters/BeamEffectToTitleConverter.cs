using VisualPlayer.Enums;

namespace VisualPlayer.Converters;

public class BeamEffectToTitleConverter : LocalizedEnumConverter
{
    protected override string? GetTranslationKey(object? value) => value switch
    {
        BeamEffect.None => "ENUM_BEAMEFFECT_NONE",
        BeamEffect.Beam => "ENUM_BEAMEFFECT_BEAM",
        _ => null
    };
}
