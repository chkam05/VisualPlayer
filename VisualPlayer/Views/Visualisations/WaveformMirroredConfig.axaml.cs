using System;
using System.Collections.Generic;
using Avalonia.Controls;
using VisualPlayer.Enums;

namespace VisualPlayer.Views.Visualisations;

public partial class WaveformMirroredConfig : UserControl
{
    public IReadOnlyList<BeamEffect> BeamEffects { get; }
        = Enum.GetValues<BeamEffect>();

    public IReadOnlyList<MirroredVisualizationDirection> Directions { get; }
        = Enum.GetValues<MirroredVisualizationDirection>();

    public IReadOnlyList<WaveformTransformation> Transformations { get; }
        = Enum.GetValues<WaveformTransformation>();

    public WaveformMirroredConfig()
    {
        InitializeComponent();
    }
}
