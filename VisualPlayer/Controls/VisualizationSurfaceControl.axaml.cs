using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using VisualPlayer.Core;
using VisualPlayer.Resources;

namespace VisualPlayer.Controls;

public partial class VisualizationSurfaceControl : UserControl
{
    private readonly DispatcherTimer _renderTimer;
    private bool _isFramePending;

    #region AVALONIA PROPERTIES

    public static readonly StyledProperty<double> BottomChromeHeightProperty
        = AvaloniaProperty.Register<VisualizationSurfaceControl, double>(nameof(BottomChromeHeight));

    public static readonly StyledProperty<VisualizationCore?> CoreProperty
        = AvaloniaProperty.Register<VisualizationSurfaceControl, VisualizationCore?>(nameof(Core));

    public static readonly StyledProperty<double> LeftChromeWidthProperty
        = AvaloniaProperty.Register<VisualizationSurfaceControl, double>(nameof(LeftChromeWidth));

    public static readonly StyledProperty<bool> IsRenderingEnabledProperty
        = AvaloniaProperty.Register<VisualizationSurfaceControl, bool>(nameof(IsRenderingEnabled), true);

    public static readonly StyledProperty<double> TopChromeHeightProperty
        = AvaloniaProperty.Register<VisualizationSurfaceControl, double>(nameof(TopChromeHeight));

    #endregion AVALONIA PROPERTIES

    #region PROPERTIES

    public double BottomChromeHeight
    {
        get => GetValue(BottomChromeHeightProperty);
        set => SetValue(BottomChromeHeightProperty, value);
    }

    public VisualizationCore? Core
    {
        get => GetValue(CoreProperty);
        set => SetValue(CoreProperty, value);
    }

    public double LeftChromeWidth
    {
        get => GetValue(LeftChromeWidthProperty);
        set => SetValue(LeftChromeWidthProperty, value);
    }

    public bool IsRenderingEnabled
    {
        get => GetValue(IsRenderingEnabledProperty);
        set => SetValue(IsRenderingEnabledProperty, value);
    }

    public double TopChromeHeight
    {
        get => GetValue(TopChromeHeightProperty);
        set => SetValue(TopChromeHeightProperty, value);
    }

    #endregion PROPERTIES

    #region CONSTRUCTOR

    public VisualizationSurfaceControl()
    {
        _renderTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(1000d / 60)
        };

        _renderTimer.Tick += RenderTimer_Tick;

        InitializeComponent();
    }

    #endregion CONSTRUCTOR

    #region VISUAL TREE

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _renderTimer.Start();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _renderTimer.Stop();
        base.OnDetachedFromVisualTree(e);
    }

    #endregion VISUAL TREE

    #region VISUALIZATION RENDER

    public override void Render(DrawingContext context)
    {
        _isFramePending = false;
        base.Render(context);

        if (!IsRenderingEnabled || Core is null)
            return;

        var topInset = Core.FillScreen ? 0 : Math.Max(0, TopChromeHeight - Dimensions.AutoHideRevealSize);
        var bottomInset = Core.FillScreen ? 0 : Math.Max(0, BottomChromeHeight - Dimensions.AutoHideRevealSize);
        var leftInset = Core.FillScreen ? 0 : Math.Clamp(LeftChromeWidth, 0, Bounds.Width);
        var drawingArea = new Rect(
            leftInset,
            topInset,
            Math.Max(0, Bounds.Width - leftInset),
            Math.Max(0, Bounds.Height - topInset - bottomInset));
        var renderScaling = TopLevel.GetTopLevel(this)?.RenderScaling ?? 1;

        Core.UpdateGraphics(drawingArea, renderScaling);
        Core.Draw(context, Foreground ?? Brushes.DodgerBlue);
    }

    private void RenderTimer_Tick(object? sender, EventArgs e)
    {
        if (!IsRenderingEnabled)
            return;

        if (Core is not null && _renderTimer.Interval != Core.FrameInterval)
            _renderTimer.Interval = Core.FrameInterval;

        if (_isFramePending || !IsEffectivelyVisible)
            return;

        _isFramePending = true;
        InvalidateVisual();
    }

    #endregion VISUALIZATION RENDER
}
