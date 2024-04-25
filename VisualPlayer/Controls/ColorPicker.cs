using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using VisualPlayer.Controls.Events;
using VisualPlayer.Data.ColorModels;
using static VisualPlayer.Controls.ColorSelector;

namespace VisualPlayer.Controls
{
    public class ColorPicker : Control
    {

        //  DEPENDENCY PROPERTIES

        public static readonly DependencyProperty ColorMainProperty = DependencyProperty.Register(
            nameof(ColorMain),
            typeof(Color),
            typeof(ColorPicker),
            new PropertyMetadata(Colors.Red, MainColorPropertyChangedCallback));

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            nameof(CornerRadius),
            typeof(CornerRadius),
            typeof(ColorPicker),
            new PropertyMetadata(new CornerRadius(8)));

        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register(
            nameof(SelectedColor),
            typeof(ThemeColor),
            typeof(ColorPicker),
            new PropertyMetadata(new ThemeColor(Colors.Red, "Red"), SelectedColorPropertyChangedCallback));


        //  EVENTS

        public event ColorSelectionChangedEventHandler SelectionChanged;


        //  VARIABLES

        private bool _isColorChanging = false;
        private bool _colorSliderManualChange = false;
        private bool _intensitySliderManualChange = false;
        private bool _lightnessSliderManualChange = false;

        private Slider _colorSlider;
        private Slider _intensitySlider;
        private Slider _lightnessSlider;

        private Grid _canvasGrid;
        private Ellipse _visualSelectorEllipse;


        //  GETTERS & SETTERS

        private bool IsUserChanging
        {
            get => _colorSliderManualChange || _intensitySliderManualChange || _lightnessSliderManualChange;
        }

        public Color ColorMain
        {
            get => (Color)GetValue(ColorMainProperty);
            set => SetValue(ColorMainProperty, value);
        }

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public ThemeColor SelectedColor
        {
            get => (ThemeColor)GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Static ColorPicker class constructor. </summary>
        static ColorPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPicker),
                new FrameworkPropertyMetadata(typeof(ColorPicker)));
        }

        #endregion CLASS METHODS

        #region COLOR MANAGEMETN METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get color from color slider value. </summary>
        /// <param name="value"> External color slider value. </param>
        /// <returns> Color. </returns>
        private Color GetColorSliderColor(double? value = null)
        {
            double sliderValue = value ?? _colorSlider.Value;

            if (sliderValue <= 255)
            {
                //  Red 255; Green 0 - 255; Blue 0
                var green = ConvertToRGBColorComponent(sliderValue);
                return Color.FromRgb(255, green, 0);
            }
            else if (sliderValue >= 256 && sliderValue <= 511)
            {
                //  Red 255 - 0; Green 255; Blue 0;
                var red = ConvertToRGBColorComponent(255 - (sliderValue - 256));
                return Color.FromRgb(red, 255, 0);
            }
            else if (sliderValue >= 512 && sliderValue <= 767)
            {
                //  Red 0; Green 255; Blue 0 - 255
                var blue = ConvertToRGBColorComponent(sliderValue - 512);
                return Color.FromRgb(0, 255, blue);
            }
            else if (sliderValue >= 768 && sliderValue <= 1023)
            {
                //  Red 0; Green 255 - 0; Blue 255
                var green = ConvertToRGBColorComponent(255 - (sliderValue - 768));
                return Color.FromRgb(0, green, 255);
            }
            else if (sliderValue >= 1024 && sliderValue <= 1279)
            {
                //  Red 0 - 255; Green 0; Blue 255
                var red = ConvertToRGBColorComponent(sliderValue - 1024);
                return Color.FromRgb(red, 0, 255);
            }
            else if (sliderValue >= 1280)
            {
                //  Red 255; Green 0; Blue 255 - 0
                var blue = ConvertToRGBColorComponent(255 - (sliderValue - 1280));
                return Color.FromRgb(255, 0, blue);
            }
            else
                return Colors.Red;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get intensity value from slider. </summary>
        /// <param name="value"> External intensity value. </param>
        /// <returns> Intensity. </returns>
        private int GetIntensitySliderValue(double? value = null)
        {
            return (int)Math.Max(0, Math.Min(150, value ?? _intensitySlider.Value));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get lightness value from slider. </summary>
        /// <param name="value"> External lightness value. </param>
        /// <returns> Lightness. </returns>
        private int GetLightnessSliderValue(double? value = null)
        {
            return (int)Math.Max(0, Math.Min(50, value ?? _lightnessSlider.Value));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set selected color. </summary>
        /// <param name="color"> Color. </param>
        /// <param name="s"> Stauration / Intensity. </param>
        /// <param name="l"> Lightness. </param>
        private void SetSelectedColor(Color color, int s, int l)
        {
            var lightness = s < 50 ? (50 - s) + l : l;
            var saturation = Math.Max(0, s - 50);

            var hue = AHSLColor.FromColor(color).H;
            var ahslColor = new AHSLColor(255, hue, saturation, lightness);

            SelectedColor = new ThemeColor(ahslColor.ToColor()); 
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set sliders values. </summary>
        /// <param name="h"> Hue. </param>
        /// <param name="s"> Stauration / Intensity. </param>
        /// <param name="l"> Lightness. </param>
        private void SetSliders(int h, int s, int l)
        {
            _colorSlider.Value = Get1535Hue(h);
            _lightnessSlider.Value = Math.Min(50, l);
            _intensitySlider.Value = Math.Min(150, l > 50 ? s + 50 : s);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set visual selector position. </summary>
        /// <param name="s"> Saturation / Intensity. </param>
        /// <param name="l"> Lightness. </param>
        private void SetVisualSelectorPosition(int? s = null, int? l = null)
        {
            var intensity = GetIntensitySliderValue(s);
            var lightness = GetLightnessSliderValue(l);

            var x = (intensity * _canvasGrid.ActualWidth) / 150;
            var y = ((50 - lightness) * _canvasGrid.ActualHeight) / 50;

            _visualSelectorEllipse.Margin = new Thickness(x - 8, y - 8, 0, 0);
        }

        #endregion COLOR MANAGEMETN METHODS

        #region COMPONENTS INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after value change in color slider. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Property Changed Event Arguments. </param>
        private void OnColorValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_colorSliderManualChange)
            {
                ColorMain = GetColorSliderColor(e.NewValue);

                var intensity = GetIntensitySliderValue();
                var lightness = GetLightnessSliderValue();

                SetSelectedColor(ColorMain, intensity, lightness);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after value change in intensity slider. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Property Changed Event Arguments. </param>
        private void OnIntensityValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_intensitySliderManualChange)
            {
                var intensity = GetIntensitySliderValue(e.NewValue);
                var lightness = GetLightnessSliderValue();

                SetSelectedColor(ColorMain, intensity, lightness);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after value change in lightness slider. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Property Changed Event Arguments. </param>
        private void OnLightnessValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_lightnessSliderManualChange)
            {
                var intensity = GetIntensitySliderValue();
                var lightness = GetLightnessSliderValue(e.NewValue);

                SetSelectedColor(ColorMain, intensity, lightness);
            }
        }

        #endregion COMPONENTS INTERACTION METHODS

        #region CONTROL METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Participates in rendering operations that are directed by the layout system. </summary>
        /// <param name="drawingContext"> Drawing context. </param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            SetVisualSelectorPosition();
        }

        #endregion CONTROL METHODS

        #region NOTIFICATION PROPERTIES CHANGED METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Invoke selection changed event. </summary>
        /// <param name="themeColor"> External theme color. </param>
        private void InvokeSelectionChanged(ThemeColor themeColor = null)
        {
            SelectionChanged?.Invoke(this, new ColorSelectionChangedEventArgs(themeColor ?? SelectedColor));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after changing main color property. </summary>
        /// <param name="s"> Dependency object in which method is invoked. </param>
        /// <param name="e"> Dependency Property Changed Event Arguments. </param>
        private static void MainColorPropertyChangedCallback(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            if (s is ColorPicker instance)
            {
                if (instance.IsUserChanging)
                    return;

                if (instance._isColorChanging)
                    return;

                var color = (Color)e.NewValue;
                int intensity = instance.GetIntensitySliderValue();
                int lightness = instance.GetLightnessSliderValue();

                instance.SetSelectedColor(color, intensity, lightness);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after changing selected color property. </summary>
        /// <param name="s"> Dependency object in which method is invoked. </param>
        /// <param name="e"> Dependency Property Changed Event Arguments. </param>
        private static void SelectedColorPropertyChangedCallback(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            if (s is ColorPicker instance && e.NewValue != null)
            {
                instance._isColorChanging = true;

                var themeColor = (ThemeColor)e.NewValue;
                var ahslColor = AHSLColor.FromColor(themeColor.Color);

                if (instance.IsUserChanging)
                {
                    instance.InvokeSelectionChanged(themeColor);
                }
                else
                {
                    instance.SetSliders(ahslColor.H, ahslColor.S, ahslColor.L);
                    instance.ColorMain = instance.GetColorSliderColor(instance.Get1535Hue(ahslColor.H)); 
                }

                instance.SetVisualSelectorPosition();

                instance._isColorChanging = false;
            }
        }

        #endregion NOTIFICATION PROPERTIES CHANGED METHODS

        #region TEMPLATE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> When overridden in a derived class,cis invoked whenever 
        /// application code or internal processes call ApplyTemplate. </summary>
        public override void OnApplyTemplate()
        {
            //  Apply Template
            base.OnApplyTemplate();

            _colorSlider = GetSlider("_colorSlider");
            _intensitySlider = GetSlider("_intensitySlider");
            _lightnessSlider = GetSlider("_lightnessSlider");
            _canvasGrid = GetGrid("_canvasGrid");
            _visualSelectorEllipse = GetEllipse("_visualSelectorEllipse");

            SetComponentPreviewMouseDown(_colorSlider, (s, e) => { _colorSliderManualChange = true; });
            SetComponentPreviewMouseDown(_intensitySlider, (s, e) => { _intensitySliderManualChange = true; });
            SetComponentPreviewMouseDown(_lightnessSlider, (s, e) => { _lightnessSliderManualChange = true; });

            SetComponentPreviewMouseUp(_colorSlider, (s, e) => { _colorSliderManualChange = false; });
            SetComponentPreviewMouseUp(_intensitySlider, (s, e) => { _intensitySliderManualChange = false; });
            SetComponentPreviewMouseUp(_lightnessSlider, (s, e) => { _lightnessSliderManualChange = false; });

            SetValueChanged(_colorSlider, OnColorValueChanged);
            SetValueChanged(_intensitySlider, OnIntensityValueChanged);
            SetValueChanged(_lightnessSlider, OnLightnessValueChanged);

            if (SelectedColor != null)
            {
                var ahslColor = AHSLColor.FromColor(SelectedColor.Color);
                SetSliders(ahslColor.H, ahslColor.S, ahslColor.L);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get Ellipse from ContentTemplate. </summary>
        /// <param name="ellipseName"> Ellipse name. </param>
        /// <returns> Ellipse or null. </returns>
        private Ellipse GetEllipse(string ellipseName)
        {
            return this.Template.FindName(ellipseName, this) as Ellipse;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get Grid from ContentTemplate. </summary>
        /// <param name="gridName"> Grid name. </param>
        /// <returns> Grid or null. </returns>
        private Grid GetGrid(string gridName)
        {
            return this.Template.FindName(gridName, this) as Grid;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get Slider from ContentTemplate. </summary>
        /// <param name="sliderName"> Slider name. </param>
        /// <returns> Slider or null. </returns>
        private Slider GetSlider(string sliderName)
        {
            return this.Template.FindName(sliderName, this) as Slider;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set component preview mouse down event handler. </summary>
        /// <param name="listView"> Framework element. </param>
        /// <param name="handler"> Selection changed event handler. </param>
        private void SetComponentPreviewMouseDown(FrameworkElement frameworkElement, MouseButtonEventHandler handler)
        {
            if (frameworkElement != null && handler != null)
                frameworkElement.PreviewMouseDown += handler;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set component preview mouse up event handler. </summary>
        /// <param name="listView"> Framework element. </param>
        /// <param name="handler"> Selection changed event handler. </param>
        private void SetComponentPreviewMouseUp(FrameworkElement frameworkElement, MouseButtonEventHandler handler)
        {
            if (frameworkElement != null && handler != null)
                frameworkElement.PreviewMouseUp += handler;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set slider value changed event handler. </summary>
        /// <param name="slider"> Slider. </param>
        /// <param name="handler"> Routed property changed event handler. </param>
        private void SetValueChanged(Slider slider, RoutedPropertyChangedEventHandler<double> handler)
        {
            if (slider != null && handler != null)
                slider.ValueChanged += handler;
        }

        #endregion TEMPLATE METHODS

        #region UTILITY METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Convert slider value to rgb color component. </summary>
        /// <param name="value"> Slider value. </param>
        /// <returns> RGB color component. </returns>
        private byte ConvertToRGBColorComponent(double value)
        {
            return (byte)Math.Max(0, Math.Min(255, value));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get 1535 hue. </summary>
        /// <param name="hue"> AHSL color hue. </param>
        /// <returns> 1535 hue. </returns>
        private int Get1535Hue(int hue)
        {
            return hue + (int)Math.Floor(hue / 256d);
        }

        #endregion UTILITY METHODS

    }
}
