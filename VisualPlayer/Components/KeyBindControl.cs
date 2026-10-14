using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using VisualPlayer.Utils;

namespace VisualPlayer.Components;

[TemplatePart("PART_InputButton", typeof(Button), IsRequired = true)]
[TemplatePart("PART_ClearButton", typeof(Button), IsRequired = true)]
public class KeyBindControl : TemplatedControl
{
    private Button? _inputButton;
    private Button? _clearButton;

    #region AVALONIA PROPERTIES

    public static readonly StyledProperty<string?> GestureProperty
        = AvaloniaProperty.Register<KeyBindControl, string?>(nameof(Gesture), defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

    #endregion AVALONIA PROPERTIES

    #region PROPERTIES

    public string? Gesture
    {
        get => GetValue(GestureProperty);
        set => SetValue(GestureProperty, value);
    }

    #endregion PROPERTIES

    #region ACTIONS

    private void ClearButton_Click(object? sender, RoutedEventArgs e)
    {
        SetCurrentValue(GestureProperty, null);
        PseudoClasses.Set(":capturing", false);
        e.Handled = true;
    }

    private void InputButton_Click(object? sender, RoutedEventArgs e)
    {
        Focus();
        PseudoClasses.Set(":capturing", true);
        e.Handled = true;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (e.Key == Key.Escape)
        {
            PseudoClasses.Set(":capturing", false);
            e.Handled = true;
            return;
        }

        var gesture = KeyboardShortcutUtils.FromKeyEvent(e);
        if (gesture is null)
        {
            e.Handled = true;
            return;
        }

        SetCurrentValue(GestureProperty, gesture);
        PseudoClasses.Set(":capturing", false);
        e.Handled = true;
    }

    #endregion ACTIONS

    #region TEMPLATE

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        DetachHandlers();
        base.OnApplyTemplate(e);

        _inputButton = e.NameScope.Find<Button>("PART_InputButton");
        _clearButton = e.NameScope.Find<Button>("PART_ClearButton");

        if (_inputButton is not null)
            _inputButton.Click += InputButton_Click;
        if (_clearButton is not null)
            _clearButton.Click += ClearButton_Click;
    }

    private void DetachHandlers()
    {
        if (_inputButton is not null)
            _inputButton.Click -= InputButton_Click;
        if (_clearButton is not null)
            _clearButton.Click -= ClearButton_Click;
    }

    #endregion TEMPLATE
}
