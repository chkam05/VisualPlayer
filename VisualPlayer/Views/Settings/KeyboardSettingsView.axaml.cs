using Avalonia.Controls;
using Avalonia.Interactivity;
using VisualPlayer.Contexts;
using VisualPlayer.ViewModels.Config;

namespace VisualPlayer.Views.Settings;

public partial class KeyboardSettingsView : UserControl
{
    public KeyboardSettingsView()
    {
        InitializeComponent();
    }

    private void ResetButton_Click(object? sender, RoutedEventArgs e)
    {
        ApplicationContext.Instance.Config.Keyboard = new KeyboardConfig();
    }
}
