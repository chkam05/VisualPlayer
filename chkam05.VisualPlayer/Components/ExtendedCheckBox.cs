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
    public class ExtendedCheckBox : CheckBox, INotifyPropertyChanged
    {

        //  DEPENDENCY PROPERTIES

        public static readonly DependencyProperty HoveredGlyphProperty = DependencyProperty.Register(
            nameof(HoveredGlyph),
            typeof(SolidColorBrush),
            typeof(ExtendedCheckBox),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(192, 255, 255, 255))));

        public static readonly DependencyProperty PressedGlyphProperty = DependencyProperty.Register(
            nameof(PressedGlyph),
            typeof(SolidColorBrush),
            typeof(ExtendedCheckBox),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(128, 255, 255, 255))));

        public static readonly DependencyProperty GlyphColorProperty = DependencyProperty.Register(
            nameof(GlyphColor),
            typeof(SolidColorBrush),
            typeof(ExtendedCheckBox),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 0, 120, 215))));


        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  GETTERS & SETTERS

        public SolidColorBrush HoveredGlyph
        {
            get => (SolidColorBrush)GetValue(HoveredGlyphProperty);
            set
            {
                SetValue(HoveredGlyphProperty, value);
                OnPropertyChanged(nameof(HoveredGlyph));
            }
        }

        public SolidColorBrush PressedGlyph
        {
            get => (SolidColorBrush)GetValue(PressedGlyphProperty);
            set
            {
                SetValue(PressedGlyphProperty, value);
                OnPropertyChanged(nameof(PressedGlyph));
            }
        }

        public SolidColorBrush GlyphColor
        {
            get => (SolidColorBrush)GetValue(GlyphColorProperty);
            set
            {
                SetValue(GlyphColorProperty, value);
                OnPropertyChanged(nameof(GlyphColor));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> ExtendedCheckBox static class constructor. </summary>
        static ExtendedCheckBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ExtendedCheckBox),
                new FrameworkPropertyMetadata(typeof(ExtendedCheckBox)));
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
