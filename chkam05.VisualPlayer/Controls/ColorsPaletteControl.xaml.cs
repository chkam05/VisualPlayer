using chkam05.VisualPlayer.Components;
using chkam05.VisualPlayer.Controls.Events;
using chkam05.VisualPlayer.Data.Config;
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
    public partial class ColorsPaletteControl : UserControl, INotifyPropertyChanged
    {

        //  DEPENDENCY PROPERTIES

        public static readonly DependencyProperty PaletteBackgroundProperty = DependencyProperty.Register(
            nameof(PaletteBackground),
            typeof(SolidColorBrush),
            typeof(ColorsPaletteControl),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(128, 0, 0, 0))));

        public static readonly DependencyProperty PaletteItemHoveredColorBrushProperty = DependencyProperty.Register(
            nameof(PaletteItemHoveredColorBrush),
            typeof(SolidColorBrush),
            typeof(ColorsPaletteControl),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(244, 0, 120, 215))));

        public static readonly DependencyProperty PaletteItemPressedColorBrushProperty = DependencyProperty.Register(
            nameof(PaletteItemPressedColorBrush),
            typeof(SolidColorBrush),
            typeof(ColorsPaletteControl),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 0, 120, 215))));

        public static readonly DependencyProperty PaletteItemSelectedInactiveColorBrushProperty = DependencyProperty.Register(
            nameof(PaletteItemSelectedInactiveColorBrush),
            typeof(SolidColorBrush),
            typeof(ColorsPaletteControl),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(64, 255, 255, 255))));


        public static readonly DependencyProperty PaletteColorsTitleProperty = DependencyProperty.Register(
            nameof(PaletteColorsTitle),
            typeof(string),
            typeof(ColorsPaletteControl),
            new PropertyMetadata("Palette colors:"));

        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register(
            nameof(SelectedColor),
            typeof(Color),
            typeof(ColorsPaletteControl),
            new PropertyMetadata(Color.FromArgb(255, 0, 120, 215)));

        public static readonly DependencyProperty UsedColorsTitleProperty = DependencyProperty.Register(
            nameof(UsedColorsTitle),
            typeof(string),
            typeof(ColorsPaletteControl),
            new PropertyMetadata("Recently used colors:"));

        public static readonly DependencyProperty UsedColorsProperty = DependencyProperty.Register(
            nameof(UsedColors),
            typeof(ObservableCollection<ColorInfo>),
            typeof(ColorsPaletteControl),
            new PropertyMetadata(new ObservableCollection<ColorInfo>()));


        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<PaletteColorUpdateEventArgs> OnColorUpdate;


        //  VARIABLES

        private ObservableCollection<ColorInfo> _paletteColors;


        //  GETTERS & SETTERS

        #region Theme

        public SolidColorBrush PaletteBackground
        {
            get => (SolidColorBrush)GetValue(PaletteBackgroundProperty);
            set
            {
                SetValue(PaletteBackgroundProperty, value);
                OnPropertyChanged(nameof(PaletteBackground));
            }
        }

        public SolidColorBrush PaletteItemHoveredColorBrush
        {
            get => (SolidColorBrush)GetValue(PaletteItemHoveredColorBrushProperty);
            set
            {
                SetValue(PaletteItemHoveredColorBrushProperty, value);
                OnPropertyChanged(nameof(PaletteItemHoveredColorBrush));
            }
        }

        public SolidColorBrush PaletteItemPressedColorBrush
        {
            get => (SolidColorBrush)GetValue(PaletteItemPressedColorBrushProperty);
            set
            {
                SetValue(PaletteItemPressedColorBrushProperty, value);
                OnPropertyChanged(nameof(PaletteItemPressedColorBrush));
            }
        }

        public SolidColorBrush PaletteItemSelectedInactiveColorBrush
        {
            get => (SolidColorBrush)GetValue(PaletteItemSelectedInactiveColorBrushProperty);
            set
            {
                SetValue(PaletteItemSelectedInactiveColorBrushProperty, value);
                OnPropertyChanged(nameof(PaletteItemSelectedInactiveColorBrush));
            }
        }

        #endregion Theme

        public ObservableCollection<ColorInfo> PaletteColors
        {
            get => _paletteColors;
            set
            {
                _paletteColors = value;
                OnPropertyChanged(nameof(PaletteColors));
            }
        }

        public string PaletteColorsTitle
        {
            get => (string)GetValue(PaletteColorsTitleProperty);
            set
            {
                SetValue(PaletteColorsTitleProperty, value);
                OnPropertyChanged(nameof(PaletteColorsTitle));
            }
        }

        public string UsedColorsTitle
        {
            get => (string)GetValue(UsedColorsTitleProperty);
            set
            {
                SetValue(UsedColorsTitleProperty, value);
                OnPropertyChanged(nameof(UsedColorsTitle));
            }
        }

        public Color SelectedColor
        {
            get => (Color)GetValue(SelectedColorProperty);
            set
            {
                SetValue(SelectedColorProperty, value);
                OnPropertyChanged(nameof(SelectedColor));
            }
        }

        public ObservableCollection<ColorInfo> UsedColors
        {
            get => (ObservableCollection<ColorInfo>)GetValue(UsedColorsProperty);
            set
            {
                SetValue(UsedColorsProperty, value);
                OnPropertyChanged(nameof(UsedColors));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> ColorsPaletteControl class constructor. </summary>
        public ColorsPaletteControl()
        {
            //  Setup Data Containers.
            SetupDataContainers();

            //  Initialize interface and components.
            InitializeComponent();
        }

        #endregion CLASS METHODS

        #region COLORS SELECTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Add color to used colors collection. </summary>
        /// <param name="colorInfo"> Color info. </param>
        private void AddColor(ColorInfo colorInfo)
        {
            if (UsedColors.Any(c => c.Color == colorInfo.Color))
                UsedColors.Remove(UsedColors.Last(c => c.Color == colorInfo.Color));

            while (UsedColors.Count >= 5)
                UsedColors.Remove(UsedColors.Last());

            UsedColors.Insert(0, colorInfo);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after selecting color in CustomColors/PaletteColors ExtendedListView. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Selection Changed Event Arguments. </param>
        private void ColorsExtendedListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listView = (ExtendedListView)sender;
            var selectedItem = listView.SelectedItem;

            if (selectedItem != null)
            {
                var colorInfo = selectedItem as ColorInfo;

                if (colorInfo != null)
                {
                    AddColor(colorInfo);
                    SelectedColor = colorInfo.Color;
                    OnColorUpdate?.Invoke(this, new PaletteColorUpdateEventArgs(colorInfo));
                }

                listView.SelectedIndex = -1;
                listView.SelectedItem = null;
            }
        }

        #endregion COLORS SELECTION METHODS

        #region INTERFACE INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after moving scroll in component. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Mouse Wheel Event Arguments. </param>
        private void ExtendedListView_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = true;
                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                eventArg.RoutedEvent = UIElement.MouseWheelEvent;
                eventArg.Source = sender;
                var parent = ((Control)sender).Parent as UIElement;
                parent.RaiseEvent(eventArg);
            }
        }

        #endregion INTERFACE INTERACTION METHODS

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
            PaletteColors = new ObservableCollection<ColorInfo>(
                ColorUtilities.StaticColors.OrderByDescending(o => o.ColorCode));
        }

        #endregion SETUP METHODS

    }
}
