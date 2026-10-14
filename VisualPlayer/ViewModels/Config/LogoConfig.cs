using System;
using System.Text.Json.Serialization;
using Avalonia.Media;
using VisualPlayer.Attributes;
using VisualPlayer.Models.Config;
using VisualPlayer.Resources;

namespace VisualPlayer.ViewModels.Config;

public class LogoConfig : ViewModelBase
{
    #region CONST

    public const int ANIMATION_CONTRACTION_DEFAULT = 6;
    public const int ANIMATION_CONTRACTION_INCREMENT = 1;
    public const int ANIMATION_CONTRACTION_MAXIMUM = 20;
    public const int ANIMATION_CONTRACTION_MINIMUM = 1;
    public const int ANIMATION_FREQUENCY_INCREMENT = 10;
    public const int ANIMATION_FREQUENCY_MAXIMUM = 1000;
    public const int ANIMATION_FREQUENCY_MINIMUM = 0;
    public const int ANIMATION_SCALE_DEFAULT = 50;
    public const int ANIMATION_SCALE_INCREMENT = 5;
    public const int ANIMATION_SCALE_MAXIMUM = 100;
    public const int ANIMATION_SCALE_MINIMUM = 0;
    public const int ANIMATION_SENSITIVITY_DEFAULT = 30;
    public const int ANIMATION_SENSITIVITY_INCREMENT = 5;
    public const int ANIMATION_SENSITIVITY_MAXIMUM = 100;
    public const int ANIMATION_SENSITIVITY_MINIMUM = 0;
    public const int MAXIMUM_FREQUENCY_DEFAULT = 180;
    public const int MINIMUM_FREQUENCY_DEFAULT = 90;
    public const double OPACITY_DEFAULT = 0.85;
    public const double OPACITY_MAXIMUM = 1.0;
    public const double OPACITY_MINIMUM = 0.5;
    public const double OPACITY_SMALL_CHANGE = 0.01;

    #endregion CONST

    private bool _animationEnabled = true;
    private int _contraction = ANIMATION_CONTRACTION_DEFAULT;
    private bool _enabled = true;
    private int _maximumFrequency = MAXIMUM_FREQUENCY_DEFAULT;
    private int _minimumFrequency = MINIMUM_FREQUENCY_DEFAULT;
    private double _opacity = OPACITY_DEFAULT;
    private int _scale = ANIMATION_SCALE_DEFAULT;
    private int _sensitivity = ANIMATION_SENSITIVITY_DEFAULT;
    private AccentColor _borderColor1 = new("#000000", "Czarny");
    private AccentColor _borderColor2 = new("#000000", "Czarny");
    private AccentColor _fillColor1 = AccentColors.Red;
    private AccentColor _fillColor2 = AccentColors.Blue;

    #region PROPERTIES

    [NotifyPropertyUpdate]
    public bool AnimationEnabled
    {
        get => _animationEnabled;
        set => UpdateProperty(ref _animationEnabled, value);
    }

    [NotifyPropertyUpdate]
    public int Contraction
    {
        get => _contraction;
        set => UpdateProperty(
            ref _contraction,
            Math.Clamp(value, ANIMATION_CONTRACTION_MINIMUM, ANIMATION_CONTRACTION_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public bool Enabled
    {
        get => _enabled;
        set => UpdateProperty(ref _enabled, value);
    }

    [NotifyPropertyUpdate]
    public int MaximumFrequency
    {
        get => _maximumFrequency;
        set => UpdateProperty(
            ref _maximumFrequency,
            Math.Clamp(value, MinimumFrequency + 1, ANIMATION_FREQUENCY_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public int MinimumFrequency
    {
        get => _minimumFrequency;
        set => UpdateProperty(
            ref _minimumFrequency,
            Math.Clamp(value, ANIMATION_FREQUENCY_MINIMUM, MaximumFrequency - 1));
    }

    [NotifyPropertyUpdate]
    public double Opacity
    {
        get => _opacity;
        set => UpdateProperty(ref _opacity, Math.Clamp(value, OPACITY_MINIMUM, OPACITY_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public int Scale
    {
        get => _scale;
        set => UpdateProperty(
            ref _scale,
            Math.Clamp(value, ANIMATION_SCALE_MINIMUM, ANIMATION_SCALE_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public int Sensitivity
    {
        get => _sensitivity;
        set => UpdateProperty(
            ref _sensitivity,
            Math.Clamp(value, ANIMATION_SENSITIVITY_MINIMUM, ANIMATION_SENSITIVITY_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public AccentColor BorderColor1
    {
        get => _borderColor1;
        set
        {
            if (value is null || _borderColor1.Equals(value))
                return;

            UpdateProperty(ref _borderColor1, value);
            NotifyPropertyChanged(nameof(BorderBrush1));
        }
    }

    [NotifyPropertyUpdate]
    public AccentColor BorderColor2
    {
        get => _borderColor2;
        set
        {
            if (value is null || _borderColor2.Equals(value))
                return;

            UpdateProperty(ref _borderColor2, value);
            NotifyPropertyChanged(nameof(BorderBrush2));
        }
    }

    [NotifyPropertyUpdate]
    public AccentColor FillColor1
    {
        get => _fillColor1;
        set
        {
            if (value is null || _fillColor1.Equals(value))
                return;

            UpdateProperty(ref _fillColor1, value);
            NotifyPropertyChanged(nameof(FillBrush1));
        }
    }

    [NotifyPropertyUpdate]
    public AccentColor FillColor2
    {
        get => _fillColor2;
        set
        {
            if (value is null || _fillColor2.Equals(value))
                return;

            UpdateProperty(ref _fillColor2, value);
            NotifyPropertyChanged(nameof(FillBrush2));
        }
    }

    [JsonIgnore]
    public SolidColorBrush BorderBrush1
    {
        get => new(BorderColor1.ToColor());
    }

    [JsonIgnore]
    public SolidColorBrush BorderBrush2
    {
        get => new(BorderColor2.ToColor());
    }

    [JsonIgnore]
    public SolidColorBrush FillBrush1
    {
        get => new(FillColor1.ToColor());
    }

    [JsonIgnore]
    public SolidColorBrush FillBrush2
    {
        get => new(FillColor2.ToColor());
    }

    #endregion PROPERTIES
}
