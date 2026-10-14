using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace VisualPlayer.Controls;

[TemplatePart("PART_Button", typeof(Button), IsRequired = true)]
public class HamburgerMenuItemControl : TemplatedControl
{
    private Button? _button;
    
    #region EVENTS

    public static readonly RoutedEvent<RoutedEventArgs> InvokedEvent 
        = RoutedEvent.Register<HamburgerMenuItemControl, RoutedEventArgs>(nameof(Invoked), RoutingStrategies.Bubble);

    public event EventHandler<RoutedEventArgs>? Invoked
    {
        add => AddHandler(InvokedEvent, value);
        remove => RemoveHandler(InvokedEvent, value);
    }

    #endregion EVENTS

    #region AVALONIA PROPERTIES

    public static readonly StyledProperty<bool> IsMenuOpenProperty
        = AvaloniaProperty.Register<HamburgerMenuItemControl, bool>(nameof(IsMenuOpen));

    #endregion AVALONIA PROPERTIES

    #region PROPERTIES

    public bool IsMenuOpen
    {
        get => GetValue(IsMenuOpenProperty);
        set => SetValue(IsMenuOpenProperty, value);
    }

    #endregion PROPERTIES

    #region ACTIONS

    private void OnButtonClick(object? sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(InvokedEvent));
    }

    #endregion ACTIONS

    #region TEMPLATE

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (_button is not null)
            _button.Click -= OnButtonClick;

        base.OnApplyTemplate(e);

        _button = e.NameScope.Find<Button>("PART_Button");

        if (_button is not null)
            _button.Click += OnButtonClick;
    }

    #endregion TEMPLATE
}