using System;

namespace VisualPlayer.Events;

public class BackgroundImageChangedEventArgs : EventArgs
{
    public string ImagePath { get; }

    public BackgroundImageChangedEventArgs(string imagePath)
    {
        ImagePath = imagePath;
    }
}
