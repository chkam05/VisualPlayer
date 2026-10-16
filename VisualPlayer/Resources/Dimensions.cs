using System;
using Avalonia;
using Avalonia.Controls;

namespace VisualPlayer.Resources;

public static class Dimensions
{
    #region AUTO HIDE

    public const double AutoHideRevealSize = 32;
    public const double InfoBarMinimumWidth = 384;
    public const double PlayerControlMinimumWidth = 704;
    public const double SidePanelCollapsedWidth = 64;
    public const double SidePanelExpandedMenuWidth = 248;
    public const double SidePanelOuterLeftMargin = 8;
    public const double SidePanelMinimumPlayListWidth = 420;
    public const double SidePanelResizeThumbWidth = 16;

    public const double MainWindowMinimumWidth
        = SidePanelCollapsedWidth
        + SidePanelMinimumPlayListWidth
        + SidePanelResizeThumbWidth
        + InfoBarMinimumWidth;

    public static readonly TimeSpan SidePanelAnimationDuration
        = TimeSpan.FromMilliseconds(240);

    public static readonly GridLength AutoHideRevealGridLength
        = new(AutoHideRevealSize);

    #endregion AUTO HIDE
}
