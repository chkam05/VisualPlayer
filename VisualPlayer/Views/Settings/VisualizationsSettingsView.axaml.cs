using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using VisualPlayer.Contexts;
using VisualPlayer.Enums;
using VisualPlayer.Visualizations.Config.Base;

namespace VisualPlayer.Views.Settings;

public partial class VisualizationsSettingsView : UserControl
{
    public IReadOnlyList<ChannelMode> ChannelModes { get; } = Enum.GetValues<ChannelMode>();
    public IReadOnlyList<FftSize> FftSizes { get; } = Enum.GetValues<FftSize>();
    public IReadOnlyList<FftWindowType> FftWindows { get; } = Enum.GetValues<FftWindowType>();
    public IReadOnlyList<MiniVisualizationType> MiniVisualizationTypes { get; } = Enum.GetValues<MiniVisualizationType>();
    public IReadOnlyList<ScalingStrategy> ScalingStrategies { get; } = Enum.GetValues<ScalingStrategy>();

    #region CONSTRUCTORS

    public VisualizationsSettingsView()
    {
        InitializeComponent();
    }

    #endregion CONSTRUCTORS

    #region ACTIONS

    private void ResetButton_Click(object? sender, RoutedEventArgs e)
    {
        ApplicationContext.Instance.VisualizationConfig.Global = new VisualizationGlobalConfig();
    }

    #endregion ACTIONS
}
