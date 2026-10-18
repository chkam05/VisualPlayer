using System;
using VisualPlayer.Attributes;
using VisualPlayer.Visualizations.Config.Base;

namespace VisualPlayer.Visualizations.PeaksBouncing.Config;

public class PeaksBouncingVisualizationConfig : BouncingVisualizationConfig
{
    private int _peaksCount = VisualizationBaseConfig.DEFAULT_BARS_COUNT;
    private int _barHeight = VisualizationBaseConfig.DEFAULT_BAR_HEIGHT;
    private int _verticalBarSpace = VisualizationBaseConfig.DEFAULT_BAR_VERTICAL_SPACE;

    [NotifyPropertyUpdate]
    public int PeaksCount
    {
        get => _peaksCount;
        set => UpdateProperty(ref _peaksCount, Math.Clamp(value, VisualizationBaseConfig.BARS_COUNT_MINIMUM, VisualizationBaseConfig.BARS_COUNT_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public int BarHeight
    {
        get => _barHeight;
        set => UpdateProperty(ref _barHeight, Math.Clamp(value, VisualizationBaseConfig.BAR_HEIGHT_MINIMUM, VisualizationBaseConfig.BAR_HEIGHT_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public int VerticalBarSpace
    {
        get => _verticalBarSpace;
        set => UpdateProperty(ref _verticalBarSpace, Math.Clamp(value, VisualizationBaseConfig.BAR_SPACE_MINIMUM, VisualizationBaseConfig.BAR_SPACE_MAXIMUM));
    }
}
