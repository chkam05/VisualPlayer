using System;
using VisualPlayer.Models.Player;

namespace VisualPlayer.Events;

public class TrackEventArgs : EventArgs
{
    public Track Track { get; }


    public TrackEventArgs(Track track)
    {
        Track = track;
    }
}
