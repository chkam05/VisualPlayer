using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace VisualPlayer.Controls;

[TemplatePart("PART_TopItemsControl", typeof(ItemsControl), IsRequired = true)]
[TemplatePart("PART_BottomItemsControl", typeof(ItemsControl), IsRequired = true)]
public class HamburgerMenuControl : TemplatedControl
{
    private TopLevel? _topLevel;

    #region AVALONIA PROPERTIES

    public static readonly StyledProperty<bool> IsOpenProperty
        = AvaloniaProperty.Register<HamburgerMenuControl, bool>(nameof(IsOpen), defaultValue: false, defaultBindingMode: BindingMode.TwoWay);

    #endregion AVALONIA PROPERTIES

    #region PROPERTIES

    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    #endregion PROPERTIES

    #region CONSTRUCTOR

    public HamburgerMenuControl()
    {
        // Visibility transitions are coordinated by SidePanelControl.
    }

    #endregion CONSTRUCTOR

    #region ACTIONS

    private void TopLevel_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!IsOpen || Bounds.Contains(e.GetPosition(this)))
            return;

        SetCurrentValue(IsOpenProperty, false);
    }

    #endregion ACTIONS

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
