using System;
using VisualPlayer.Visualizations.Config.Base;

namespace VisualPlayer.Events;

public class VisualizationProfileOverwriteRequestedEventArgs : EventArgs
{
    public VisualisationProfile Profile { get; }


    public VisualizationProfileOverwriteRequestedEventArgs(VisualisationProfile profile)
    {
        Profile = profile;
    }
}
