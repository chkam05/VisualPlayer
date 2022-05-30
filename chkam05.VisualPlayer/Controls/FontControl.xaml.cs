using chkam05.VisualPlayer.Data.Config;
using chkam05.VisualPlayer.Data.Fonts;
using chkam05.VisualPlayer.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace chkam05.VisualPlayer.Controls
{
    public partial class FontControl : UserControl, INotifyPropertyChanged
    {

        //  DEPENDENCY PROPERTIES

        #region Appearance

        public static readonly DependencyProperty AccentColorBrushProperty = DependencyProperty.Register(
            nameof(AccentColorBrush),
            typeof(SolidColorBrush),
            typeof(FontControl),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 0, 120, 215))));

        public static readonly DependencyProperty AccentHoveredColorBrushProperty = DependencyProperty.Register(
            nameof(AccentHoveredColorBrush),
            typeof(SolidColorBrush),
            typeof(FontControl),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(244, 0, 120, 215))));

        public static readonly DependencyProperty AccentSelectedColorBrushProperty = DependencyProperty.Register(
            nameof(AccentSelectedColorBrush),
            typeof(SolidColorBrush),
            typeof(FontControl),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 0, 120, 215))));

        public static readonly DependencyProperty ControlBackgroundProperty = DependencyProperty.Register(
            nameof(ControlBackground),
            typeof(SolidColorBrush),
            typeof(FontControl),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(128, 0, 0, 0))));

        public static readonly DependencyProperty HoveredColorBrushProperty = DependencyProperty.Register(
            nameof(HoveredColorBrush),
            typeof(SolidColorBrush),
            typeof(FontControl),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(192, 255, 255, 255))));

        public static readonly DependencyProperty PressedColorBrushProperty = DependencyProperty.Register(
            nameof(PressedColorBrush),
            typeof(SolidColorBrush),
            typeof(FontControl),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(64, 255, 255, 255))));

        #endregion Appearance

        public static readonly DependencyProperty SelectedFontProperty = DependencyProperty.Register(
            nameof(SelectedFont),
            typeof(FontContainer),
            typeof(FontControl),
            new PropertyMetadata(FontsManager.Instance.GetFontByName("Segoe UI")));

        public static readonly DependencyProperty SelectedFontSizeProperty = DependencyProperty.Register(
            nameof(SelectedFontSize),
            typeof(int),
            typeof(FontControl),
            new PropertyMetadata(12));

        public static readonly DependencyProperty SelectedFontStyleProperty = DependencyProperty.Register(
            nameof(SelectedFontStyle),
            typeof(FontStyle),
            typeof(FontControl),
            new PropertyMetadata(System.Windows.FontStyles.Normal));

        public static readonly DependencyProperty SelectedFontStretchProperty = DependencyProperty.Register(
            nameof(SelectedFontStretch),
            typeof(FontStretch),
            typeof(FontControl),
            new PropertyMetadata(System.Windows.FontStretches.Normal));

        public static readonly DependencyProperty SelectedFontWeightProperty = DependencyProperty.Register(
            nameof(SelectedFontWeight),
            typeof(FontWeight),
            typeof(FontControl),
            new PropertyMetadata(System.Windows.FontWeights.Normal));


        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private ObservableCollection<FontContainer> _fonts;
        private ObservableCollection<FontStyle> _fontStyles;
        private ObservableCollection<FontStretch> _fontStretches;
        private ObservableCollection<FontWeight> _fontWeights;


        //  GETTERS & SETTERS

        #region Appearance

        public SolidColorBrush AccentColorBrush
        {
            get => (SolidColorBrush)GetValue(AccentColorBrushProperty);
            set
            {
                SetValue(AccentColorBrushProperty, value);
                OnPropertyChanged(nameof(AccentColorBrush));
            }
        }

        public SolidColorBrush AccentHoveredColorBrush
        {
            get => (SolidColorBrush)GetValue(AccentHoveredColorBrushProperty);
            set
            {
                SetValue(AccentHoveredColorBrushProperty, value);
                OnPropertyChanged(nameof(AccentHoveredColorBrush));
            }
        }

        public SolidColorBrush AccentSelectedColorBrush
        {
            get => (SolidColorBrush)GetValue(AccentSelectedColorBrushProperty);
            set
            {
                SetValue(AccentSelectedColorBrushProperty, value);
                OnPropertyChanged(nameof(AccentSelectedColorBrush));
            }
        }

        public SolidColorBrush ControlBackground
        {
            get => (SolidColorBrush)GetValue(ControlBackgroundProperty);
            set
            {
                SetValue(ControlBackgroundProperty, value);
                OnPropertyChanged(nameof(ControlBackground));
            }
        }

        public SolidColorBrush HoveredColorBrush
        {
            get => (SolidColorBrush)GetValue(HoveredColorBrushProperty);
            set
            {
                SetValue(HoveredColorBrushProperty, value);
                OnPropertyChanged(nameof(HoveredColorBrush));
            }
        }

        public SolidColorBrush PressedColorBrush
        {
            get => (SolidColorBrush)GetValue(PressedColorBrushProperty);
            set
            {
                SetValue(PressedColorBrushProperty, value);
                OnPropertyChanged(nameof(PressedColorBrush));
            }
        }

        #endregion Appearance

        public ObservableCollection<FontContainer> Fonts
        {
            get => _fonts;
            private set
            {
                _fonts = value;
                OnPropertyChanged(nameof(Fonts));
            }
        }

        public ObservableCollection<FontStyle> FontStyles
        {
            get => _fontStyles;
            private set
            {
                _fontStyles = value;
                OnPropertyChanged(nameof(FontStyles));
            }
        }

        public ObservableCollection<FontStretch> FontStretches
        {
            get => _fontStretches;
            private set
            {
                _fontStretches = value;
                OnPropertyChanged(nameof(FontStretches));
            }
        }

        public ObservableCollection<FontWeight> FontWeights
        {
            get => _fontWeights;
            private set
            {
                _fontWeights = value;
                OnPropertyChanged(nameof(FontWeights));
            }
        }

        public FontContainer SelectedFont
        {
            get => (FontContainer)GetValue(SelectedFontProperty);
            set
            {
                SetValue(SelectedFontProperty, value);
                OnPropertyChanged(nameof(SelectedFont));
            }
        }

        public int SelectedFontSize
        {
            get => (int)GetValue(SelectedFontSizeProperty);
            set
            {
                SetValue(SelectedFontSizeProperty, value);
                OnPropertyChanged(nameof(SelectedFontSize));
            }
        }

        public FontStyle SelectedFontStyle
        {
            get => (FontStyle)GetValue(SelectedFontStyleProperty);
            set
            {
                SetValue(SelectedFontStyleProperty, value);
                OnPropertyChanged(nameof(SelectedFontStyle));
            }
        }

        public FontStretch SelectedFontStretch
        {
            get => (FontStretch)GetValue(SelectedFontStretchProperty);
            set
            {
                SetValue(SelectedFontStretchProperty, value);
                OnPropertyChanged(nameof(SelectedFontStretch));
            }
        }

        public FontWeight SelectedFontWeight
        {
            get => (FontWeight)GetValue(SelectedFontWeightProperty);
            set
            {
                SetValue(SelectedFontWeightProperty, value);
                OnPropertyChanged(nameof(SelectedFontWeight));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> FontControl class constructor. </summary>
        public FontControl()
        {
            //  Setup Data Containers.
            SetupDataContainers();

            //  Initialize interface and components.
            InitializeComponent();
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

        #region SETUP METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Setup data containers. </summary>
        private void SetupDataContainers()
        {
            Fonts = FontsManager.Instance.Fonts;

            FontStyles = new ObservableCollection<FontStyle>()
            {
                System.Windows.FontStyles.Normal,
                System.Windows.FontStyles.Italic,
                System.Windows.FontStyles.Oblique
            };

            FontStretches = new ObservableCollection<FontStretch>()
            {
                System.Windows.FontStretches.Normal,
                System.Windows.FontStretches.Condensed,
                System.Windows.FontStretches.Expanded,
                System.Windows.FontStretches.ExtraCondensed,
                System.Windows.FontStretches.ExtraExpanded,
                //System.Windows.FontStretches.Medium,
                System.Windows.FontStretches.SemiCondensed,
                System.Windows.FontStretches.SemiExpanded, 
                System.Windows.FontStretches.UltraExpanded
            };

            FontWeights = new ObservableCollection<FontWeight>()
            {
                System.Windows.FontWeights.Normal,
                //System.Windows.FontWeights.Regular,
                System.Windows.FontWeights.Black,
                System.Windows.FontWeights.Bold,
                //System.Windows.FontWeights.DemiBold,
                System.Windows.FontWeights.ExtraBlack,
                System.Windows.FontWeights.ExtraBold,
                System.Windows.FontWeights.ExtraLight,
                //System.Windows.FontWeights.Heavy,
                System.Windows.FontWeights.Light,
                System.Windows.FontWeights.Medium,
                System.Windows.FontWeights.SemiBold,
                System.Windows.FontWeights.Thin,
                //System.Windows.FontWeights.UltraBlack,
                //System.Windows.FontWeights.UltraBold,
                //System.Windows.FontWeights.UltraLight,
            };
        }

        #endregion SETUP METHODS

    }
}
