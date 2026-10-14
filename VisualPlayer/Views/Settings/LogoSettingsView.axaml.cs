using Avalonia.Controls;
using Avalonia.Interactivity;
using VisualPlayer.Contexts;
using VisualPlayer.ViewModels.Config;

namespace VisualPlayer.Views.Settings;

public partial class LogoSettingsView : UserControl
{
    #region CONSTRUCTORS

    public LogoSettingsView()
    {
        InitializeComponent();
    }

    #endregion CONSTRUCTORS

    #region ACTIONS

    private void ResetButton_Click(object? sender, RoutedEventArgs e)
    {
        ApplicationContext.Instance.Config.Logo = new LogoConfig();
    }

    #endregion ACTIONS
}
