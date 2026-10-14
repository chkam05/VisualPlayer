using System;
using Avalonia.Media;
using VisualPlayer.Attributes;
using VisualPlayer.Models.Config;

namespace VisualPlayer.ViewModels.Config;

public class AccentColorViewModel : ViewModelBase
{
    #region PRIVATE VARIABLES

    private AccentColor _accentColor;
    private SolidColorBrush _brush;

    #endregion PRIVATE VARIABLES

    #region PROPERTIES

    [NotifyPropertyUpdate]
    public AccentColor AccentColor
    {
        get => _accentColor;

        set
        {
            ArgumentNullException.ThrowIfNull(value);

            if (ReferenceEquals(_accentColor, value))
                return;

            UpdateProperty(ref _accentColor, value);
            Brush = new SolidColorBrush(value.ToColor());

            NotifyPropertyChanged(nameof(HexCode));
            NotifyPropertyChanged(nameof(Name));
        }
    }

    [NotifyPropertyUpdate]
    public SolidColorBrush Brush
    {
        get => _brush;
        private set => UpdateProperty(ref _brush, value);
    }

    [NotifyPropertyUpdate]
    public string HexCode => AccentColor.ToHexCode();

    [NotifyPropertyUpdate]
    public string Name => string.IsNullOrWhiteSpace(AccentColor.Name)
        ? HexCode
        : AccentColor.Name;

    #endregion PROPERTIES

    #region CONSTRUCTORS

    public AccentColorViewModel(AccentColor accentColor)
    {
        _accentColor = accentColor ?? throw new ArgumentNullException(nameof(accentColor));
        _brush = new SolidColorBrush(accentColor.ToColor());
    }

    #endregion CONSTRUCTORS
}
