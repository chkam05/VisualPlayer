using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;

namespace VisualPlayer.Controls;

public class WindowResizeBorder : Border
{
    private double _screenUnitsPerDip;
    private PixelPoint _startPointerPosition;
    private PixelPoint _startWindowPosition;
    private double _startWidth;
    private Window? _window;

    public WindowEdge Edge { get; set; }

    public WindowResizeBorder()
    {
        Background = Brushes.Transparent;
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed
            || TopLevel.GetTopLevel(this) is not Window window)
        {
            return;
        }

        _window = window;
        var screenOrigin = window.PointToScreen(default);
        var screenSample = window.PointToScreen(new Point(100, 0));
        _screenUnitsPerDip = Math.Abs(screenSample.X - screenOrigin.X) / 100d;
        if (!double.IsFinite(_screenUnitsPerDip) || _screenUnitsPerDip <= 0)
            _screenUnitsPerDip = 1;

        _startPointerPosition = window.PointToScreen(e.GetPosition(window));
        _startWindowPosition = window.Position;
        _startWidth = window.ClientSize.Width;
        e.Pointer.Capture(this);
        e.Handled = true;
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);

        if (_window is not { } window || e.Pointer.Captured != this)
            return;

        var pointerPosition = window.PointToScreen(e.GetPosition(window));
        var pointerDelta = (pointerPosition.X - _startPointerPosition.X) / _screenUnitsPerDip;
        var requestedWidth = Edge == WindowEdge.West
            ? _startWidth - pointerDelta
            : _startWidth + pointerDelta;
        var width = Math.Clamp(requestedWidth, window.MinWidth, window.MaxWidth);

        window.Width = width;

        if (Edge == WindowEdge.West)
        {
            var actualDelta = _startWidth - width;
            window.Position = new PixelPoint(
                _startWindowPosition.X + (int)Math.Round(actualDelta * _screenUnitsPerDip),
                _startWindowPosition.Y);
        }

        e.Handled = true;
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        if (e.Pointer.Captured == this)
            e.Pointer.Capture(null);

        _window = null;
        e.Handled = true;
    }

    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
    {
        base.OnPointerCaptureLost(e);
        _window = null;
    }
}
