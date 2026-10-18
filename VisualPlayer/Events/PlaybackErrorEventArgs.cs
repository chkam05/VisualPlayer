using System;
using VisualPlayer.Models.Player;

namespace VisualPlayer.Events;

public sealed class PlaybackErrorEventArgs(Track? track, string details, int failedTrackCount = 1) : EventArgs
{
    public Track? Track { get; } = track;
    public string Details { get; } = details;
    public int FailedTrackCount { get; } = failedTrackCount;
}
