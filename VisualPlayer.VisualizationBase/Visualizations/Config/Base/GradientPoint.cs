using VisualPlayer.Attributes;
using VisualPlayer.Models.Config;
using VisualPlayer.ViewModels;

namespace VisualPlayer.Visualizations.Config.Base;

public class GradientPoint : ViewModelBase
{
    private AccentColor _color = new("#10893E", string.Empty);
    private double _position;

    #region PROPERTIES

    [NotifyPropertyUpdate]
    public AccentColor Color
    {
        get => _color;
        set => UpdateProperty(ref _color, value ?? new AccentColor("#10893E", string.Empty));
    }

    [NotifyPropertyUpdate]
    public double Position
    {
        get => _position;
        set => UpdateProperty(ref _position, System.Math.Clamp(value, 0, 1));
    }

    #endregion PROPERTIES

    public GradientPoint Copy()
    {
        return new GradientPoint
        {
            Color = new AccentColor(Color.ToColor(), Color.Name),
            Position = Position
        };
    }
}
