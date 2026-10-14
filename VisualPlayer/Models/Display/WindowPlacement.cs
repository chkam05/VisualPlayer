using Avalonia;

namespace VisualPlayer.Models.Display;

public readonly struct WindowPlacement
{
    public double Height { get; }
    public PixelPoint Position { get; }
    public double Width { get; }


    public WindowPlacement(PixelPoint position, double width, double height)
    {
        Position = position;
        Width = width;
        Height = height;
    }
}