using System;
using System.Collections.Generic;
using Avalonia.Controls;
using VisualPlayer.Enums;

namespace VisualPlayer.Views.Visualisations;

public partial class PeaksConfig : UserControl
{
    public IReadOnlyList<VisualizationDirection> Directions { get; }
        = Enum.GetValues<VisualizationDirection>();


    public PeaksConfig()
    {
        InitializeComponent();
    }
}
