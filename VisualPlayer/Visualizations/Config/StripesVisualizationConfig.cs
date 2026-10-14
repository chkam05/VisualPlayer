using VisualPlayer.Attributes;
using VisualPlayer.Enums;
using VisualPlayer.Interfaces;
using VisualPlayer.ViewModels;

using VisualPlayer.Visualizations.Config.Base;

namespace VisualPlayer.Visualizations.Config;

public class StripesVisualizationConfig : ViewModelBase, IVisualizationConfig
{
    private int _descentRate = VisualizationBaseConfig.DEFAULT_DESCENT_RATE;
    private VisualizationDirection _direction = VisualizationDirection.BottomUp;
    private int _floatingBarDescentRate = VisualizationBaseConfig.DEFAULT_FLOATING_DESCENT_RATE;
    private int _floatingBarHeight = VisualizationBaseConfig.DEFAULT_BAR_HEIGHT;
    private int _horizontalBarSpace = VisualizationBaseConfig.DEFAULT_BAR_HORIZONTAL_SPACE;
    private bool _showFloatingBar = true;
    private int _stripesCount = VisualizationBaseConfig.DEFAULT_BARS_COUNT;
    private bool _useFloatingBarGravity = false;
    private int _verticalBarSpace = VisualizationBaseConfig.DEFAULT_BAR_VERTICAL_SPACE;

    #region PROPERTIES

    [NotifyPropertyUpdate]
    public int DescentRate
    {
        get => _descentRate;
        set => UpdateProperty(ref _descentRate, System.Math.Clamp(value, VisualizationBaseConfig.DESCENT_RATE_MINIMUM, VisualizationBaseConfig.DESCENT_RATE_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public VisualizationDirection Direction
    {
        get => _direction;
        set => UpdateProperty(ref _direction, value);
    }

    [NotifyPropertyUpdate]
    public int FloatingBarDescentRate
    {
        get => _floatingBarDescentRate;
        set => UpdateProperty(ref _floatingBarDescentRate, System.Math.Clamp(value, VisualizationBaseConfig.DESCENT_RATE_MINIMUM, VisualizationBaseConfig.DESCENT_RATE_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public int FloatingBarHeight
    {
        get => _floatingBarHeight;
        set => UpdateProperty(ref _floatingBarHeight, System.Math.Clamp(value, VisualizationBaseConfig.BAR_HEIGHT_MINIMUM, VisualizationBaseConfig.BAR_HEIGHT_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public int HorizontalBarSpace
    {
        get => _horizontalBarSpace;
        set => UpdateProperty(ref _horizontalBarSpace, System.Math.Clamp(value, VisualizationBaseConfig.BAR_SPACE_MINIMUM, VisualizationBaseConfig.BAR_SPACE_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public bool ShowFloatingBar
    {
        get => _showFloatingBar;
        set => UpdateProperty(ref _showFloatingBar, value);
    }

    [NotifyPropertyUpdate]
    public int StripesCount
    {
        get => _stripesCount;
        set => UpdateProperty(ref _stripesCount, System.Math.Clamp(value, VisualizationBaseConfig.BARS_COUNT_MINIMUM, VisualizationBaseConfig.BARS_COUNT_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public bool UseFloatingBarGravity
    {
        get => _useFloatingBarGravity;
        set => UpdateProperty(ref _useFloatingBarGravity, value);
    }

    [NotifyPropertyUpdate]
    public int VerticalBarSpace
    {
        get => _verticalBarSpace;
        set => UpdateProperty(ref _verticalBarSpace, System.Math.Clamp(value, VisualizationBaseConfig.BAR_SPACE_MINIMUM, VisualizationBaseConfig.BAR_SPACE_MAXIMUM));
    }

    #endregion PROPERTIES
}