using System;
using VisualPlayer.Attributes;
using VisualPlayer.Enums;

namespace VisualPlayer.ViewModels.Config;

public sealed class MiniPlayerConfig : ViewModelBase
{
    #region CONST

    private const double DEFAULT_OPACITY = 0.90;

    public const double OPACITY_MAXIMUM = 1.0;
    public const double OPACITY_MINIMUM = 0.75;
    public const double OPACITY_SMALL_CHANGE = 0.01;

    #endregion CONST

    private double _opacity = DEFAULT_OPACITY;
    private bool _showCover = true;
    private bool _showVisualization;
    private bool _stayOnTop;
    private MiniVisualizationType _visualizationType = MiniVisualizationType.Regular;

    #region PROPERTIES

    [NotifyPropertyUpdate]
    public double Opacity
    {
        get => _opacity;
        set => UpdateProperty(ref _opacity, Math.Clamp(value, OPACITY_MINIMUM, OPACITY_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public bool ShowCover
    {
        get => _showCover;
        set => UpdateProperty(ref _showCover, value);
    }

    [NotifyPropertyUpdate]
    public bool ShowVisualization
    {
        get => _showVisualization;
        set => UpdateProperty(ref _showVisualization, value);
    }

    [NotifyPropertyUpdate]
    public bool StayOnTop
    {
        get => _stayOnTop;
        set => UpdateProperty(ref _stayOnTop, value);
    }

    [NotifyPropertyUpdate]
    public MiniVisualizationType VisualizationType
    {
        get => _visualizationType;
        set => UpdateProperty(ref _visualizationType, value);
    }

    #endregion PROPERTIES
}
