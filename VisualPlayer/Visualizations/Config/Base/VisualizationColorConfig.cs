using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using VisualPlayer.Attributes;
using VisualPlayer.Enums;
using VisualPlayer.Models.Config;
using VisualPlayer.ViewModels;

namespace VisualPlayer.Visualizations.Config.Base;

public class VisualizationColorConfig : ViewModelBase
{
    #region CONST

    public const int ANIMATION_SPEED_INCREMENT = 5;
    public const int ANIMATION_SPEED_MAXIMUM = 500;
    public const int ANIMATION_SPEED_MINIMUM = -500;
    private const int DEFAULT_ANIMATION_SPEED = 30;
    public const int STRETCH_INCREMENT = 10;
    public const int STRETCH_MAXIMUM = 400;
    public const int STRETCH_MINIMUM = 10;

    #endregion CONST

    private bool _animateGradient;
    private GradientDefinition _gradient = new();
    private int _horizontalAnimationSpeed = DEFAULT_ANIMATION_SPEED;
    private VisualizationColorMode _mode = VisualizationColorMode.Solid;
    private ObservableCollection<AccentColor> _recentColors = [];
    private int _rainbowAngle;
    private GradientType _rainbowGradientType = GradientType.Linear;
    private AccentColor _solidColor = new("#10893E", string.Empty);
    private int _stretch = 100;
    private int _verticalAnimationSpeed;

    #region PROPERTIES

    [NotifyPropertyUpdate]
    public bool AnimateGradient
    {
        get => _animateGradient;
        set
        {
            if (value && HorizontalAnimationSpeed == 0 && VerticalAnimationSpeed == 0)
                HorizontalAnimationSpeed = DEFAULT_ANIMATION_SPEED;

            UpdateProperty(ref _animateGradient, value);
            NotifyPropertyChanged(nameof(IsAnimationEnabled));
        }
    }

    [NotifyPropertyUpdate]
    public GradientDefinition Gradient
    {
        get => _gradient;
        set => UpdateProperty(ref _gradient, value ?? new GradientDefinition());
    }

    [NotifyPropertyUpdate]
    public int HorizontalAnimationSpeed
    {
        get => _horizontalAnimationSpeed;
        set => UpdateProperty(ref _horizontalAnimationSpeed, System.Math.Clamp(value, ANIMATION_SPEED_MINIMUM, ANIMATION_SPEED_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public VisualizationColorMode Mode
    {
        get => _mode;
        set
        {
            UpdateProperty(ref _mode, value);
            NotifyPropertyChanged(nameof(IsAnimationAvailable));
            NotifyPropertyChanged(nameof(IsAnimationEnabled));
            NotifyPropertyChanged(nameof(IsGradient));
            NotifyPropertyChanged(nameof(IsRainbow));
            NotifyPropertyChanged(nameof(IsSolid));
            NotifyPropertyChanged(nameof(IsSolidOrGradient));
        }
    }

    [NotifyPropertyUpdate]
    public ObservableCollection<AccentColor> RecentColors
    {
        get => _recentColors;
        set => UpdateProperty(ref _recentColors, value ?? []);
    }

    [NotifyPropertyUpdate]
    public int RainbowAngle
    {
        get => _rainbowAngle;
        set => UpdateProperty(
            ref _rainbowAngle,
            ((value % 360) + 360) % 360);
    }

    [NotifyPropertyUpdate]
    public GradientType RainbowGradientType
    {
        get => _rainbowGradientType;
        set => UpdateProperty(ref _rainbowGradientType, value);
    }

    [NotifyPropertyUpdate]
    public AccentColor SolidColor
    {
        get => _solidColor;
        set => UpdateProperty(ref _solidColor, value ?? new AccentColor("#0078D7", string.Empty));
    }

    [NotifyPropertyUpdate]
    public int Stretch
    {
        get => _stretch;
        set => UpdateProperty(ref _stretch, System.Math.Clamp(value, STRETCH_MINIMUM, STRETCH_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public int VerticalAnimationSpeed
    {
        get => _verticalAnimationSpeed;
        set => UpdateProperty(ref _verticalAnimationSpeed, System.Math.Clamp(value, ANIMATION_SPEED_MINIMUM, ANIMATION_SPEED_MAXIMUM));
    }


    [JsonIgnore]
    public bool IsAnimationAvailable
    {
        get => Mode is VisualizationColorMode.Gradient or VisualizationColorMode.Rainbow;
    }

    [JsonIgnore]
    public bool IsAnimationEnabled
    {
        get => IsAnimationAvailable && AnimateGradient;
    }

    [JsonIgnore]
    public bool IsGradient
    {
        get => Mode == VisualizationColorMode.Gradient;
    }

    [JsonIgnore]
    public bool IsSolid
    {
        get => Mode == VisualizationColorMode.Solid;
    }

    [JsonIgnore]
    public bool IsRainbow
    {
        get => Mode == VisualizationColorMode.Rainbow;
    }

    [JsonIgnore]
    public bool IsSolidOrGradient
    {
        get => Mode is VisualizationColorMode.Solid or VisualizationColorMode.Gradient;
    }

    #endregion PROPERTIES    

    public VisualizationColorConfig Copy()
    {
        return new VisualizationColorConfig
        {
            AnimateGradient = AnimateGradient,
            Gradient = Gradient.Copy(),
            HorizontalAnimationSpeed = HorizontalAnimationSpeed,
            Mode = Mode,
            RainbowAngle = RainbowAngle,
            RainbowGradientType = RainbowGradientType,
            RecentColors = new ObservableCollection<AccentColor>(RecentColors),
            SolidColor = new AccentColor(SolidColor.ToColor(), SolidColor.Name),
            Stretch = Stretch,
            VerticalAnimationSpeed = VerticalAnimationSpeed
        };
    }
}
