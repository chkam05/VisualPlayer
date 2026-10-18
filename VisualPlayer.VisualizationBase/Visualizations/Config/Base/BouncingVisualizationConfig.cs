using System;
using VisualPlayer.Attributes;
using VisualPlayer.Interfaces;
using VisualPlayer.ViewModels;

namespace VisualPlayer.Visualizations.Config.Base;

public abstract class BouncingVisualizationConfig : ViewModelBase, IVisualizationConfig
{
    private int _descentRate = VisualizationBaseConfig.DEFAULT_DESCENT_RATE;
    private int _upperEdgeDescentRate = VisualizationBaseConfig.DEFAULT_FLOATING_DESCENT_RATE;
    private int _horizontalBarSpace = VisualizationBaseConfig.DEFAULT_BAR_HORIZONTAL_SPACE;

    [NotifyPropertyUpdate]
    public int DescentRate
    {
        get => _descentRate;
        set => UpdateProperty(ref _descentRate, Math.Clamp(value, VisualizationBaseConfig.DESCENT_RATE_MINIMUM, VisualizationBaseConfig.DESCENT_RATE_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public int UpperEdgeDescentRate
    {
        get => _upperEdgeDescentRate;
        set => UpdateProperty(ref _upperEdgeDescentRate, Math.Clamp(value, VisualizationBaseConfig.DESCENT_RATE_MINIMUM, VisualizationBaseConfig.DESCENT_RATE_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public int HorizontalBarSpace
    {
        get => _horizontalBarSpace;
        set => UpdateProperty(ref _horizontalBarSpace, Math.Clamp(value, VisualizationBaseConfig.BAR_SPACE_MINIMUM, VisualizationBaseConfig.BAR_SPACE_MAXIMUM));
    }
}
