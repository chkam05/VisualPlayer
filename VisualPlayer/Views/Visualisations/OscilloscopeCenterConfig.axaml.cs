using System;
using System.Collections.Generic;
using Avalonia.Controls;
using VisualPlayer.Enums;

namespace VisualPlayer.Views.Visualisations;

public partial class OscilloscopeCenterConfig : UserControl
{
    public IReadOnlyList<BeamEffect> BeamEffects { get; } = Enum.GetValues<BeamEffect>();

    public OscilloscopeCenterConfig()
    {
        InitializeComponent();
    }
}
