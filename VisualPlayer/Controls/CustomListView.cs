using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VisualPlayer.Data.ColorModels;

namespace VisualPlayer.Controls
{
    public class CustomListView : ListView
    {

        //  DEPENDENCY PROPERTIES

        public static readonly DependencyProperty ColumnHeaderBackgroundProperty = DependencyProperty.Register(
            nameof(ColumnHeaderBackground),
            typeof(Brush),
            typeof(CustomListView),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultThemeColorShade)));

        public static readonly DependencyProperty ColumnHeaderBackgroundEmptyProperty = DependencyProperty.Register(
            nameof(ColumnHeaderBackgroundEmpty),
            typeof(Brush),
            typeof(CustomListView),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultThemeColorShade)));

        public static readonly DependencyProperty ColumnHeaderBackgroundMouseOverProperty = DependencyProperty.Register(
            nameof(ColumnHeaderBackgroundMouseOver),
            typeof(Brush),
            typeof(CustomListView),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultThemeColorMouseOver)));

        public static readonly DependencyProperty ColumnHeaderBackgroundPressedProperty = DependencyProperty.Register(
            nameof(ColumnHeaderBackgroundPressed),
            typeof(Brush),
            typeof(CustomListView),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultThemeColorPressed)));

        public static readonly DependencyProperty ColumnHeaderBorderBrushProperty = DependencyProperty.Register(
            nameof(ColumnHeaderBorderBrush),
            typeof(Brush),
            typeof(CustomListView),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultAccentColor)));

        public static readonly DependencyProperty ColumnHeaderBorderBrushEmptyProperty = DependencyProperty.Register(
            nameof(ColumnHeaderBorderBrushEmpty),
            typeof(Brush),
            typeof(CustomListView),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultAccentColor)));

        public static readonly DependencyProperty ColumnHeaderBorderBrushMouseOverProperty = DependencyProperty.Register(
            nameof(ColumnHeaderBorderBrushMouseOver),
            typeof(Brush),
            typeof(CustomListView),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultAccentColorMouseOver)));

        public static readonly DependencyProperty ColumnHeaderBorderBrushPressedProperty = DependencyProperty.Register(
            nameof(ColumnHeaderBorderBrushPressed),
            typeof(Brush),
            typeof(CustomListView),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultAccentColorPressed)));

        public static readonly DependencyProperty ColumnHeaderBorderThicknessProperty = DependencyProperty.Register(
            nameof(ColumnHeaderBorderThickness),
            typeof(Thickness),
            typeof(CustomListView),
            new PropertyMetadata(new Thickness(0, 0, 1, 1)));

        public static readonly DependencyProperty ColumnHeaderCornerRadiusProperty = DependencyProperty.Register(
            nameof(ColumnHeaderCornerRadius),
            typeof(CornerRadius),
            typeof(CustomListView),
            new PropertyMetadata(new CornerRadius(0)));

        public static readonly DependencyProperty ColumnHeaderForegroundProperty = DependencyProperty.Register(
            nameof(ColumnHeaderForeground),
            typeof(Brush),
            typeof(CustomListView),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultThemeColorForeground)));

        public static readonly DependencyProperty ColumnHeaderForegroundMouseOverProperty = DependencyProperty.Register(
            nameof(ColumnHeaderForegroundMouseOver),
            typeof(Brush),
            typeof(CustomListView),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultThemeColorForeground)));

        public static readonly DependencyProperty ColumnHeaderForegroundPressedProperty = DependencyProperty.Register(
            nameof(ColumnHeaderForegroundPressed),
            typeof(Brush),
            typeof(CustomListView),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultThemeColorForeground)));

        public static readonly DependencyProperty ColumnHeaderGripperWidthProperty = DependencyProperty.Register(
            nameof(ColumnHeaderGripperWidth),
            typeof(double),
            typeof(CustomListView),
            new PropertyMetadata(8d));

        public static readonly DependencyProperty ColumnHeaderMarginProperty = DependencyProperty.Register(
            nameof(ColumnHeaderMargin),
            typeof(Thickness),
            typeof(CustomListView),
            new PropertyMetadata(new Thickness(0)));

        public static readonly DependencyProperty ColumnHeaderPaddingProperty = DependencyProperty.Register(
            nameof(ColumnHeaderPadding),
            typeof(Thickness),
            typeof(CustomListView),
            new PropertyMetadata(new Thickness(8, 2, 8, 2)));

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            nameof(CornerRadius),
            typeof(CornerRadius),
            typeof(CustomListView),
            new PropertyMetadata(new CornerRadius(0)));


        //  GETTERS & SETTERS

        public Brush ColumnHeaderBackground
        {
            get => (Brush)GetValue(ColumnHeaderBackgroundProperty);
            set => SetValue(ColumnHeaderBackgroundProperty, value);
        }

        public Brush ColumnHeaderBackgroundEmpty
        {
            get => (Brush)GetValue(ColumnHeaderBackgroundEmptyProperty);
            set => SetValue(ColumnHeaderBackgroundEmptyProperty, value);
        }

        public Brush ColumnHeaderBackgroundMouseOver
        {
            get => (Brush)GetValue(ColumnHeaderBackgroundMouseOverProperty);
            set => SetValue(ColumnHeaderBackgroundMouseOverProperty, value);
        }

        public Brush ColumnHeaderBackgroundPressed
        {
            get => (Brush)GetValue(ColumnHeaderBackgroundPressedProperty);
            set => SetValue(ColumnHeaderBackgroundPressedProperty, value);
        }

        public Brush ColumnHeaderBorderBrush
        {
            get => (Brush)GetValue(ColumnHeaderBorderBrushProperty);
            set => SetValue(ColumnHeaderBorderBrushProperty, value);
        }

        public Brush ColumnHeaderBorderBrushEmpty
        {
            get => (Brush)GetValue(ColumnHeaderBorderBrushEmptyProperty);
            set => SetValue(ColumnHeaderBorderBrushEmptyProperty, value);
        }

        public Brush ColumnHeaderBorderBrushMouseOver
        {
            get => (Brush)GetValue(ColumnHeaderBorderBrushMouseOverProperty);
            set => SetValue(ColumnHeaderBorderBrushMouseOverProperty, value);
        }

        public Brush ColumnHeaderBorderBrushPressed
        {
            get => (Brush)GetValue(ColumnHeaderBorderBrushPressedProperty);
            set => SetValue(ColumnHeaderBorderBrushPressedProperty, value);
        }

        public Thickness ColumnHeaderBorderThickness
        {
            get => (Thickness)GetValue(ColumnHeaderBorderThicknessProperty);
            set => SetValue(ColumnHeaderBorderThicknessProperty, value);
        }

        public CornerRadius ColumnHeaderCornerRadius
        {
            get => (CornerRadius)GetValue(ColumnHeaderCornerRadiusProperty);
            set => SetValue(ColumnHeaderCornerRadiusProperty, value);
        }

        public Brush ColumnHeaderForeground
        {
            get => (Brush)GetValue(ColumnHeaderForegroundProperty);
            set => SetValue(ColumnHeaderForegroundProperty, value);
        }

        public Brush ColumnHeaderForegroundMouseOver
        {
            get => (Brush)GetValue(ColumnHeaderForegroundMouseOverProperty);
            set => SetValue(ColumnHeaderForegroundMouseOverProperty, value);
        }

        public Brush ColumnHeaderForegroundPressed
        {
            get => (Brush)GetValue(ColumnHeaderForegroundPressedProperty);
            set => SetValue(ColumnHeaderForegroundPressedProperty, value);
        }

        public double ColumnHeaderGripperWidth
        {
            get => (double)GetValue(ColumnHeaderGripperWidthProperty);
            set => SetValue(ColumnHeaderGripperWidthProperty, value);
        }

        public Thickness ColumnHeaderMargin
        {
            get => (Thickness)GetValue(ColumnHeaderMarginProperty);
            set => SetValue(ColumnHeaderMarginProperty, value);
        }

        public Thickness ColumnHeaderPadding
        {
            get => (Thickness)GetValue(ColumnHeaderPaddingProperty);
            set => SetValue(ColumnHeaderPaddingProperty, value);
        }

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> CustomListView static class constructor. </summary>
        static CustomListView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomListView),
                new FrameworkPropertyMetadata(typeof(CustomListView)));
        }

        #endregion CLASS METHODS

        #region ITEMS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Creates and returns new ListViewItemEx container. </summary>
        /// <returns> A new ListViewItemEx control. </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new CustomListViewItem();
        }

        #endregion ITEMS METHODS

    }
}
