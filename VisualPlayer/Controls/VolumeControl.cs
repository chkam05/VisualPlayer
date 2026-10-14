using System;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Threading;
using VisualPlayer.Resources;

namespace VisualPlayer.Controls;

public class VolumeControl : TemplatedControl
{
    #region CONST

    public const double OPEN_HEIGHT = 256;
    public const double WIDTH = 64;

    #endregion CONST

    private bool _hasPointerEntered;

    #region AVALONIA PROPERTIES

    public static readonly StyledProperty<bool> IsOpenProperty
        = AvaloniaProperty.Register<VolumeControl, bool>(nameof(IsOpen));

    #endregion AVALONIA PROPERTIES

    #region PROPERTIES

    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    #endregion PROPERTIES

    #region CONSTRUCTORS

    public VolumeControl()
    {
        ClipToBounds = true;
        Height = 0;
        IsHitTestVisible = false;

        Transitions =
        [
            new DoubleTransition
            {
                Property = HeightProperty,
                Duration = Dimensions.SidePanelAnimationDuration
            }
        ];

        PointerEntered += VolumeControl_PointerEntered;
        PointerExited += VolumeControl_PointerExited;
    }

    #endregion CONSTRUCTORS

    #region ACTIONS

    public void Close()
    {
        IsOpen = false;
    }

    public void Toggle()
    {
        IsOpen = !IsOpen;
    }

    #endregion ACTIONS

    #region POINTER ACTIONS

    private void VolumeControl_PointerEntered(object? sender, PointerEventArgs e)
    {
        if (IsOpen)
            _hasPointerEntered = true;
    }

    private void VolumeControl_PointerExited(object? sender, PointerEventArgs e)
    {
        if (!_hasPointerEntered
            || e.Pointer.Captured is not null
            || Contains(e.GetPosition(this)))
        {
            return;
        }

        Close();
    }

    public bool Contains(Point point)
    {
        return new Rect(Bounds.Size).Contains(point);
    }

    #endregion POINTER ACTIONS

    #region PROPERTY CHANGE HANDLING

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property != IsOpenProperty)
            return;

        if (IsOpen)
        {
            _hasPointerEntered = false;
            IsHitTestVisible = true;
            Dispatcher.UIThread.Post(() => Height = OPEN_HEIGHT, DispatcherPriority.Render);
        }
        else
        {
            Height = 0;
            _hasPointerEntered = false;
            Dispatcher.UIThread.Post(
                () =>
                {
                    if (!IsOpen)
                        IsHitTestVisible = false;
                },
                DispatcherPriority.Background);
        }
    }

    #endregion PROPERTY CHANGE HANDLING
}
