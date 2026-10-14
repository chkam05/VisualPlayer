using System;
using VisualPlayer.ViewModels.Config;

namespace VisualPlayer.Utils;

public static class PlaybackSpeedUtils
{
    public static int NormalizePercent(int value)
    {
        return Math.Clamp(
            value,
            PlayerConfig.PLAYBACK_SPEED_MINIMUM,
            PlayerConfig.PLAYBACK_SPEED_MAXIMUM);
    }

    public static int ToPercent(double playbackSpeed)
    {
        return NormalizePercent((int)Math.Round(playbackSpeed * 100));
    }

    public static double ToPlaybackSpeed(int percent)
    {
        return NormalizePercent(percent) / 100d;
    }
}
