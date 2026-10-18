using System;
using Avalonia.Media;

namespace VisualPlayer.Utils;

public class AHSLColor
{
    #region CONST

    public const int HUE_MAX = 1530;
    public const int HUE_MIN = 0;
    public const int SATURATION_MAX = 100;
    public const int SATURATION_MIN = 0;
    public const int LIGHTNESS_MAX = 100;
    public const int LIGHTNESS_MIN = 0;

    #endregion CONST

    #region PRIVATE VARIABLES

    private byte _alpha;
    private int _hue;
    private int _saturation;
    private int _lightness;

    #endregion PRIVATE VARIABLES

    #region PROPERTIES

    public byte A
    {
        get => _alpha;
        set => _alpha = value;
    }

    public int H
    {
        get => _hue;
        set => _hue = Math.Clamp(value, HUE_MIN, HUE_MAX);
    }

    public int S
    {
        get => _saturation;
        set => _saturation = Math.Clamp(value, SATURATION_MIN, SATURATION_MAX);
    }

    public int L
    {
        get => _lightness;
        set => _lightness = Math.Clamp(value, LIGHTNESS_MIN, LIGHTNESS_MAX);
    }

    #endregion PROPERTIES

    #region CONSTRUCTORS

    public AHSLColor(byte a, int h, int s, int l)
    {
        A = a;
        H = h;
        S = s;
        L = l;
    }

    #endregion CONSTRUCTORS

    #region CONVERSION

    public static AHSLColor FromColor(Color color)
    {
        var r = color.R / 255.0;
        var g = color.G / 255.0;
        var b = color.B / 255.0;

        var min = Math.Min(Math.Min(r, g), b);
        var max = Math.Max(Math.Max(r, g), b);
        var delta = max - min;

        var l = (max + min) / 2.0;

        if (delta == 0)
            return new AHSLColor(color.A, 0, 0, (int)Math.Round(l * 100));

        var s = l <= 0.5
            ? delta / (max + min)
            : delta / (2 - max - min);

        double h;

        if (r == max)
        {
            h = ((g - b) / 6) / delta;
        }
        else if (g == max)
        {
            h = (1.0 / 3) + ((b - r) / 6) / delta;
        }
        else
        {
            h = (2.0 / 3) + ((r - g) / 6) / delta;
        }

        if (h < 0)
            h += 1;

        if (h > 1)
            h -= 1;

        return new AHSLColor(
            color.A,
            (int)Math.Round(h * HUE_MAX),
            (int)Math.Round(s * 100),
            (int)Math.Round(l * 100));
    }

    public Color ToColor()
    {
        var h = H / (double)HUE_MAX;
        var s = S / 100.0;
        var l = L / 100.0;

        if (s == 0)
        {
            var value = ToByte(l);

            return Color.FromArgb(A, value, value, value);
        }

        var v2 = l < 0.5
            ? l * (1.0 + s)
            : (l + s) - (l * s);

        var v1 = 2 * l - v2;

        var r = HueToRGB(v1, v2, h + (1.0 / 3.0));
        var g = HueToRGB(v1, v2, h);
        var b = HueToRGB(v1, v2, h - (1.0 / 3.0));

        return Color.FromArgb(
            A,
            ToByte(r),
            ToByte(g),
            ToByte(b));
    }

    #endregion CONVERSION

    #region UTILITIES

    public AHSLColor With(byte? a = null, int? h = null, int? s = null, int? l = null)
    {
        return new AHSLColor(
            a ?? A,
            h ?? H,
            s ?? S,
            l ?? L);
    }

    public AHSLColor AdjustLightness(int value)
    {
        return With(l: L + value);
    }

    public AHSLColor AdjustSaturation(int value)
    {
        return With(s: S + value);
    }

    public override string ToString()
    {
        return $"A:{A} H:{H} S:{S} L:{L}";
    }

    private static byte ToByte(double value)
    {
        return (byte)Math.Clamp((int)Math.Round(value * 255), 0, 255);
    }

    private static double HueToRGB(double v1, double v2, double hue)
    {
        if (hue < 0)
            hue += 1;

        if (hue > 1)
            hue -= 1;

        if (6 * hue < 1)
            return v1 + (v2 - v1) * 6 * hue;

        if (2 * hue < 1)
            return v2;

        if (3 * hue < 2)
            return v1 + (v2 - v1) * ((2.0 / 3.0) - hue) * 6;

        return v1;
    }

    #endregion UTILITIES
}