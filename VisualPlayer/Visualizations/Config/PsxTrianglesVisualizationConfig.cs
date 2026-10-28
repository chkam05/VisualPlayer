using VisualPlayer.Attributes;
using VisualPlayer.Enums;
using VisualPlayer.Interfaces;
using VisualPlayer.ViewModels;
using VisualPlayer.Visualizations.Config.Base;

namespace VisualPlayer.Visualizations.Config;

public class PsxTrianglesVisualizationConfig : ViewModelBase, IVisualizationConfig
{
    #region CONST

    public const int GROUP_COUNT = 6;
    public const int MAX_TRIANGLE_WIDTH_MAXIMUM = 512;
    public const int MAX_TRIANGLE_WIDTH_MINIMUM = 8;
    public const int OFFSET_X_INCREMENT = 5;
    public const int OFFSET_X_MAXIMUM = 1000;
    public const int OFFSET_X_MINIMUM = -1000;
    public const double OPACITY_MAXIMUM = 1;
    public const double OPACITY_MINIMUM = 0;
    public const double OPACITY_SMALL_CHANGE = 0.01;
    public const int TRIANGLE_COUNT_MAXIMUM = 64;
    public const int TRIANGLE_COUNT_MINIMUM = 1;

    #endregion CONST

    private BeamEffect _borderEffect = BeamEffect.None;
    private bool _borderEnabled = true;
    private double _borderOpacity = 1;
    private double _centerLineOpacity = 0.5;
    private int _descentRate = VisualizationBaseConfig.DEFAULT_DESCENT_RATE;
    private int _group1TriangleCount = 2;
    private int _group2TriangleCount = 3;
    private int _group3TriangleCount = 5;
    private int _group4TriangleCount = 8;
    private int _group5TriangleCount = 13;
    private int _group6TriangleCount = 21;
    private double _group1Opacity = 0.9;
    private double _group2Opacity = 0.65;
    private double _group3Opacity = 0.85;
    private double _group4Opacity = 0.7;
    private double _group5Opacity = 0.8;
    private double _group6Opacity = 0.75;
    private int[] _layerOrder = [0, 2, 4, 5, 3, 1];
    private int _maximumTriangleWidth = 256;
    private int _offsetX = 5;
    private bool _splitBand = true;

    [NotifyPropertyUpdate]
    public BeamEffect BorderEffect
    {
        get => _borderEffect;
        set => UpdateProperty(ref _borderEffect, value);
    }

    [NotifyPropertyUpdate]
    public bool BorderEnabled
    {
        get => _borderEnabled;
        set => UpdateProperty(ref _borderEnabled, value);
    }

    [NotifyPropertyUpdate]
    public double BorderOpacity
    {
        get => _borderOpacity;
        set => UpdateProperty(ref _borderOpacity, ClampOpacity(value));
    }

    [NotifyPropertyUpdate]
    public double CenterLineOpacity
    {
        get => _centerLineOpacity;
        set => UpdateProperty(ref _centerLineOpacity, ClampOpacity(value));
    }

    [NotifyPropertyUpdate]
    public int DescentRate
    {
        get => _descentRate;
        set => UpdateProperty(ref _descentRate, System.Math.Clamp(value,
            VisualizationBaseConfig.DESCENT_RATE_MINIMUM,
            VisualizationBaseConfig.DESCENT_RATE_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public int Group1TriangleCount
    {
        get => _group1TriangleCount;
        set => UpdateProperty(ref _group1TriangleCount, ClampTriangleCount(value));
    }

    [NotifyPropertyUpdate]
    public int Group2TriangleCount
    {
        get => _group2TriangleCount;
        set => UpdateProperty(ref _group2TriangleCount, ClampTriangleCount(value));
    }

    [NotifyPropertyUpdate]
    public int Group3TriangleCount
    {
        get => _group3TriangleCount;
        set => UpdateProperty(ref _group3TriangleCount, ClampTriangleCount(value));
    }

    [NotifyPropertyUpdate]
    public int Group4TriangleCount
    {
        get => _group4TriangleCount;
        set => UpdateProperty(ref _group4TriangleCount, ClampTriangleCount(value));
    }

    [NotifyPropertyUpdate]
    public int Group5TriangleCount
    {
        get => _group5TriangleCount;
        set => UpdateProperty(ref _group5TriangleCount, ClampTriangleCount(value));
    }

    [NotifyPropertyUpdate]
    public int Group6TriangleCount
    {
        get => _group6TriangleCount;
        set => UpdateProperty(ref _group6TriangleCount, ClampTriangleCount(value));
    }

    [NotifyPropertyUpdate]
    public int MaximumTriangleWidth
    {
        get => _maximumTriangleWidth;
        set => UpdateProperty(ref _maximumTriangleWidth, System.Math.Clamp(value,
            MAX_TRIANGLE_WIDTH_MINIMUM,
            MAX_TRIANGLE_WIDTH_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public double Group1Opacity
    {
        get => _group1Opacity;
        set => UpdateProperty(ref _group1Opacity, ClampOpacity(value));
    }

    [NotifyPropertyUpdate]
    public double Group2Opacity
    {
        get => _group2Opacity;
        set => UpdateProperty(ref _group2Opacity, ClampOpacity(value));
    }

    [NotifyPropertyUpdate]
    public double Group3Opacity
    {
        get => _group3Opacity;
        set => UpdateProperty(ref _group3Opacity, ClampOpacity(value));
    }

    [NotifyPropertyUpdate]
    public double Group4Opacity
    {
        get => _group4Opacity;
        set => UpdateProperty(ref _group4Opacity, ClampOpacity(value));
    }

    [NotifyPropertyUpdate]
    public double Group5Opacity
    {
        get => _group5Opacity;
        set => UpdateProperty(ref _group5Opacity, ClampOpacity(value));
    }

    [NotifyPropertyUpdate]
    public double Group6Opacity
    {
        get => _group6Opacity;
        set => UpdateProperty(ref _group6Opacity, ClampOpacity(value));
    }

    [NotifyPropertyUpdate]
    public int[] LayerOrder
    {
        get => _layerOrder;
        set => UpdateProperty(ref _layerOrder, ValidateLayerOrder(value));
    }

    [NotifyPropertyUpdate]
    public int OffsetX
    {
        get => _offsetX;
        set => UpdateProperty(ref _offsetX, System.Math.Clamp(value,
            OFFSET_X_MINIMUM,
            OFFSET_X_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public bool SplitBand
    {
        get => _splitBand;
        set => UpdateProperty(ref _splitBand, value);
    }

    #region UTILS

    public double GetLayerOpacity(int groupIndex)
    {
        return groupIndex switch
        {
            0 => Group1Opacity,
            1 => Group2Opacity,
            2 => Group3Opacity,
            3 => Group4Opacity,
            4 => Group5Opacity,
            5 => Group6Opacity,
            _ => throw new System.ArgumentOutOfRangeException(nameof(groupIndex))
        };
    }

    public int GetTriangleCount(int groupIndex)
    {
        return groupIndex switch
        {
            0 => Group1TriangleCount,
            1 => Group2TriangleCount,
            2 => Group3TriangleCount,
            3 => Group4TriangleCount,
            4 => Group5TriangleCount,
            5 => Group6TriangleCount,
            _ => throw new System.ArgumentOutOfRangeException(nameof(groupIndex))
        };
    }

    public void SetLayerOpacity(int groupIndex, double value)
    {
        switch (groupIndex)
        {
            case 0: Group1Opacity = value; break;
            case 1: Group2Opacity = value; break;
            case 2: Group3Opacity = value; break;
            case 3: Group4Opacity = value; break;
            case 4: Group5Opacity = value; break;
            case 5: Group6Opacity = value; break;
            default: throw new System.ArgumentOutOfRangeException(nameof(groupIndex));
        }
    }

    public void SetTriangleCount(int groupIndex, int value)
    {
        switch (groupIndex)
        {
            case 0: Group1TriangleCount = value; break;
            case 1: Group2TriangleCount = value; break;
            case 2: Group3TriangleCount = value; break;
            case 3: Group4TriangleCount = value; break;
            case 4: Group5TriangleCount = value; break;
            case 5: Group6TriangleCount = value; break;
            default: throw new System.ArgumentOutOfRangeException(nameof(groupIndex));
        }
    }


    private static double ClampOpacity(double value)
    {
        return System.Math.Clamp(value, OPACITY_MINIMUM, OPACITY_MAXIMUM);
    }

    private static int ClampTriangleCount(int value)
    {
        return System.Math.Clamp(value, TRIANGLE_COUNT_MINIMUM, TRIANGLE_COUNT_MAXIMUM);
    }

    private static int[] ValidateLayerOrder(int[]? value)
    {
        if (value is null || value.Length != GROUP_COUNT)
            return [5, 4, 3, 2, 1, 0];

        var seen = new bool[GROUP_COUNT];
        foreach (var groupIndex in value)
        {
            if (groupIndex < 0 || groupIndex >= GROUP_COUNT || seen[groupIndex])
                return [5, 4, 3, 2, 1, 0];
            seen[groupIndex] = true;
        }

        return [.. value];
    }

    #endregion UTILS
}
