using Avalonia.Interactivity;
using VisualPlayer.ViewModels;

namespace VisualPlayer.Events;

public class TrackRequestedEventArgs : RoutedEventArgs
{
    public TrackViewModel Track { get; }


    public TrackRequestedEventArgs(RoutedEvent routedEvent, TrackViewModel track) : base(routedEvent)
    {
        Track = track;
    }
}