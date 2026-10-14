using System;
using VisualPlayer.Models.Config;

namespace VisualPlayer.Events;

public class AccentColorChangedEventArgs : EventArgs
{
    public AccentColor Color { get; }


    public AccentColorChangedEventArgs(AccentColor color)
    {
        Color = color;
    }
}
