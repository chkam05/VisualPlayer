using System;
using System.Text.Json.Serialization;
using Avalonia.Media;
using VisualPlayer.Utils;

namespace VisualPlayer.Models.Config;

public class AccentColor : IEquatable<AccentColor>
{
    private readonly Color _color;

    #region PROPERTIES

    public string HexCode => ToHexCode();
    public string Name { get; }

    #endregion PROPERTIES

    #region CONSTRUCTORS

    [JsonConstructor]
    public AccentColor(string hexCode, string name)
    {
        if (!Color.TryParse(hexCode, out var color))
            throw new ArgumentException("Invalid color code.", nameof(hexCode));

        _color = color;
        Name = ValidateName(name);
    }

    public AccentColor(Color color, string name)
    {
        _color = color;
        Name = ValidateName(name);
    }

    public AccentColor(AHSLColor color, string name)
        : this(color?.ToColor() ?? throw new ArgumentNullException(nameof(color)), name)
    {
        //
    }

    #endregion CONSTRUCTORS

    #region CONVERSION

    public AHSLColor ToAHSLColor()
    {
        return AHSLColor.FromColor(_color);
    }

    public Color ToColor()
    {
        return _color;
    }

    public string ToHexCode()
    {
        return _color.A == byte.MaxValue
            ? $"#{_color.R:X2}{_color.G:X2}{_color.B:X2}"
            : $"#{_color.A:X2}{_color.R:X2}{_color.G:X2}{_color.B:X2}";
    }

    #endregion CONVERSION

    #region UTILITIES

    public bool Equals(AccentColor? other)
    {
        return other is not null && _color.Equals(other._color);
    }

    public override bool Equals(object? obj)
    {
        return obj is AccentColor other && Equals(other);
    }

    public override int GetHashCode()
    {
        return _color.GetHashCode();
    }

    public override string ToString()
    {
        return $"{Name} ({ToHexCode()})";
    }

    private static string ValidateName(string name)
    {
        return name?.Trim() ?? string.Empty;
    }

    #endregion UTILITIES
}
