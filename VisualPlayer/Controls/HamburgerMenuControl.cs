using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using VisualPlayer.Resources;

namespace VisualPlayer.Controls;

[TemplatePart("PART_TopItemsControl", typeof(ItemsControl), IsRequired = true)]
[TemplatePart("PART_BottomItemsControl", typeof(ItemsControl), IsRequired = true)]
public class HamburgerMenuControl : TemplatedControl
{
    private int _contentVisibilityVersion;
    private bool _isContentVisible;
    private TopLevel? _topLevel;

    #region AVALONIA PROPERTIES

    public static readonly StyledProperty<bool> IsOpenProperty
        = AvaloniaProperty.Register<HamburgerMenuControl, bool>(nameof(IsOpen), defaultValue: false, defaultBindingMode: BindingMode.TwoWay);

    public static readonly DirectProperty<HamburgerMenuControl, bool> IsContentVisibleProperty
        = AvaloniaProperty.RegisterDirect<HamburgerMenuControl, bool>(nameof(IsContentVisible), control => control.IsContentVisible);

    #endregion AVALONIA PROPERTIES

    #region PROPERTIES

    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    public bool IsContentVisible
    {
        get => _isContentVisible;
        private set => SetAndRaise(IsContentVisibleProperty, ref _isContentVisible, value);
    }

    #endregion PROPERTIES

    #region CONSTRUCTOR

    public HamburgerMenuControl()
    {
        // Visibility transitions are coordinated by SidePanelControl.
    }

    #endregion CONSTRUCTOR

    #region ACTIONS

    private async void UpdateContentVisibility(bool isOpen)
    {
        var version = ++_contentVisibilityVersion;

        if (isOpen)
        {
            IsContentVisible = true;
            return;
        }

        await Task.Delay(Dimensions.SidePanelAnimationDuration);

        if (version == _contentVisibilityVersion && !IsOpen)
            IsContentVisible = false;
    }

    private void TopLevel_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!IsOpen || Bounds.Contains(e.GetPosition(this)))
            return;

        SetCurrentValue(IsOpenProperty, false);
    }

    #endregion ACTIONS

    #region PROPERTY CHANGES

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsOpenProperty)
            UpdateContentVisibility(change.GetNewValue<bool>());
    }

    #endregion PROPERTY CHANGES

    #region TEMPLATE

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
    }

    #endregion TEMPLATE

    #region VISUAL TREE

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _topLevel = TopLevel.GetTopLevel(this);
        _topLevel?.AddHandler(PointerPressedEvent, TopLevel_PointerPressed, RoutingStrategies.Tunnel);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        if (_topLevel is not null)
            _topLevel.RemoveHandler(PointerPressedEvent, TopLevel_PointerPressed);

        _topLevel = null;
        base.OnDetachedFromVisualTree(e);
    }

    #endregion VISUAL TREE
}
