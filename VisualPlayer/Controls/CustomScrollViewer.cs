using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VisualPlayer.Data.ColorModels;

namespace VisualPlayer.Controls
{
    public class CustomScrollViewer : ScrollViewer
    {

        //  DEPENDENCY PROPERTIES

        public static readonly DependencyProperty ScrollBarBackgroundProperty = DependencyProperty.Register(
            nameof(ScrollBarBackground),
            typeof(Brush),
            typeof(CustomScrollViewer),
            new PropertyMetadata(null));

        public static readonly DependencyProperty ScrollBarCornerBackgroundProperty = DependencyProperty.Register(
            nameof(ScrollBarCornerBackground),
            typeof(Brush),
            typeof(CustomScrollViewer),
            new PropertyMetadata(new SolidColorBrush(System.Windows.Media.Colors.Transparent)));

        public static readonly DependencyProperty ScrollBarHorizontalHeightProperty = DependencyProperty.Register(
            nameof(ScrollBarHorizontalHeight),
            typeof(double),
            typeof(CustomScrollViewer),
            new PropertyMetadata(16d));

        public static readonly DependencyProperty ScrollBarThumbBackgroundProperty = DependencyProperty.Register(
            nameof(ScrollBarThumbBackground),
            typeof(Brush),
            typeof(CustomScrollViewer),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultAccentColor)));

        public static readonly DependencyProperty ScrollBarThumbBackgroundDraggingProperty = DependencyProperty.Register(
            nameof(ScrollBarThumbBackgroundDragging),
            typeof(Brush),
            typeof(CustomScrollViewer),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultAccentColorPressed)));

        public static readonly DependencyProperty ScrollBarThumbBackgroundMouseOverProperty = DependencyProperty.Register(
            nameof(ScrollBarThumbBackgroundMouseOver),
            typeof(Brush),
            typeof(CustomScrollViewer),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultAccentColorMouseOver)));

        public static readonly DependencyProperty ScrollBarThumbBorderBrushProperty = DependencyProperty.Register(
            nameof(ScrollBarThumbBorderBrush),
            typeof(Brush),
            typeof(CustomScrollViewer),
            new PropertyMetadata(null));

        public static readonly DependencyProperty ScrollBarThumbBorderBrushDraggingProperty = DependencyProperty.Register(
            nameof(ScrollBarThumbBorderBrushDragging),
            typeof(Brush),
            typeof(CustomScrollViewer),
            new PropertyMetadata(null));

        public static readonly DependencyProperty ScrollBarThumbBorderBrushMouseOverProperty = DependencyProperty.Register(
            nameof(ScrollBarThumbBorderBrushMouseOver),
            typeof(Brush),
            typeof(CustomScrollViewer),
            new PropertyMetadata(new SolidColorBrush(System.Windows.Media.Colors.Transparent)));

        public static readonly DependencyProperty ScrollBarThumbBorderThicknessProperty = DependencyProperty.Register(
            nameof(ScrollBarThumbBorderThickness),
            typeof(Thickness),
            typeof(CustomScrollViewer),
            new PropertyMetadata(new Thickness(0)));

        public static readonly DependencyProperty ScrollBarThumbCornerRadiusProperty = DependencyProperty.Register(
            nameof(ScrollBarThumbCornerRadius),
            typeof(CornerRadius),
            typeof(CustomScrollViewer),
            new PropertyMetadata(new CornerRadius(4)));

        public static readonly DependencyProperty ScrollBarThumbPaddingProperty = DependencyProperty.Register(
            nameof(ScrollBarThumbPadding),
            typeof(Thickness),
            typeof(CustomScrollViewer),
            new PropertyMetadata(new Thickness(0)));

        public static readonly DependencyProperty ScrollBarVerticalWidthProperty = DependencyProperty.Register(
            nameof(ScrollBarVerticalWidth),
            typeof(double),
            typeof(CustomScrollViewer),
            new PropertyMetadata(16d));


        //  GETTERS & SETTERS

        public Brush ScrollBarBackground
        {
            get => (Brush)GetValue(ScrollBarBackgroundProperty);
            set => SetValue(ScrollBarBackgroundProperty, value);
        }

        public Brush ScrollBarCornerBackground
        {
            get => (Brush)GetValue(ScrollBarCornerBackgroundProperty);
            set => SetValue(ScrollBarCornerBackgroundProperty, value);
        }

        public double ScrollBarHorizontalHeight
        {
            get => (double)GetValue(ScrollBarHorizontalHeightProperty);
            set => SetValue(ScrollBarHorizontalHeightProperty, value);
        }

        public Brush ScrollBarThumbBackground
        {
            get => (Brush)GetValue(ScrollBarThumbBackgroundProperty);
            set => SetValue(ScrollBarThumbBackgroundProperty, value);
        }

        public Brush ScrollBarThumbBackgroundDragging
        {
            get => (Brush)GetValue(ScrollBarThumbBackgroundDraggingProperty);
            set => SetValue(ScrollBarThumbBackgroundDraggingProperty, value);
        }

        public Brush ScrollBarThumbBackgroundMouseOver
        {
            get => (Brush)GetValue(ScrollBarThumbBackgroundMouseOverProperty);
            set => SetValue(ScrollBarThumbBackgroundMouseOverProperty, value);
        }

        public Brush ScrollBarThumbBorderBrush
        {
            get => (Brush)GetValue(ScrollBarThumbBorderBrushProperty);
            set => SetValue(ScrollBarThumbBorderBrushProperty, value);
        }

        public Brush ScrollBarThumbBorderBrushDragging
        {
            get => (Brush)GetValue(ScrollBarThumbBorderBrushDraggingProperty);
            set => SetValue(ScrollBarThumbBorderBrushDraggingProperty, value);
        }

        public Brush ScrollBarThumbBorderBrushMouseOver
        {
            get => (Brush)GetValue(ScrollBarThumbBorderBrushMouseOverProperty);
            set => SetValue(ScrollBarThumbBorderBrushMouseOverProperty, value);
        }

        public Thickness ScrollBarThumbBorderThickness
        {
            get => (Thickness)GetValue(ScrollBarThumbBorderThicknessProperty);
            set => SetValue(ScrollBarThumbBorderThicknessProperty, value);
        }

        public CornerRadius ScrollBarThumbCornerRadius
        {
            get => (CornerRadius)GetValue(ScrollBarThumbCornerRadiusProperty);
            set => SetValue(ScrollBarThumbCornerRadiusProperty, value);
        }

        public Thickness ScrollBarThumbPadding
        {
            get => (Thickness)GetValue(ScrollBarThumbPaddingProperty);
            set => SetValue(ScrollBarThumbPaddingProperty, value);
        }

        public double ScrollBarVerticalWidth
        {
            get => (double)GetValue(ScrollBarVerticalWidthProperty);
            set => SetValue(ScrollBarVerticalWidthProperty, value);
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> CustomScrollViewer static class constructor. </summary>
        static CustomScrollViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomScrollViewer),
                new FrameworkPropertyMetadata(typeof(CustomScrollViewer)));
        }

        #endregion CLASS METHODS

    }
}
