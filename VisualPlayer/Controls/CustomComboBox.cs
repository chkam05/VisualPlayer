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
    public class CustomComboBox : ComboBox
    {

        //  DEPENDENCY PROPERTIES

        public static readonly DependencyProperty BackgroundMouseOverProperty = DependencyProperty.Register(
            nameof(BackgroundMouseOver),
            typeof(Brush),
            typeof(CustomComboBox),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultAccentColorMouseOver)));

        public static readonly DependencyProperty BackgroundSelectedProperty = DependencyProperty.Register(
            nameof(BackgroundSelected),
            typeof(Brush),
            typeof(CustomComboBox),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultAccentColorSelected)));

        public static readonly DependencyProperty BorderBrushMouseOverProperty = DependencyProperty.Register(
            nameof(BorderBrushMouseOver),
            typeof(Brush),
            typeof(CustomComboBox),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultAccentColor)));

        public static readonly DependencyProperty BorderBrushSelectedProperty = DependencyProperty.Register(
            nameof(BorderBrushSelected),
            typeof(Brush),
            typeof(CustomComboBox),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultAccentColor)));

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            nameof(CornerRadius),
            typeof(CornerRadius),
            typeof(CustomComboBox),
            new PropertyMetadata(new CornerRadius(4)));

        public static readonly DependencyProperty DropDownBackgroundProperty = DependencyProperty.Register(
            nameof(DropDownBackground),
            typeof(Brush),
            typeof(CustomComboBox),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultThemeColorShade)));

        public static readonly DependencyProperty DropDownBorderBrushProperty = DependencyProperty.Register(
            nameof(DropDownBorderBrush),
            typeof(Brush),
            typeof(CustomComboBox),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultAccentColor)));

        public static readonly DependencyProperty DropDownBorderThicknessProperty = DependencyProperty.Register(
            nameof(DropDownBorderThickness),
            typeof(Thickness),
            typeof(CustomComboBox),
            new PropertyMetadata(new Thickness(1)));

        public static readonly DependencyProperty DropDownCornerRadiusProperty = DependencyProperty.Register(
            nameof(DropDownCornerRadius),
            typeof(CornerRadius),
            typeof(CustomComboBox),
            new PropertyMetadata(new CornerRadius(4)));

        public static readonly DependencyProperty DropDownForegroundProperty = DependencyProperty.Register(
            nameof(DropDownForeground),
            typeof(Brush),
            typeof(CustomComboBox),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultThemeColorForeground)));

        public static readonly DependencyProperty ForegroundMouseOverProperty = DependencyProperty.Register(
            nameof(ForegroundMouseOver),
            typeof(Brush),
            typeof(CustomComboBox),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultAccentColorForeground)));

        public static readonly DependencyProperty ForegroundSelectedProperty = DependencyProperty.Register(
            nameof(ForegroundSelected),
            typeof(Brush),
            typeof(CustomComboBox),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultAccentColorForeground)));


        //  GETTERS & SETTERS

        public Brush BackgroundMouseOver
        {
            get => (Brush)GetValue(BackgroundMouseOverProperty);
            set => SetValue(BackgroundMouseOverProperty, value);
        }

        public Brush BackgroundSelected
        {
            get => (Brush)GetValue(BackgroundSelectedProperty);
            set => SetValue(BackgroundSelectedProperty, value);
        }

        public Brush BorderBrushMouseOver
        {
            get => (Brush)GetValue(BorderBrushMouseOverProperty);
            set => SetValue(BorderBrushMouseOverProperty, value);
        }

        public Brush BorderBrushSelected
        {
            get => (Brush)GetValue(BorderBrushSelectedProperty);
            set => SetValue(BorderBrushSelectedProperty, value);
        }

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public Brush DropDownBackground
        {
            get => (Brush)GetValue(DropDownBackgroundProperty);
            set => SetValue(DropDownBackgroundProperty, value);
        }

        public Brush DropDownBorderBrush
        {
            get => (Brush)GetValue(DropDownBorderBrushProperty);
            set => SetValue(DropDownBorderBrushProperty, value);
        }

        public Thickness DropDownBorderThickness
        {
            get => (Thickness)GetValue(DropDownBorderThicknessProperty);
            set => SetValue(DropDownBorderThicknessProperty, value);
        }

        public CornerRadius DropDownCornerRadius
        {
            get => (CornerRadius)GetValue(DropDownCornerRadiusProperty);
            set => SetValue(DropDownCornerRadiusProperty, value);
        }

        public Brush DropDownForeground
        {
            get => (Brush)GetValue(DropDownForegroundProperty);
            set => SetValue(DropDownForegroundProperty, value);
        }

        public Brush ForegroundMouseOver
        {
            get => (Brush)GetValue(ForegroundMouseOverProperty);
            set => SetValue(ForegroundMouseOverProperty, value);
        }

        public Brush ForegroundSelected
        {
            get => (Brush)GetValue(ForegroundSelectedProperty);
            set => SetValue(ForegroundSelectedProperty, value);
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Static CustomComboBox class constructor. </summary>
        static CustomComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomComboBox),
                new FrameworkPropertyMetadata(typeof(CustomComboBox)));
        }

        #endregion CLASS METHODS

        #region ITEMS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Creates and returns new CustomComboBoxItem container. </summary>
        /// <returns> A new CustomComboBoxItem control. </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new CustomComboBoxItem();
        }

        #endregion ITEMS METHODS

    }
}
