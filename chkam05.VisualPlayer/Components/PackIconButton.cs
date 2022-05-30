using chkam05.VisualPlayer.Components.Data;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace chkam05.VisualPlayer.Components
{
    public class PackIconButton : Button, INotifyPropertyChanged
    {

        //  DEPENDENCY PROPERTIES

        public static readonly DependencyProperty HoveredBackgroundProperty = DependencyProperty.Register(
            nameof(HoveredBackground),
            typeof(SolidColorBrush),
            typeof(PackIconButton),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(244, 0, 120, 215))));

        public static readonly DependencyProperty HoveredBorderProperty = DependencyProperty.Register(
            nameof(HoveredBorder),
            typeof(SolidColorBrush),
            typeof(PackIconButton),
            new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        public static readonly DependencyProperty PressedBackgroundProperty = DependencyProperty.Register(
            nameof(PressedBackground),
            typeof(SolidColorBrush),
            typeof(PackIconButton),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(128, 0, 0, 0))));

        public static readonly DependencyProperty PressedBorderProperty = DependencyProperty.Register(
            nameof(PressedBorder),
            typeof(SolidColorBrush),
            typeof(PackIconButton),
            new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        public static readonly DependencyProperty ContentSideProperty = DependencyProperty.Register(
            nameof(ContentSide),
            typeof(ContentSide),
            typeof(PackIconButton),
            new PropertyMetadata(ContentSide.BOTTOM));

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            nameof(CornerRadius),
            typeof(CornerRadius),
            typeof(PackIconButton),
            new PropertyMetadata(new CornerRadius(4)));

        public static readonly DependencyProperty PackIconHeightProperty = DependencyProperty.Register(
            nameof(PackIconHeight),
            typeof(double),
            typeof(PackIconButton),
            new PropertyMetadata(double.NaN));

        public static readonly DependencyProperty PackIconKindProperty = DependencyProperty.Register(
            nameof(PackIconKind),
            typeof(PackIconKind),
            typeof(PackIconButton),
            new PropertyMetadata(PackIconKind.None));

        public static readonly DependencyProperty PackIconMarginProperty = DependencyProperty.Register(
            nameof(PackIconMargin),
            typeof(Thickness),
            typeof(PackIconButton),
            new PropertyMetadata(new Thickness(4)));

        public static readonly DependencyProperty PackIconWidthProperty = DependencyProperty.Register(
            nameof(PackIconWidth),
            typeof(double),
            typeof(PackIconButton),
            new PropertyMetadata(double.NaN));


        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  GETTERS & SETTERS

        public SolidColorBrush HoveredBackground
        {
            get => (SolidColorBrush)GetValue(HoveredBackgroundProperty);
            set
            {
                SetValue(HoveredBackgroundProperty, value);
                OnPropertyChanged(nameof(HoveredBackground));
            }
        }

        public SolidColorBrush HoveredBorder
        {
            get => (SolidColorBrush)GetValue(HoveredBorderProperty);
            set
            {
                SetValue(HoveredBorderProperty, value);
                OnPropertyChanged(nameof(HoveredBorder));
            }
        }

        public SolidColorBrush PressedBackground
        {
            get => (SolidColorBrush)GetValue(PressedBackgroundProperty);
            set
            {
                SetValue(PressedBackgroundProperty, value);
                OnPropertyChanged(nameof(PressedBackground));
            }
        }

        public SolidColorBrush PressedBorder
        {
            get => (SolidColorBrush)GetValue(PressedBorderProperty);
            set
            {
                SetValue(PressedBorderProperty, value);
                OnPropertyChanged(nameof(PressedBorder));
            }
        }

        public ContentSide ContentSide
        {
            get => (ContentSide)GetValue(ContentSideProperty);
            set
            {
                SetValue(ContentSideProperty, value);
                OnPropertyChanged(nameof(ContentSide));
            }
        }

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set
            {
                SetValue(CornerRadiusProperty, value);
                OnPropertyChanged(nameof(CornerRadius));
            }
        }

        public double PackIconHeight
        {
            get => (double)GetValue(PackIconHeightProperty);
            set
            {
                SetValue(PackIconHeightProperty, value);
                OnPropertyChanged(nameof(PackIconHeight));
            }
        }

        public PackIconKind PackIconKind
        {
            get => (PackIconKind)GetValue(PackIconKindProperty);
            set
            {
                SetValue(PackIconKindProperty, value);
                OnPropertyChanged(nameof(PackIconKind));
            }
        }

        public Thickness PackIconMargin
        {
            get => (Thickness)GetValue(PackIconMarginProperty);
            set
            {
                SetValue(PackIconMarginProperty, value);
                OnPropertyChanged(nameof(PackIconMargin));
            }
        }

        public double PackIconWidth
        {
            get => (double)GetValue(PackIconWidthProperty);
            set
            {
                SetValue(PackIconWidthProperty, value);
                OnPropertyChanged(nameof(PackIconWidth));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> PackIconButton static class constructor. </summary>
        static PackIconButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(PackIconButton),
                new FrameworkPropertyMetadata(typeof(PackIconButton)));
        }

        #endregion CLASS METHODS

        #region NOTIFY PROPERTIES CHANGED INTERFACE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method for invoking PropertyChangedEventHandler external method. </summary>
        /// <param name="propertyName"> Changed property name. </param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion NOTIFY PROPERTIES CHANGED INTERFACE METHODS

    }
}
