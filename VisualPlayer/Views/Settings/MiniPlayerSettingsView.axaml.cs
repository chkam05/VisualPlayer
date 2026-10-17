using System;
using System.Collections.Generic;
using Avalonia.Controls;
using VisualPlayer.Enums;

namespace VisualPlayer.Views.Settings;

public partial class MiniPlayerSettingsView : UserControl
{
    public IReadOnlyList<MiniVisualizationType> VisualizationTypes { get; }
        = Enum.GetValues<MiniVisualizationType>();

    public MiniPlayerSettingsView()
    {
        InitializeComponent();
    }
}
