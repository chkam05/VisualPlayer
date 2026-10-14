using System;
using System.Collections.Generic;
using Avalonia.Controls;
using VisualPlayer.Enums;

namespace VisualPlayer.Views.Visualisations.Common;

public partial class VisualisationColorConfig : UserControl
{
    public IReadOnlyList<VisualizationColorMode> ColorModes { get; }
        = Enum.GetValues<VisualizationColorMode>();

    public IReadOnlyList<GradientType> GradientTypes { get; }
        = Enum.GetValues<GradientType>();


    public VisualisationColorConfig()
    {
        InitializeComponent();
    }
}
