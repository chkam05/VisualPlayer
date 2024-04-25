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
    public class CustomListViewItem : ListViewItem
    {

        //  DEPENDENCY PROPERTIES

        public static readonly DependencyProperty BackgroundMouseOverProperty = DependencyProperty.Register(
            nameof(BackgroundMouseOver),
            typeof(Brush),
            typeof(CustomListViewItem),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultAccentColorMouseOver)));

        public static readonly DependencyProperty BackgroundSelectedProperty = DependencyProperty.Register(
            nameof(BackgroundSelected),
            typeof(Brush),
            typeof(CustomListViewItem),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultAccentColorSelected)));

        public static readonly DependencyProperty BackgroundSelectedInactiveProperty = DependencyProperty.Register(
            nameof(BackgroundSelectedInactive),
            typeof(Brush),
            typeof(CustomListViewItem),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultAccentColorSelected)));

        public static readonly DependencyProperty BorderBrushMouseOverProperty = DependencyProperty.Register(
            nameof(BorderBrushMouseOver),
            typeof(Brush),
            typeof(CustomListViewItem),
            new PropertyMetadata(null));

        public static readonly DependencyProperty BorderBrushSelectedProperty = DependencyProperty.Register(
            nameof(BorderBrushSelected),
            typeof(Brush),
            typeof(CustomListViewItem),
            new PropertyMetadata(null));

        public static readonly DependencyProperty BorderBrushSelectedInactiveProperty = DependencyProperty.Register(
            nameof(BorderBrushSelectedInactive),
            typeof(Brush),
            typeof(CustomListViewItem),
            new PropertyMetadata(null));

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            nameof(CornerRadius),
            typeof(CornerRadius),
            typeof(CustomListViewItem),
            new PropertyMetadata(new CornerRadius(4)));

        public static readonly DependencyProperty ForegroundMouseOverProperty = DependencyProperty.Register(
            nameof(ForegroundMouseOver),
            typeof(Brush),
            typeof(CustomListViewItem),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultAccentColorForeground)));

        public static readonly DependencyProperty ForegroundSelectedProperty = DependencyProperty.Register(
            nameof(ForegroundSelected),
            typeof(Brush),
            typeof(CustomListViewItem),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultAccentColorForeground)));

        public static readonly DependencyProperty ForegroundSelectedInactiveProperty = DependencyProperty.Register(
            nameof(ForegroundSelectedInactive),
            typeof(Brush),
            typeof(CustomListViewItem),
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

        public Brush BackgroundSelectedInactive
        {
            get => (Brush)GetValue(BackgroundSelectedInactiveProperty);
            set => SetValue(BackgroundSelectedInactiveProperty, value);
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

        public Brush BorderBrushSelectedInactive
        {
            get => (Brush)GetValue(BorderBrushSelectedInactiveProperty);
            set => SetValue(BorderBrushSelectedInactiveProperty, value);
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

        public Brush ForegroundSelectedInactive
        {
            get => (Brush)GetValue(ForegroundSelectedInactiveProperty);
            set => SetValue(ForegroundSelectedInactiveProperty, value);
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> CustomListViewItem static class constructor. </summary>
        static CustomListViewItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomListViewItem),
                new FrameworkPropertyMetadata(typeof(CustomListViewItem)));
        }

        #endregion CLASS METHODS

    }
}
