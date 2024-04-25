using chkam05.VisualPlayer.Data.Config;
using chkam05.VisualPlayer.Data.Configuration;
using MaterialDesignThemes.Wpf;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace chkam05.VisualPlayer.Controls
{
    public partial class CoverImage : UserControl, INotifyPropertyChanged
    {

        //  DEPENDENCY PROPERTIES

        public static readonly DependencyProperty CoverProperty = DependencyProperty.Register(
            nameof(Cover),
            typeof(ImageSource),
            typeof(CoverImage),
            new PropertyMetadata(null));

        public static readonly DependencyProperty DefaultIconKindProperty = DependencyProperty.Register(
            nameof(DefaultIconKind),
            typeof(PackIconKind),
            typeof(CoverImage),
            new PropertyMetadata(PackIconKind.MusicNote));


        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        public ConfigManager ConfigManager { get; private set; }


        //  GETTERS & SETTERS

        public ImageSource Cover
        {
            get => (ImageSource)GetValue(CoverProperty);
            set
            {
                SetValue(CoverProperty, value);
                ShowCoverIcon(value == null);
                OnPropertyChanged(nameof(Cover));
            }
        }

        public PackIconKind DefaultIconKind
        {
            get => (PackIconKind)GetValue(DefaultIconKindProperty);
            set
            {
                SetValue(DefaultIconKindProperty, value);
                OnPropertyChanged(nameof(DefaultIconKind));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> CoverImage class constructor. </summary>
        public CoverImage()
        {
            //  Setup modules.
            ConfigManager = ConfigManager.Instance;

            //  Initialize interface and components.
            InitializeComponent();
        }

        #endregion CLASS METHODS

        #region ICON MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Show/hide CoverIcon components. </summary>
        /// <param name="visibility"> Visibility option. </param>
        private void ShowCoverIcon(bool visibility = true)
        {
            CoverIcon.Visibility = visibility ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion ICON MANAGEMENT METHODS

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
