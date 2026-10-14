using System.Collections.ObjectModel;
using VisualPlayer.Attributes;
using VisualPlayer.Enums;
using VisualPlayer.ViewModels;

namespace VisualPlayer.Visualizations.Config.Base;

public class GradientDefinition : ViewModelBase
{
    #region CONST

    public const int ANGLE_MAXIMUM = 359;
    public const int ANGLE_MINIMUM = 0;
    private const int ANGLE_RANGE = 360;

    #endregion CONST

    private int _angle = 270;
    private ObservableCollection<GradientPoint> _points =
    [
        new GradientPoint { Color = new("#10893E", string.Empty), Position = 0 },
        new GradientPoint { Color = new("#10893E", string.Empty), Position = 0.33 },
        new GradientPoint { Color = new("#FFB900", string.Empty), Position = 0.66 },
        new GradientPoint { Color = new("#E81123", string.Empty), Position = 1 },
    ];
    private GradientType _type = GradientType.Linear;

    #region PROPERTIES

    [NotifyPropertyUpdate]
    public int Angle
    {
        get => _angle;
        set => UpdateProperty(ref _angle, ((value % ANGLE_RANGE) + ANGLE_RANGE) % ANGLE_RANGE);
    }

    [NotifyPropertyUpdate]
    public ObservableCollection<GradientPoint> Points
    {
        get => _points;
        set => UpdateProperty(ref _points, value ?? []);
    }

    [NotifyPropertyUpdate]
    public GradientType Type
    {
        get => _type;
        set => UpdateProperty(ref _type, value);
    }

    #endregion PROPERTIES

    public GradientDefinition Copy()
    {
        var result = new GradientDefinition
        {
            Angle = Angle,
            Type = Type,
            Points = []
        };

        foreach (var point in Points)
            result.Points.Add(point.Copy());

        return result;
    }
}
