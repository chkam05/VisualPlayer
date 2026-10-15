using System;
using VisualPlayer.Attributes;
using VisualPlayer.Visualizations.Config.Base;

namespace VisualPlayer.Visualizations.Config;

public class StripesBouncingVisualizationConfig : BouncingVisualizationConfig
{
    private int _stripesCount = VisualizationBaseConfig.DEFAULT_BARS_COUNT;

    [NotifyPropertyUpdate]
    public int StripesCount
    {
        get => _stripesCount;
        set => UpdateProperty(ref _stripesCount, Math.Clamp(value, VisualizationBaseConfig.BARS_COUNT_MINIMUM, VisualizationBaseConfig.BARS_COUNT_MAXIMUM));
    }
}
