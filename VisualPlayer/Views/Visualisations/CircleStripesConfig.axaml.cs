using System;
using System.Collections.Generic;
using Avalonia.Controls;
using VisualPlayer.Enums;

namespace VisualPlayer.Views.Visualisations;

public partial class CircleStripesConfig : UserControl
{
    public IReadOnlyList<BeamEffect> BeamEffects { get; }
        = Enum.GetValues<BeamEffect>();

    public IReadOnlyList<RadialVisualizationDirection> Directions { get; }
        = Enum.GetValues<RadialVisualizationDirection>();

    public IReadOnlyList<RotationDirection> RotationDirections { get; }
        = Enum.GetValues<RotationDirection>();

    public CircleStripesConfig()
    {
        InitializeComponent();
    }
}
