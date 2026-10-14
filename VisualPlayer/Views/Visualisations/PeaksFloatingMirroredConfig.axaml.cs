using System;
using System.Collections.Generic;
using Avalonia.Controls;
using VisualPlayer.Enums;

namespace VisualPlayer.Views.Visualisations;

public partial class PeaksFloatingMirroredConfig : UserControl
{
    public IReadOnlyList<MirroredVisualizationDirection> Directions { get; }
        = Enum.GetValues<MirroredVisualizationDirection>();


    public PeaksFloatingMirroredConfig()
    {
        InitializeComponent();
    }
}
