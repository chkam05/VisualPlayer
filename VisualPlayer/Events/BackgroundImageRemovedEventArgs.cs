using System;

namespace VisualPlayer.Events;

public class BackgroundImageRemovedEventArgs : EventArgs
{
    public string ImagePath { get; }

    public BackgroundImageRemovedEventArgs(string imagePath)
    {
        ImagePath = imagePath;
    }
}
