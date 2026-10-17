using VisualPlayer.Attributes;
using VisualPlayer.Resources;

namespace VisualPlayer.ViewModels.Config;

public class WindowConfig : ViewModelBase
{
    private int _posX = 100;
    private int _posY = 100;
    private double _height = 760;
    private bool _isMiniMode;
    private double _miniHeight = 128;
    private double _miniWidth = Dimensions.PlayerMiniControlMinimumWidth;
    private bool _playListOpen;
    private double _playListWidth = 640;
    private double _width = 1200;


    [NotifyPropertyUpdate]
    public int PosX
    {
        get => _posX;
        set => UpdateProperty(ref _posX, value);
    }

    [NotifyPropertyUpdate]
    public int PosY
    {
        get => _posY;
        set => UpdateProperty(ref _posY, value);
    }

    [NotifyPropertyUpdate]
    public double Height
    {
        get => _height;
        set => UpdateProperty(ref _height, value);
    }

    [NotifyPropertyUpdate]
    public bool IsMiniMode
    {
        get => _isMiniMode;
        set => UpdateProperty(ref _isMiniMode, value);
    }

    [NotifyPropertyUpdate]
    public double MiniHeight
    {
        get => _miniHeight;
        set => UpdateProperty(ref _miniHeight, value);
    }

    [NotifyPropertyUpdate]
    public double MiniWidth
    {
        get => _miniWidth;
        set => UpdateProperty(ref _miniWidth, value);
    }

    [NotifyPropertyUpdate]
    public bool PlayListOpen
    {
        get => _playListOpen;
        set => UpdateProperty(ref _playListOpen, value);
    }

    [NotifyPropertyUpdate]
    public double PlayListWidth
    {
        get => _playListWidth;
        set => UpdateProperty(ref _playListWidth, value);
    }

    [NotifyPropertyUpdate]
    public double Width
    {
        get => _width;
        set => UpdateProperty(ref _width, value);
    }
}
