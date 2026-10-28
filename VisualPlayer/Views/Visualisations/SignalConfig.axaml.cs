using System;
using System.Collections.Generic;
using Avalonia.Controls;
using VisualPlayer.Enums;

namespace VisualPlayer.Views.Visualisations;

public partial class SignalConfig : UserControl
{
    public IReadOnlyList<BeamEffect> BeamEffects { get; } = Enum.GetValues<BeamEffect>();
    public IReadOnlyList<VisualizationDirection> Directions { get; } = Enum.GetValues<VisualizationDirection>();

    public SignalConfig()
    {
        InitializeComponent();
    }
}
