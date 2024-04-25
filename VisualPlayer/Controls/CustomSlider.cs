using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VisualPlayer.Data.ColorModels;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace VisualPlayer.Controls
{
    public class CustomSlider : Slider
    {

        //  DEPENDENCY PROPERTIES

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            nameof(CornerRadius),
            typeof(CornerRadius),
            typeof(CustomSlider),
            new PropertyMetadata(new CornerRadius(8)));

        public static readonly DependencyProperty SelectionRangeColorBrushProperty = DependencyProperty.Register(
            nameof(SelectionRangeColorBrush),
            typeof(Brush),
            typeof(CustomSlider),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultAccentColor)));

        public static readonly DependencyProperty TicksColorBrushProperty = DependencyProperty.Register(
            nameof(TicksColorBrush),
            typeof(Brush),
            typeof(CustomSlider),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultAccentColor)));

        //  --- Thumb ---

        public static readonly DependencyProperty ThumbBackgroundDraggingProperty = DependencyProperty.Register(
            nameof(ThumbBackgroundDragging),
            typeof(Brush),
            typeof(CustomSlider),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultAccentColorPressed)));

        public static readonly DependencyProperty ThumbBackgroundMouseOverProperty = DependencyProperty.Register(
            nameof(ThumbBackgroundMouseOver),
            typeof(Brush),
            typeof(CustomSlider),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultAccentColorMouseOver)));

        public static readonly DependencyProperty ThumbBorderBrushProperty = DependencyProperty.Register(
            nameof(ThumbBorderBrush),
            typeof(Brush),
            typeof(CustomSlider),
            new PropertyMetadata(null));

        public static readonly DependencyProperty ThumbBorderBrushDraggingProperty = DependencyProperty.Register(
            nameof(ThumbBorderBrushDragging),
            typeof(Brush),
            typeof(CustomSlider),
            new PropertyMetadata(null));

        public static readonly DependencyProperty ThumbBorderBrushMouseOverProperty = DependencyProperty.Register(
            nameof(ThumbBorderBrushMouseOver),
            typeof(Brush),
            typeof(CustomSlider),
            new PropertyMetadata(null));

        public static readonly DependencyProperty ThumbBorderThicknessProperty = DependencyProperty.Register(
            nameof(ThumbBorderThickness),
            typeof(Thickness),
            typeof(CustomSlider),
            new PropertyMetadata(new Thickness(0)));

        public static readonly DependencyProperty ThumbCornerRadiusProperty = DependencyProperty.Register(
            nameof(ThumbCornerRadius),
            typeof(CornerRadius),
            typeof(CustomSlider),
            new PropertyMetadata(new CornerRadius(4)));

        public static readonly DependencyProperty ThumbHeightProperty = DependencyProperty.Register(
            nameof(ThumbHeight),
            typeof(double),
            typeof(CustomSlider),
            new PropertyMetadata(32d));

        public static readonly DependencyProperty ThumbWidthProperty = DependencyProperty.Register(
            nameof(ThumbWidth),
            typeof(double),
            typeof(CustomSlider),
            new PropertyMetadata(32d));

        //  --- TrackBar ---

        public static readonly DependencyProperty TrackBarBackgroundProperty = DependencyProperty.Register(
            nameof(TrackBarBackground),
            typeof(Brush),
            typeof(CustomSlider),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultThemeColorShade)));

        public static readonly DependencyProperty TrackBarBorderBrushProperty = DependencyProperty.Register(
            nameof(TrackBarBorderBrush),
            typeof(Brush),
            typeof(CustomSlider),
            new PropertyMetadata(null));

        public static readonly DependencyProperty TrackBarBorderThicknessProperty = DependencyProperty.Register(
            nameof(TrackBarBorderThickness),
            typeof(Thickness),
            typeof(CustomSlider),
            new PropertyMetadata(new Thickness(0)));

        public static readonly DependencyProperty TrackBarCornerRadiusProperty = DependencyProperty.Register(
            nameof(TrackBarCornerRadius),
            typeof(CornerRadius),
            typeof(CustomSlider),
            new PropertyMetadata(new CornerRadius(4)));

        public static readonly DependencyProperty TrackBarHeightProperty = DependencyProperty.Register(
            nameof(TrackBarHeight),
            typeof(double),
            typeof(CustomSlider),
            new PropertyMetadata(6d));

        public static readonly DependencyProperty TrackBarWidthProperty = DependencyProperty.Register(
            nameof(TrackBarWidth),
            typeof(double),
            typeof(CustomSlider),
            new PropertyMetadata(6d));


        //  GETTERS & SETTERS

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public Brush SelectionRangeColorBrush
        {
            get => (Brush)GetValue(SelectionRangeColorBrushProperty);
            set => SetValue(SelectionRangeColorBrushProperty, value);
        }

        public Brush TicksColorBrush
        {
            get => (Brush)GetValue(TicksColorBrushProperty);
            set => SetValue(TicksColorBrushProperty, value);
        }

        public Brush ThumbBackgroundDragging
        {
            get => (Brush)GetValue(ThumbBackgroundDraggingProperty);
            set => SetValue(ThumbBackgroundDraggingProperty, value);
        }

        public Brush ThumbBackgroundMouseOver
        {
            get => (Brush)GetValue(ThumbBackgroundMouseOverProperty);
            set => SetValue(ThumbBackgroundMouseOverProperty, value);
        }

        public Brush ThumbBorderBrush
        {
            get => (Brush)GetValue(ThumbBorderBrushProperty);
            set => SetValue(ThumbBorderBrushProperty, value);
        }

        public Brush ThumbBorderBrushDragging
        {
            get => (Brush)GetValue(ThumbBorderBrushDraggingProperty);
            set => SetValue(ThumbBorderBrushDraggingProperty, value);
        }

        public Brush ThumbBorderBrushMouseOver
        {
            get => (Brush)GetValue(ThumbBorderBrushMouseOverProperty);
            set => SetValue(ThumbBorderBrushMouseOverProperty, value);
        }

        public Thickness ThumbBorderThickness
        {
            get => (Thickness)GetValue(ThumbBorderThicknessProperty);
            set => SetValue(ThumbBorderThicknessProperty, value);
        }

        public CornerRadius ThumbCornerRadius
        {
            get => (CornerRadius)GetValue(ThumbCornerRadiusProperty);
            set => SetValue(ThumbCornerRadiusProperty, value);
        }

        public double ThumbHeight
        {
            get => (double)GetValue(ThumbHeightProperty);
            set => SetValue(ThumbHeightProperty, value);
        }

        public double ThumbWidth
        {
            get => (double)GetValue(ThumbWidthProperty);
            set => SetValue(ThumbWidthProperty, value);
        }

        public Brush TrackBarBackground
        {
            get => (Brush)GetValue(TrackBarBackgroundProperty);
            set => SetValue(TrackBarBackgroundProperty, value);
        }

        public Brush TrackBarBorderBrush
        {
            get => (Brush)GetValue(TrackBarBorderBrushProperty);
            set => SetValue(TrackBarBorderBrushProperty, value);
        }

        public Thickness TrackBarBorderThickness
        {
            get => (Thickness)GetValue(TrackBarBorderThicknessProperty);
            set => SetValue (TrackBarBorderThicknessProperty, value);
        }

        public CornerRadius TrackBarCornerRadius
        {
            get => (CornerRadius)GetValue(TrackBarCornerRadiusProperty);
            set => SetValue(TrackBarCornerRadiusProperty, value);
        }

        public double TrackBarHeight
        {
            get => (double)GetValue(TrackBarHeightProperty);
            set => SetValue(TrackBarHeightProperty, value);
        }

        public double TrackBarWidth
        {
            get => (double)GetValue(TrackBarWidthProperty);
            set => SetValue(TrackBarWidthProperty, value);
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Static CustomSlider class constructor. </summary>
        static CustomSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomSlider),
                new FrameworkPropertyMetadata(typeof(CustomSlider)));
        }

        #endregion CLASS METHODS

    }
}
