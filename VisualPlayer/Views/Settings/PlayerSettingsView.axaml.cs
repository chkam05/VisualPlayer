using Avalonia.Controls;
using Avalonia.Interactivity;
using VisualPlayer.Contexts;

namespace VisualPlayer.Views.Settings;

public partial class PlayerSettingsView : UserControl
{
    #region CONSTRUCTORS

    public PlayerSettingsView()
    {
        InitializeComponent();
    }

    #endregion CONSTRUCTORS

    #region ACTIONS

    private void ResetButton_Click(object? sender, RoutedEventArgs e)
    {
        var config = ApplicationContext.Instance.Config.Player;

        config.CrossfadeDuration = ViewModels.Config.PlayerConfig.CROSSFADE_DURATION_DEFAULT;
        config.CrossfadeEnabled = false;
        config.PlaybackSpeed = ViewModels.Config.PlayerConfig.PLAYBACK_SPEED_DEFAULT;
        config.PlaybackSpeedPreset1 = ViewModels.Config.PlayerConfig.PLAYBACK_SPEED_PRESET_1_DEFAULT;
        config.PlaybackSpeedPreset2 = ViewModels.Config.PlayerConfig.PLAYBACK_SPEED_PRESET_2_DEFAULT;
        config.PlaybackSpeedPreset3 = ViewModels.Config.PlayerConfig.PLAYBACK_SPEED_PRESET_3_DEFAULT;
        config.PlaybackSpeedPreset4 = ViewModels.Config.PlayerConfig.PLAYBACK_SPEED_PRESET_4_DEFAULT;
        config.PlayOnStartup = false;
        config.PlayAfterAddingFiles = false;
        config.RememberLastPlayList = false;
        config.ShowRemainingTime = false;
    }

    #endregion ACTIONS
}
