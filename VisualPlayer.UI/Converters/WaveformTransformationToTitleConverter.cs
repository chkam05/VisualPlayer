using VisualPlayer.Enums;

namespace VisualPlayer.Converters;

public class WaveformTransformationToTitleConverter : LocalizedEnumConverter
{
    protected override string? GetTranslationKey(object? value) => value switch
    {
        WaveformTransformation.None => "ENUM_BEAMEFFECT_NONE",
        WaveformTransformation.Sin => "ENUM_WAVEFORMTRANSFORMATION_SIN",
        WaveformTransformation.Cos => "ENUM_WAVEFORMTRANSFORMATION_COS",
        _ => null
    };
}
