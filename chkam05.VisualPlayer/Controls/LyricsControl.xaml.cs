using chkam05.VisualPlayer.Data.Config;
using chkam05.VisualPlayer.Data.Configuration;
using chkam05.VisualPlayer.Data.Lyrics;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;


namespace chkam05.VisualPlayer.Controls
{
    public partial class LyricsControl : UserControl, ILyricsController, INotifyPropertyChanged
    {

        //  DEPENDENCY PROPERTIES

        public static readonly DependencyProperty LockVisibilityProperty = DependencyProperty.Register(
            nameof(LockVisibility),
            typeof(bool),
            typeof(LyricsControl),
            new PropertyMetadata(false));

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(LyricsControl),
            new PropertyMetadata(""));


        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        public ConfigManager ConfigManager { get; private set; }


        //  GETTERS & SETTERS

        public bool IsShowed
        {
            get => Visibility == Visibility.Visible;
        }

        public bool LockVisibility
        {
            get => (bool)GetValue(LockVisibilityProperty);
            set => SetValue(LockVisibilityProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set
            {
                SetValue(TextProperty, value);
                OnPropertyChanged(nameof(Text));
                ManageVisibility();
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> LyricsControl class constructor. </summary>
        public LyricsControl()
        {
            //  Setup modules.
            ConfigManager = ConfigManager.Instance;

            //  Initialize interface and components.
            InitializeComponent();
        }

        #endregion CLASS METHODS

        #region INTERFACE MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Show interface programmatically. </summary>
        public void ShowInterface()
        {
            if (!LockVisibility)
                this.Visibility = Visibility.Visible;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Hide interface programmatically. </summary>
        public void HideInterface()
        {
            if (!LockVisibility)
                this.Visibility = Visibility.Collapsed;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Show or hide interface based on text. </summary>
        private void ManageVisibility()
        {
            if (string.IsNullOrEmpty(Text))
                HideInterface();
            else
                ShowInterface();
        }

        #endregion INTERFACE MANAGEMENT METHODS

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
