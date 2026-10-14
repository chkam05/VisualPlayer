using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;
using Lucide.Avalonia;
using VisualPlayer.Models.Config;
using VisualPlayer.Utils;
using VisualPlayer.Views.Base;
using VisualPlayer.Contexts;

namespace VisualPlayer.Views.Dialogs;

[TemplatePart("PART_CancelButton", typeof(Button), IsRequired = true)]
[TemplatePart("PART_OkButton", typeof(Button), IsRequired = true)]
public class InternalMessageColorPicker : InternalMessageBase
{
    private static TranslationContext T => ApplicationContext.Instance.Translations;

    #region CONST

    public const int COLOR_CHANNEL_MAXIMUM = byte.MaxValue;
    public const int COLOR_CHANNEL_MINIMUM = byte.MinValue;
    public const double COLOR_VALUE_MAXIMUM = byte.MaxValue;
    public const double COLOR_VALUE_MINIMUM = byte.MinValue;
    public const int HSL_PERCENTAGE_MAXIMUM = 100;
    public const int HSL_PERCENTAGE_MINIMUM = 0;
    public const int HUE_MAXIMUM = 360;
    public const int HUE_MINIMUM = 0;

    #endregion CONST

    private Button? _cancelButton;
    private bool _isSynchronizing;
    private Button? _okButton;

    private readonly TaskCompletionSource<AccentColor?> _resultSource
        = new(TaskCreationOptions.RunContinuationsAsynchronously);

    #region AVALONIA PROPERTIES

    public static readonly StyledProperty<int> AlphaProperty
        = AvaloniaProperty.Register<InternalMessageColorPicker, int>(nameof(Alpha), byte.MaxValue);

    public static readonly StyledProperty<int> BlueProperty
        = AvaloniaProperty.Register<InternalMessageColorPicker, int>(nameof(Blue));

    public static readonly StyledProperty<double> ColorValueProperty
        = AvaloniaProperty.Register<InternalMessageColorPicker, double>(nameof(ColorValue), (double)byte.MaxValue);

    public static readonly StyledProperty<bool> EnableCloseButtonProperty
        = AvaloniaProperty.Register<InternalMessageColorPicker, bool>(nameof(EnableCloseButton), true);

    public static readonly StyledProperty<int> GreenProperty
        = AvaloniaProperty.Register<InternalMessageColorPicker, int>(nameof(Green));

    public static readonly StyledProperty<string> HexCodeProperty
        = AvaloniaProperty.Register<InternalMessageColorPicker, string>(nameof(HexCode), "#FF0078D7");

    public static readonly StyledProperty<int> HueProperty
        = AvaloniaProperty.Register<InternalMessageColorPicker, int>(nameof(Hue));

    public static readonly StyledProperty<LucideIconKind> IconProperty
        = AvaloniaProperty.Register<InternalMessageColorPicker, LucideIconKind>(nameof(Icon), LucideIconKind.Palette);

    public static readonly StyledProperty<int> LightnessProperty
        = AvaloniaProperty.Register<InternalMessageColorPicker, int>(nameof(Lightness), 50);

    public static readonly StyledProperty<int> PickerHueProperty
        = AvaloniaProperty.Register<InternalMessageColorPicker, int>(nameof(PickerHue));

    public static readonly StyledProperty<int> PickerSaturationProperty
        = AvaloniaProperty.Register<InternalMessageColorPicker, int>(nameof(PickerSaturation), 100);

    public static readonly StyledProperty<IBrush> PreviewBrushProperty
        = AvaloniaProperty.Register<InternalMessageColorPicker, IBrush>(nameof(PreviewBrush), Brushes.Transparent);

    public static readonly StyledProperty<int> RedProperty
        = AvaloniaProperty.Register<InternalMessageColorPicker, int>(nameof(Red));

    public static readonly StyledProperty<int> SaturationProperty
        = AvaloniaProperty.Register<InternalMessageColorPicker, int>(nameof(Saturation), 100);

    public static readonly StyledProperty<IBrush> ValueGradientBrushProperty
        = AvaloniaProperty.Register<InternalMessageColorPicker, IBrush>(nameof(ValueGradientBrush), Brushes.Transparent);

    #endregion AVALONIA PROPERTIES

    #region PROPERTIES

    public int Alpha
    {
        get => GetValue(AlphaProperty);
        set => SetValue(AlphaProperty, Math.Clamp(value, COLOR_CHANNEL_MINIMUM, COLOR_CHANNEL_MAXIMUM));
    }

    public int Blue
    {
        get => GetValue(BlueProperty);
        set => SetValue(BlueProperty, Math.Clamp(value, COLOR_CHANNEL_MINIMUM, COLOR_CHANNEL_MAXIMUM));
    }

    public double ColorValue
    {
        get => GetValue(ColorValueProperty);
        set => SetValue(ColorValueProperty, Math.Clamp(value, COLOR_CHANNEL_MINIMUM, COLOR_CHANNEL_MAXIMUM));
    }

    private Color CurrentColor
    {
        get => Color.FromArgb((byte)Alpha, (byte)Red, (byte)Green, (byte)Blue);
    }

    public bool EnableCloseButton
    {
        get => GetValue(EnableCloseButtonProperty);
        set => SetValue(EnableCloseButtonProperty, value);
    }
    public int Green
    {
        get => GetValue(GreenProperty);
        set => SetValue(GreenProperty, Math.Clamp(value, COLOR_CHANNEL_MINIMUM, COLOR_CHANNEL_MAXIMUM));
    }

    public string HexCode
    {
        get => GetValue(HexCodeProperty);
        set => SetValue(HexCodeProperty, value);
    }

    public int Hue
    {
        get => GetValue(HueProperty);
        set => SetValue(HueProperty, Math.Clamp(value, HUE_MINIMUM, HUE_MAXIMUM));
    }
    
    public LucideIconKind Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
    
    public int Lightness
    {
        get => GetValue(LightnessProperty);
        set => SetValue(LightnessProperty, Math.Clamp(value, HSL_PERCENTAGE_MINIMUM, HSL_PERCENTAGE_MAXIMUM));
    }
    
    public IBrush PreviewBrush
    {
        get => GetValue(PreviewBrushProperty);
        private set => SetValue(PreviewBrushProperty, value);
    }

    public int PickerHue
    {
        get => GetValue(PickerHueProperty);
        set => SetValue(PickerHueProperty, Math.Clamp(value, HUE_MINIMUM, HUE_MAXIMUM));
    }

    public int PickerSaturation
    {
        get => GetValue(PickerSaturationProperty);
        set => SetValue(PickerSaturationProperty, Math.Clamp(value, HSL_PERCENTAGE_MINIMUM, HSL_PERCENTAGE_MAXIMUM));
    }
    
    public int Red
    {
        get => GetValue(RedProperty);
        set => SetValue(RedProperty, Math.Clamp(value, COLOR_CHANNEL_MINIMUM, COLOR_CHANNEL_MAXIMUM));
    }
    
    public int Saturation
    {
        get => GetValue(SaturationProperty);
        set => SetValue(SaturationProperty, Math.Clamp(value, HSL_PERCENTAGE_MINIMUM, HSL_PERCENTAGE_MAXIMUM));
    }

    public IBrush ValueGradientBrush
    {
        get => GetValue(ValueGradientBrushProperty);
        private set => SetValue(ValueGradientBrushProperty, value);
    }

    public AccentColor SelectedColor
    {
        get => new(CurrentColor, string.Empty);
    }

    public Task<AccentColor?> ResultTask
    {
        get => _resultSource.Task;
    }


    protected override Type StyleKeyOverride => typeof(InternalMessageColorPicker);

    #endregion PROPERTIES

    #region CONSTRUCTORS

    public InternalMessageColorPicker()
        : this(new AccentColor("#0078D7", T["DIALOG_COLOR_PICKER_CUSTOM_COLOR"]))
    {
        //
    }

    public InternalMessageColorPicker(AccentColor initialColor)
    {
        Title = T["DIALOG_COLOR_PICKER_TITLE"];
        SetFromColor(initialColor.ToColor());
    }

    #endregion CONSTRUCTORS

    #region ACTIONS

    protected override void OnClosed()
    {
        _resultSource.TrySetResult(null);
    }

    private void SetFromColor(Color color, bool updatePicker = true)
    {
        _isSynchronizing = true;

        try
        {
            var ahsl = AHSLColor.FromColor(color);

            SetCurrentValue(AlphaProperty, (int)color.A);
            SetCurrentValue(RedProperty, (int)color.R);
            SetCurrentValue(GreenProperty, (int)color.G);
            SetCurrentValue(BlueProperty, (int)color.B);
            SetCurrentValue(HueProperty, (int)Math.Round(ahsl.H / (double)AHSLColor.HUE_MAX * 360));
            SetCurrentValue(SaturationProperty, ahsl.S);
            SetCurrentValue(LightnessProperty, ahsl.L);

            if (updatePicker)
            {
                GetHsv(color, out var pickerHue, out var pickerSaturation, out var colorValue);
                SetCurrentValue(PickerHueProperty, pickerHue);
                SetCurrentValue(PickerSaturationProperty, pickerSaturation);
                SetCurrentValue(ColorValueProperty, colorValue);
            }
            SetCurrentValue(HexCodeProperty, color.A == byte.MaxValue
                ? $"#{color.R:X2}{color.G:X2}{color.B:X2}"
                : $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}");

            PreviewBrush = new SolidColorBrush(color);
            UpdateValueGradient();
        }
        finally
        {
            _isSynchronizing = false;
        }
    }

    #endregion ACTIONS

    #region BUTTON ACTIONS

    private void CancelButton_Click(object? sender, RoutedEventArgs e)
    {
        _resultSource.TrySetResult(null);
        RequestClose();
    }

    private void OkButton_Click(object? sender, RoutedEventArgs e)
    {
        _resultSource.TrySetResult(SelectedColor);
        RequestClose();
    }

    #endregion BUTTON ACTIONS

    #region PROPERTY CHANGES

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (_isSynchronizing)
            return;

        if (change.Property == AlphaProperty
            || change.Property == RedProperty
            || change.Property == GreenProperty
            || change.Property == BlueProperty)
        {
            SetFromColor(CurrentColor);
            return;
        }

        if (change.Property == HueProperty
            || change.Property == SaturationProperty
            || change.Property == LightnessProperty)
        {
            var ahslHue = (int)Math.Round(Hue / 360d * AHSLColor.HUE_MAX);
            SetFromColor(new AHSLColor((byte)Alpha, ahslHue, Saturation, Lightness).ToColor());
            return;
        }

        if (change.Property == PickerHueProperty
            || change.Property == PickerSaturationProperty
            || change.Property == ColorValueProperty)
        {
            SetFromColor(
                ColorFromHsv(PickerHue, PickerSaturation, (int)Math.Round(ColorValue), (byte)Alpha),
                updatePicker: false);
            return;
        }

        if (change.Property == HexCodeProperty
            && (HexCode.Length == 7 || HexCode.Length == 9)
            && Color.TryParse(HexCode, out var color))
        {
            SetFromColor(color);
        }
    }

    #endregion PROPERTY CHANGES

    #region COLOR CONVERSION

    private static Color ColorFromHsv(int hue, int saturation, int value, byte alpha)
    {
        var h = Math.Clamp(hue, 0, 360) / 60d;
        var s = Math.Clamp(saturation, 0, 100) / 100d;
        var v = Math.Clamp(value, COLOR_CHANNEL_MINIMUM, COLOR_CHANNEL_MAXIMUM) / (double)COLOR_CHANNEL_MAXIMUM;
        var chroma = v * s;
        var x = chroma * (1 - Math.Abs(h % 2 - 1));
        var offset = v - chroma;

        var (r, g, b) = h switch
        {
            < 1 => (chroma, x, 0d),
            < 2 => (x, chroma, 0d),
            < 3 => (0d, chroma, x),
            < 4 => (0d, x, chroma),
            < 5 => (x, 0d, chroma),
            < 6 => (chroma, 0d, x),
            _ => (chroma, 0d, 0d)
        };

        return Color.FromArgb(
            alpha,
            (byte)Math.Round((r + offset) * 255),
            (byte)Math.Round((g + offset) * 255),
            (byte)Math.Round((b + offset) * 255));
    }

    private static void GetHsv(Color color, out int hue, out int saturation, out int value)
    {
        var r = color.R / 255d;
        var g = color.G / 255d;
        var b = color.B / 255d;
        var maximum = Math.Max(r, Math.Max(g, b));
        var minimum = Math.Min(r, Math.Min(g, b));
        var delta = maximum - minimum;
        double calculatedHue;

        if (delta == 0)
            calculatedHue = 0;
        else if (maximum == r)
            calculatedHue = 60 * (((g - b) / delta) % 6);
        else if (maximum == g)
            calculatedHue = 60 * ((b - r) / delta + 2);
        else
            calculatedHue = 60 * ((r - g) / delta + 4);

        if (calculatedHue < 0)
            calculatedHue += 360;

        hue = (int)Math.Round(calculatedHue);
        saturation = maximum == 0 ? 0 : (int)Math.Round(delta / maximum * 100);
        value = (int)Math.Round(maximum * 255);
    }

    private void UpdateValueGradient()
    {
        var fullValueColor = ColorFromHsv(PickerHue, PickerSaturation, byte.MaxValue, byte.MaxValue);

        ValueGradientBrush = new LinearGradientBrush
        {
            StartPoint = new RelativePoint(0.5, 0, RelativeUnit.Relative),
            EndPoint = new RelativePoint(0.5, 1, RelativeUnit.Relative),
            GradientStops =
            {
                new GradientStop(fullValueColor, 0),
                new GradientStop(Colors.Black, 1)
            }
        };
    }

    #endregion COLOR CONVERSION

    #region TEMPLATE

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        DetachHandlers();
        base.OnApplyTemplate(e);

        _cancelButton = e.NameScope.Find<Button>("PART_CancelButton");
        _okButton = e.NameScope.Find<Button>("PART_OkButton");

        if (_cancelButton is not null)
            _cancelButton.Click += CancelButton_Click;

        if (_okButton is not null)
            _okButton.Click += OkButton_Click;
    }

    private void DetachHandlers()
    {
        if (_cancelButton is not null)
            _cancelButton.Click -= CancelButton_Click;

        if (_okButton is not null)
            _okButton.Click -= OkButton_Click;

        _cancelButton = null;
        _okButton = null;
    }

    #endregion TEMPLATE
}
