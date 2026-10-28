using System;
using System.Collections.Generic;
using Avalonia.Controls;
using VisualPlayer.Enums;

namespace VisualPlayer.Views.Visualisations;

public partial class SignalMirrored : UserControl
{
    public IReadOnlyList<BeamEffect> BeamEffects { get; } = Enum.GetValues<BeamEffect>();
    public IReadOnlyList<RotationDirection> RotationDirections { get; } = Enum.GetValues<RotationDirection>();

    public SignalMirrored()
    {
        InitializeComponent();
    }
}
