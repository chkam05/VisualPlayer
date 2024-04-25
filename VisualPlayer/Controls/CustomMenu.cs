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
    public class CustomMenu : Menu
    {

        //  DEPENDENCY PROPERTIES

        public static readonly DependencyProperty SubMenuBackgroundProperty = DependencyProperty.Register(
            nameof(SubMenuBackground),
            typeof(Brush),
            typeof(CustomMenu),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultThemeColor)));

        public static readonly DependencyProperty SubMenuBorderBrushProperty = DependencyProperty.Register(
            nameof(SubMenuBorderBrush),
            typeof(Brush),
            typeof(CustomMenu),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultAccentColor)));

        public static readonly DependencyProperty SubMenuBorderThicknessProperty = DependencyProperty.Register(
            nameof(SubMenuBorderThickness),
            typeof(Thickness),
            typeof(CustomMenu),
            new PropertyMetadata(new Thickness(1)));

        public static readonly DependencyProperty SubMenuCornerRadiusProperty = DependencyProperty.Register(
            nameof(SubMenuCornerRadius),
            typeof(CornerRadius),
            typeof(CustomMenu),
            new PropertyMetadata(new CornerRadius(4)));

        public static readonly DependencyProperty SubMenuPaddingProperty = DependencyProperty.Register(
            nameof(SubMenuPadding),
            typeof(Thickness),
            typeof(CustomMenu),
            new PropertyMetadata(new Thickness(2, 1, 2, 1)));


        //  GETTERS & SETTERS

        public Brush SubMenuBackground
        {
            get => (Brush)GetValue(SubMenuBackgroundProperty);
            set => SetValue(SubMenuBackgroundProperty, value);
        }

        public Brush SubMenuBorderBrush
        {
            get => (Brush)GetValue(SubMenuBorderBrushProperty);
            set => SetValue(SubMenuBorderBrushProperty, value);
        }

        public Thickness SubMenuBorderThickness
        {
            get => (Thickness)GetValue(SubMenuBorderThicknessProperty);
            set => SetValue(SubMenuBorderThicknessProperty, value);
        }

        public CornerRadius SubMenuCornerRadius
        {
            get => (CornerRadius)GetValue(SubMenuCornerRadiusProperty);
            set => SetValue(SubMenuCornerRadiusProperty, value);
        }

        public Thickness SubMenuPadding
        {
            get => (Thickness)GetValue(SubMenuPaddingProperty);
            set => SetValue(SubMenuPaddingProperty, value);
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Static CustomMenu class constructor. </summary>
        static CustomMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomMenu),
                new FrameworkPropertyMetadata(typeof(CustomMenu)));
        }

        #endregion CLASS METHODS

        #region ITEMS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Creates and returns new CustomMenuItem container. </summary>
        /// <returns> A new CustomMenuItem control. </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new CustomMenuItem();
        }

        #endregion ITEMS METHODS

    }
}
