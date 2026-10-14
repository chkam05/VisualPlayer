using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace VisualPlayer.Components;

[TemplatePart("PART_DecreaseButton", typeof(Button), IsRequired = true)]
[TemplatePart("PART_IncreaseButton", typeof(Button), IsRequired = true)]
[TemplatePart("PART_Input", typeof(TextBox), IsRequired = true)]
public class NumericUpDown : TemplatedControl
{
    private Button? _decreaseButton;
    private Button? _increaseButton;
    private TextBox? _input;
    private bool _isNormalizingValue;

    #region AVALONIA PROPERTIES

    public static readonly StyledProperty<int> IncrementProperty
        = AvaloniaProperty.Register<NumericUpDown, int>(nameof(Increment), 1);

    public static readonly StyledProperty<int> MaximumProperty
        = AvaloniaProperty.Register<NumericUpDown, int>(nameof(Maximum), 100);

    public static readonly StyledProperty<int> MinimumProperty
        = AvaloniaProperty.Register<NumericUpDown, int>(nameof(Minimum));

    public static readonly StyledProperty<string> SuffixProperty
        = AvaloniaProperty.Register<NumericUpDown, string>(nameof(Suffix), string.Empty);

    public static readonly StyledProperty<int> ValueProperty
        = AvaloniaProperty.Register<NumericUpDown, int>(nameof(Value), defaultBindingMode: BindingMode.TwoWay);

    #endregion AVALONIA PROPERTIES

    #region PROPERTIES

    public int Increment
    {
        get => GetValue(IncrementProperty);
        set => SetValue(IncrementProperty, value);
    }

    public int Maximum
    {
        get => GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public int Minimum
    {
        get => GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    public string Suffix
    {
        get => GetValue(SuffixProperty);
        set => SetValue(SuffixProperty, value);
    }

    public int Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    protected override Type StyleKeyOverride => typeof(NumericUpDown);

    #endregion PROPERTIES

    #region ACTIONS

    private void DecreaseButton_Click(object? sender, RoutedEventArgs e)
    {
        ChangeValue(-GetEffectiveIncrement());
    }

    private void IncreaseButton_Click(object? sender, RoutedEventArgs e)
    {
        ChangeValue(GetEffectiveIncrement());
    }

    private void Input_KeyDown(object? sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Enter:
                CommitInput();
                e.Handled = true;
                break;
            case Key.Up:
                ChangeValue(GetEffectiveIncrement());
                e.Handled = true;
                break;
            case Key.Down:
                ChangeValue(-GetEffectiveIncrement());
                e.Handled = true;
                break;
        }
    }

    private void Input_LostFocus(object? sender, RoutedEventArgs e)
    {
        CommitInput();
    }

    #endregion ACTIONS

    #region PROPERTY CHANGE HANDLING

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (_isNormalizingValue)
            return;

        if (change.Property == ValueProperty
            || change.Property == MinimumProperty
            || change.Property == MaximumProperty)
        {
            NormalizeValue();
            UpdateInput();
            UpdateButtons();
            return;
        }

        if (change.Property == IncrementProperty)
        {
            UpdateButtons();
            return;
        }

        if (change.Property == IsEnabledProperty)
        {
            UpdateButtons();
            return;
        }

        if (change.Property == SuffixProperty)
            UpdateInput();
    }

    #endregion PROPERTY CHANGE HANDLING

    #region TEMPLATE

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        DetachHandlers();
        base.OnApplyTemplate(e);

        _decreaseButton = e.NameScope.Find<Button>("PART_DecreaseButton");
        _increaseButton = e.NameScope.Find<Button>("PART_IncreaseButton");
        _input = e.NameScope.Find<TextBox>("PART_Input");

        if (_decreaseButton is not null)
            _decreaseButton.Click += DecreaseButton_Click;

        if (_increaseButton is not null)
            _increaseButton.Click += IncreaseButton_Click;

        if (_input is not null)
        {
            _input.KeyDown += Input_KeyDown;
            _input.LostFocus += Input_LostFocus;
        }

        NormalizeValue();
        UpdateInput();
        UpdateButtons();
    }

    private void DetachHandlers()
    {
        if (_decreaseButton is not null)
            _decreaseButton.Click -= DecreaseButton_Click;

        if (_increaseButton is not null)
            _increaseButton.Click -= IncreaseButton_Click;

        if (_input is not null)
        {
            _input.KeyDown -= Input_KeyDown;
            _input.LostFocus -= Input_LostFocus;
        }

        _decreaseButton = null;
        _increaseButton = null;
        _input = null;
    }

    #endregion TEMPLATE

    #region UTILITIES

    private void ChangeValue(int difference)
    {
        var value = (long)Value + difference;
        SetCurrentValue(ValueProperty, (int)Math.Clamp(value, GetMinimum(), GetMaximum()));
    }

    private void CommitInput()
    {
        if (_input is null)
            return;

        var text = _input.Text?.Trim() ?? string.Empty;

        if (!string.IsNullOrEmpty(Suffix)
            && text.EndsWith(Suffix, StringComparison.CurrentCulture))
        {
            text = text[..^Suffix.Length].Trim();
        }

        if (int.TryParse(text, NumberStyles.Integer, CultureInfo.CurrentCulture, out var value))
            SetCurrentValue(ValueProperty, Math.Clamp(value, GetMinimum(), GetMaximum()));

        UpdateInput();
    }

    private int GetEffectiveIncrement()
    {
        return Math.Max(1, Math.Abs(Increment));
    }

    private int GetMaximum()
    {
        return Math.Max(Minimum, Maximum);
    }

    private int GetMinimum()
    {
        return Math.Min(Minimum, Maximum);
    }

    private void NormalizeValue()
    {
        var value = Math.Clamp(Value, GetMinimum(), GetMaximum());

        if (value == Value)
            return;

        _isNormalizingValue = true;

        try
        {
            SetCurrentValue(ValueProperty, value);
        }
        finally
        {
            _isNormalizingValue = false;
        }
    }

    private void UpdateButtons()
    {
        if (_decreaseButton is not null)
            _decreaseButton.IsEnabled = IsEnabled && Value > GetMinimum();

        if (_increaseButton is not null)
            _increaseButton.IsEnabled = IsEnabled && Value < GetMaximum();
    }

    private void UpdateInput()
    {
        if (_input is not null)
            _input.Text = $"{Value}{Suffix}";
    }

    #endregion UTILITIES
}
