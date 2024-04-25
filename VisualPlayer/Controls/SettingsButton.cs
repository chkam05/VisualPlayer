using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VisualPlayer.Data.ColorModels;
using VisualPlayer.Data.Enums;

namespace VisualPlayer.Controls
{
    public class SettingsButton : Button
    {

        //  DEPENDENCY PROPERTIES

        public static readonly DependencyProperty BackgroundMouseOverProperty = DependencyProperty.Register(
            nameof(BackgroundMouseOver),
            typeof(Brush),
            typeof(SettingsButton),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultAccentColorMouseOver)));

        public static readonly DependencyProperty BackgroundPressedProperty = DependencyProperty.Register(
            nameof(BackgroundPressed),
            typeof(Brush),
            typeof(SettingsButton),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultAccentColorPressed)));

        public static readonly DependencyProperty BorderBrushMouseOverProperty = DependencyProperty.Register(
            nameof(BorderBrushMouseOver),
            typeof(Brush),
            typeof(SettingsButton),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultAccentColor)));

        public static readonly DependencyProperty BorderBrushPressedProperty = DependencyProperty.Register(
            nameof(BorderBrushPressed),
            typeof(Brush),
            typeof(SettingsButton),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultAccentColor)));

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            nameof(CornerRadius),
            typeof(CornerRadius),
            typeof(SettingsButton),
            new PropertyMetadata(new CornerRadius(4)));

        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
            nameof(Description),
            typeof(string),
            typeof(SettingsButton),
            new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty ForegroundMouseOverProperty = DependencyProperty.Register(
            nameof(ForegroundMouseOver),
            typeof(Brush),
            typeof(SettingsButton),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultAccentColorForeground)));

        public static readonly DependencyProperty ForegroundPressedProperty = DependencyProperty.Register(
            nameof(ForegroundPressed),
            typeof(Brush),
            typeof(SettingsButton),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultAccentColorForeground)));

        public static readonly DependencyProperty IconHeightProperty = DependencyProperty.Register(
            nameof(IconHeight),
            typeof(double),
            typeof(SettingsButton),
            new PropertyMetadata(24d));

        public static readonly DependencyProperty IconKindProperty = DependencyProperty.Register(
            nameof(IconKind),
            typeof(PackIconKind),
            typeof(SettingsButton),
            new PropertyMetadata(PackIconKind.None));

        public static readonly DependencyProperty IconWidthProperty = DependencyProperty.Register(
            nameof(IconWidth),
            typeof(double),
            typeof(SettingsButton),
            new PropertyMetadata(24d));

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            nameof(Title),
            typeof(string),
            typeof(SettingsButton),
            new PropertyMetadata(string.Empty));


        //  GETTERS & SETTERS

        public Brush BackgroundMouseOver
        {
            get => (Brush)GetValue(BackgroundMouseOverProperty);
            set => SetValue(BackgroundMouseOverProperty, value);
        }

        public Brush BackgroundPressed
        {
            get => (Brush)GetValue(BackgroundPressedProperty);
            set => SetValue(BackgroundPressedProperty, value);
        }

        public Brush BorderBrushMouseOver
        {
            get => (Brush)GetValue(BorderBrushMouseOverProperty);
            set => SetValue(BorderBrushMouseOverProperty, value);
        }

        public Brush BorderBrushPressed
        {
            get => (Brush)GetValue(BorderBrushPressedProperty);
            set => SetValue(BorderBrushPressedProperty, value);
        }

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        public Brush ForegroundMouseOver
        {
            get => (Brush)GetValue(ForegroundMouseOverProperty);
            set => SetValue(ForegroundMouseOverProperty, value);
        }

        public Brush ForegroundPressed
        {
            get => (Brush)GetValue(ForegroundPressedProperty);
            set => SetValue(ForegroundPressedProperty, value);
        }

        public double IconHeight
        {
            get => (double)GetValue(IconHeightProperty);
            set => SetValue(IconHeightProperty, value);
        }

        public PackIconKind IconKind
        {
            get => (PackIconKind)GetValue(IconKindProperty);
            set => SetValue(IconKindProperty, value);
        }

        public double IconWidth
        {
            get => (double)GetValue(IconWidthProperty);
            set => SetValue(IconWidthProperty, value);
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Static SettingsButton class constructor. </summary>
        static SettingsButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SettingsButton),
                new FrameworkPropertyMetadata(typeof(SettingsButton)));
        }

        #endregion CLASS METHODS

    }
}
