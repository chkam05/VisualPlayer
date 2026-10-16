using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace VisualPlayer.Controls;

public class PlayerMiniControl : PlayerControl
{
    public event EventHandler<PointerPressedEventArgs>? DragRequested;

    public PlayerMiniControl()
    {
        AddHandler(
            InputElement.PointerPressedEvent,
            PlayerMiniControl_PointerPressed,
            RoutingStrategies.Tunnel,
            handledEventsToo: true);
    }

    private void PlayerMiniControl_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            return;

        if (e.Source is not Visual source
            || source.FindAncestorOfType<WindowResizeBorder>(includeSelf: true) is not null
            || source.FindAncestorOfType<Button>(includeSelf: true) is not null
            || source.FindAncestorOfType<Slider>(includeSelf: true) is not null)
        {
            return;
        }

        DragRequested?.Invoke(this, e);
    }
}
