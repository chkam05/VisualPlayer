using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using VisualPlayer.Data.ColorModels;

namespace VisualPlayer.Controls
{
    public class CustomScrollBar : ScrollBar
    {

        //  DEPENDENCY PROPERTIES

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            nameof(CornerRadius),
            typeof(CornerRadius),
            typeof(CustomScrollBar),
            new PropertyMetadata(new CornerRadius(8)));

        public static readonly DependencyProperty ThumbBackgroundProperty = DependencyProperty.Register(
            nameof(ThumbBackground),
            typeof(Brush),
            typeof(CustomScrollBar),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultAccentColor)));

        public static readonly DependencyProperty ThumbBackgroundDraggingProperty = DependencyProperty.Register(
            nameof(ThumbBackgroundDragging),
            typeof(Brush),
            typeof(CustomScrollBar),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultAccentColorPressed)));

        public static readonly DependencyProperty ThumbBackgroundMouseOverProperty = DependencyProperty.Register(
            nameof(ThumbBackgroundMouseOver),
            typeof(Brush),
            typeof(CustomScrollBar),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultAccentColorMouseOver)));

        public static readonly DependencyProperty ThumbBorderBrushProperty = DependencyProperty.Register(
            nameof(ThumbBorderBrush),
            typeof(Brush),
            typeof(CustomScrollBar),
            new PropertyMetadata(null));

        public static readonly DependencyProperty ThumbBorderBrushDraggingProperty = DependencyProperty.Register(
            nameof(ThumbBorderBrushDragging),
            typeof(Brush),
            typeof(CustomScrollBar),
            new PropertyMetadata(null));

        public static readonly DependencyProperty ThumbBorderBrushMouseOverProperty = DependencyProperty.Register(
            nameof(ThumbBorderBrushMouseOver),
            typeof(Brush),
            typeof(CustomScrollBar),
            new PropertyMetadata(null));

        public static readonly DependencyProperty ThumbBorderThicknessProperty = DependencyProperty.Register(
            nameof(ThumbBorderThickness),
            typeof(Thickness),
            typeof(CustomScrollBar),
            new PropertyMetadata(new Thickness(0)));

        public static readonly DependencyProperty ThumbCornerRadiusProperty = DependencyProperty.Register(
            nameof(ThumbCornerRadius),
            typeof(CornerRadius),
            typeof(CustomScrollBar),
            new PropertyMetadata(new CornerRadius(4)));


        //  GETTERS & SETTERS

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public Brush ThumbBackground
        {
            get => (Brush)GetValue(ThumbBackgroundProperty);
            set => SetValue(ThumbBackgroundProperty, value);
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


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> CustomScrollBar static class constructor. </summary>
        static CustomScrollBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomScrollBar),
                new FrameworkPropertyMetadata(typeof(CustomScrollBar)));
        }

        #endregion CLASS METHODS

    }
}
