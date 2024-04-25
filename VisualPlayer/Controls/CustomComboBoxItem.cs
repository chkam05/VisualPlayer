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
    public class CustomComboBoxItem : ComboBoxItem
    {

        //  DEPENDENCY PROPERTIES

        public static readonly DependencyProperty BackgroundMouseOverProperty = DependencyProperty.Register(
            nameof(BackgroundMouseOver),
            typeof(Brush),
            typeof(CustomComboBoxItem),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultAccentColorMouseOver)));

        public static readonly DependencyProperty BackgroundSelectedProperty = DependencyProperty.Register(
            nameof(BackgroundSelected),
            typeof(Brush),
            typeof(CustomComboBoxItem),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultAccentColorSelected)));

        public static readonly DependencyProperty BorderBrushMouseOverProperty = DependencyProperty.Register(
            nameof(BorderBrushMouseOver),
            typeof(Brush),
            typeof(CustomComboBoxItem),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultAccentColor)));

        public static readonly DependencyProperty BorderBrushSelectedProperty = DependencyProperty.Register(
            nameof(BorderBrushSelected),
            typeof(Brush),
            typeof(CustomComboBoxItem),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultAccentColor)));

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            nameof(CornerRadius),
            typeof(CornerRadius),
            typeof(CustomComboBoxItem),
            new PropertyMetadata(new CornerRadius(4)));

        public static readonly DependencyProperty ForegroundMouseOverProperty = DependencyProperty.Register(
            nameof(ForegroundMouseOver),
            typeof(Brush),
            typeof(CustomComboBoxItem),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultAccentColorForeground)));

        public static readonly DependencyProperty ForegroundSelectedProperty = DependencyProperty.Register(
            nameof(ForegroundSelected),
            typeof(Brush),
            typeof(CustomComboBoxItem),
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
        /// <summary> Static CustomComboBoxItem class constructor. </summary>
        static CustomComboBoxItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomComboBoxItem),
                new FrameworkPropertyMetadata(typeof(CustomComboBoxItem)));
        }

        #endregion CLASS METHODS

    }
}
