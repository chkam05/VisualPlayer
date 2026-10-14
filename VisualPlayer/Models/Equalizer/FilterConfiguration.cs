namespace VisualPlayer.Models.Equalizer;

public record FilterConfiguration(
    BiquadCoefficients[] Coefficients, double PreampGain, bool IsFlat)
{
    public static FilterConfiguration Empty { get; } = new([], 1, true);
}
