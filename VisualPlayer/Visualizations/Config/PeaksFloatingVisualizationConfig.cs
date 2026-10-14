using VisualPlayer.Attributes;
using VisualPlayer.Enums;
using VisualPlayer.Interfaces;
using VisualPlayer.ViewModels;

using VisualPlayer.Visualizations.Config.Base;

namespace VisualPlayer.Visualizations.Config;

public class PeaksFloatingVisualizationConfig : ViewModelBase, IVisualizationConfig
{
    private int _barHeight = VisualizationBaseConfig.DEFAULT_BAR_HEIGHT;
    private int _descentRate = VisualizationBaseConfig.DEFAULT_DESCENT_RATE;
    private VisualizationDirection _direction = VisualizationDirection.BottomUp;
    private int _horizontalBarSpace = VisualizationBaseConfig.DEFAULT_BAR_HORIZONTAL_SPACE;
    private int _peaksCount = VisualizationBaseConfig.DEFAULT_BARS_COUNT;

    #region PROPERTIES

    [NotifyPropertyUpdate]
    public int BarHeight
    {
        get => _barHeight;
        set => UpdateProperty(ref _barHeight, System.Math.Clamp(value, VisualizationBaseConfig.BAR_HEIGHT_MINIMUM, VisualizationBaseConfig.BAR_HEIGHT_MAXIMUM));
    }
    
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
    public int HorizontalBarSpace
    {
        get => _horizontalBarSpace;
        set => UpdateProperty(ref _horizontalBarSpace, System.Math.Clamp(value, VisualizationBaseConfig.BAR_SPACE_MINIMUM, VisualizationBaseConfig.BAR_SPACE_MAXIMUM));
    }
    
    [NotifyPropertyUpdate]
    public int PeaksCount
    {
        get => _peaksCount;
        set => UpdateProperty(ref _peaksCount, System.Math.Clamp(value, VisualizationBaseConfig.BARS_COUNT_MINIMUM, VisualizationBaseConfig.BARS_COUNT_MAXIMUM));
    }

    #endregion PROPERTIES
}
