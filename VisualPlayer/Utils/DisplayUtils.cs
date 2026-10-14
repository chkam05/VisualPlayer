using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using VisualPlayer.Models.Display;

namespace VisualPlayer.Utils;

public static class DisplayUtils
{
    public static WindowPlacement CorrectWindowPlacement(
        Window window, int posX, int posY, double width, double height)
    {
        if (!double.IsFinite(width) || width <= 0)
            width = Math.Max(1.0, window.ClientSize.Width);

        if (!double.IsFinite(height) || height <= 0)
            height = Math.Max(1.0, window.ClientSize.Height);

        var savedPosition = new PixelPoint(posX, posY);
        var screen = GetScreen(window.Screens, savedPosition);

        if (screen is null)
            return new WindowPlacement(savedPosition, width, height);

        var workingArea = screen.WorkingArea;

        if (workingArea.Width <= 0 || workingArea.Height <= 0)
            return new WindowPlacement(savedPosition, width, height);

        var scaling = screen.Scaling > 0 ? screen.Scaling : 1.0;
        var frameOffset = GetFrameOffset(window);

        //  Width and Height specify the size of the window client area.Width and Height specify the size of the window client area.
        //  WorkingArea, on the other hand, defines the area available to the entire window, including the native title bar and frame.

        var maxWidth = Math.Max(1.0, workingArea.Width / scaling - frameOffset.Width);
        var maxHeight = Math.Max(1.0, workingArea.Height / scaling - frameOffset.Height);

        //  First, keep the saved size.
        //  Reduce it only when the entire window physically cannot fit within the WorkingArea.
        var correctedWidth = Math.Min(width, maxWidth);
        var correctedHeight = Math.Min(height, maxHeight);

        //  To calculate the position, the physical size of the entire window in pixels is required.
        var frameSize = PixelSize.FromSize(
            new Size(
                correctedWidth + frameOffset.Width,
                correctedHeight + frameOffset.Height),
            scaling);
        
        //  Maximum achievable X/Y position.
        //  If the taskbar is, for example, on the left side, WorkingArea.X will be shifted accordingly.
        //  If the taskbar or menu bar is at the top, WorkingArea.Y will also be shifted.

        var maxX = Math.Max(workingArea.X, workingArea.Right - frameSize.Width);
        var maxY = Math.Max(workingArea.Y, workingArea.Bottom - frameSize.Height);

        //  Moving the window to the nearest reachable position.

        var correctedX = Math.Clamp(posX, workingArea.X, maxX);
        var correctedY = Math.Clamp(posY, workingArea.Y, maxY);

        return new WindowPlacement(
            new PixelPoint(correctedX, correctedY),
            correctedWidth,
            correctedHeight);
    }

    private static Screen? GetScreen(Screens screens, PixelPoint position)
    {
        //  An attempt to locate the exact monitor where the saved position was located.

        var screen = screens.ScreenFromPoint(position);

        if (screen is not null)
            return screen;

        //  The monitor may have been disconnected, or the screen layout has changed.
        //  Searching for the nearest available monitor.

        Screen? nearestScreen = null;
        var nearestDistance = double.MaxValue;

        foreach (var currentScreen in screens.All)
        {
            var distance = GetDistanceSquared(
                currentScreen.WorkingArea,
                position);

            if (distance >= nearestDistance)
                continue;

            nearestDistance = distance;
            nearestScreen = currentScreen;
        }

        return nearestScreen ?? screens.Primary;
    }

    private static Size GetFrameOffset(Window window)
    {
        if (window.FrameSize is not { } frameSize)
            return default;

        return new Size(
            Math.Max(0.0, frameSize.Width - window.ClientSize.Width),
            Math.Max(0.0, frameSize.Height - window.ClientSize.Height));
    }

    private static double GetDistanceSquared(PixelRect area, PixelPoint point)
    {
        double distanceX = 0;
        double distanceY = 0;

        if (point.X < area.X)
            distanceX = area.X - (double)point.X;
        else if (point.X > area.Right)
            distanceX = point.X - (double)area.Right;

        if (point.Y < area.Y)
            distanceY = area.Y - (double)point.Y;
        else if (point.Y > area.Bottom)
            distanceY = point.Y - (double)area.Bottom;

        return distanceX * distanceX + distanceY * distanceY;
    }
}