using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Media;
using VisualPlayer.Utils;

namespace VisualPlayer.Components;

public class ColorPickerAreaControl : Control
{
    private bool _isDragging;

    #region AVALONIA PROPERTIES

    public static readonly StyledProperty<int> HueProperty
        = AvaloniaProperty.Register<ColorPickerAreaControl, int>(nameof(Hue), defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<int> SaturationProperty
        = AvaloniaProperty.Register<ColorPickerAreaControl, int>(nameof(Saturation), 100, defaultBindingMode: BindingMode.TwoWay);

    #endregion AVALONIA PROPERTIES

    #region PROPERTIES

    public int Hue
    {
        get => GetValue(HueProperty);
        set => SetValue(HueProperty, Math.Clamp(value, 0, 360));
    }

    public int Saturation
    {
        get => GetValue(SaturationProperty);
        set => SetValue(SaturationProperty, Math.Clamp(value, AHSLColor.SATURATION_MIN, AHSLColor.SATURATION_MAX));
    }

    #endregion PROPERTIES

    #region ACTIONS

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            return;

        _isDragging = true;
        e.Pointer.Capture(this);
        UpdateSelection(e.GetPosition(this));
        e.Handled = true;
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);

        if (_isDragging)
            UpdateSelection(e.GetPosition(this));
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        if (!_isDragging)
            return;

        UpdateSelection(e.GetPosition(this));
        _isDragging = false;
        e.Pointer.Capture(null);
        e.Handled = true;
    }

    private void UpdateSelection(Point point)
    {
        if (Bounds.Width <= 0 || Bounds.Height <= 0)
            return;

        Hue = (int)Math.Round(Math.Clamp(point.X / Bounds.Width, 0, 1) * 360);
        Saturation = (int)Math.Round((1 - Math.Clamp(point.Y / Bounds.Height, 0, 1)) * 100);
    }

    #endregion ACTIONS

    #region PROPERTY CHANGES
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == HueProperty
            || change.Property == SaturationProperty)
        {
            InvalidateVisual();
        }
    }

    #endregion PROPERTY CHANGES

    #region RENDER

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var bounds = new Rect(Bounds.Size);

        if (bounds.Width <= 0 || bounds.Height <= 0)
            return;

        var rainbow = new LinearGradientBrush
        {
            StartPoint = new RelativePoint(0, 0.5, RelativeUnit.Relative),
            EndPoint = new RelativePoint(1, 0.5, RelativeUnit.Relative),
            GradientStops =
            {
                new GradientStop(Colors.Red, 0),
                new GradientStop(Colors.Yellow, 1d / 6),
                new GradientStop(Colors.Lime, 2d / 6),
                new GradientStop(Colors.Cyan, 3d / 6),
                new GradientStop(Colors.Blue, 4d / 6),
                new GradientStop(Colors.Magenta, 5d / 6),
                new GradientStop(Colors.Red, 1)
            }
        };

        context.DrawRectangle(rainbow, null, bounds, 6, 6);

        var saturationOverlay = new LinearGradientBrush
        {
            StartPoint = new RelativePoint(0.5, 0, RelativeUnit.Relative),
            EndPoint = new RelativePoint(0.5, 1, RelativeUnit.Relative),
            GradientStops =
            {
                new GradientStop(Colors.Transparent, 0),
                new GradientStop(Colors.White, 1)
            }
        };

        context.DrawRectangle(saturationOverlay, null, bounds, 6, 6);

        var x = Hue / 360d * bounds.Width;
        var y = (1 - Saturation / 100d) * bounds.Height;
        var marker = new Point(Math.Clamp(x, 0, bounds.Width), Math.Clamp(y, 0, bounds.Height));

        context.DrawEllipse(Brushes.Transparent, new Pen(Brushes.Black, 3), marker, 7, 7);
        context.DrawEllipse(Brushes.Transparent, new Pen(Brushes.White, 1), marker, 7, 7);
    }
    
    #endregion RENDER
}
