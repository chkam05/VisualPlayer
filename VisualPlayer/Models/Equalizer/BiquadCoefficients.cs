namespace VisualPlayer.Models.Equalizer;

public readonly record struct BiquadCoefficients(
    bool Enabled, double B0, double B1, double B2, double A1, double A2)
{
    public static BiquadCoefficients Disabled { get; } = new(false, 1, 0, 0, 0, 0);
}
