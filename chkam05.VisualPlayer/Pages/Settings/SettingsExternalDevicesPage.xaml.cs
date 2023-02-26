using chkam05.Tools.ControlsEx.Static;
using chkam05.VisualPlayer.Controls.Static;
using chkam05.VisualPlayer.Data.Configuration;
using chkam05.VisualPlayer.Utilities;
using chkam05.VisualPlayer.Utilities.Serial;
using chkam05.VisualPlayer.Utilities.Serial.Data;
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

namespace chkam05.VisualPlayer.Pages.Settings
{
    public partial class SettingsExternalDevicesPage : Page, IPage, INotifyPropertyChanged
    {

        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        public ConfigManager ConfigManager { get; private set; }
        public IPagesManager PagesManager { get; private set; }
        public SerialController SerialController { get; private set; }

        private ObservableCollection<int> _baudRatesCollection;
        private ObservableCollection<ComPort> _devicesCollection;
        private int _baudRate;
        private ComPort _selectedDevice;

        public ObservableCollection<int> BaudRatesCollection
        {
            get => _baudRatesCollection;
            set
            {
                _baudRatesCollection = value;
                _baudRatesCollection.CollectionChanged += (s, e) => OnPropertyChanged(nameof(BaudRatesCollection));
                OnPropertyChanged(nameof(BaudRatesCollection));
            }
        }

        public ObservableCollection<ComPort> DevicesCollection
        {
            get => _devicesCollection;
            set
            {
                _devicesCollection = value;
                _devicesCollection.CollectionChanged += (s, e) => OnPropertyChanged(nameof(DevicesCollection));
                OnPropertyChanged(nameof(DevicesCollection));
            }
        }

        public int BaudRate
        {
            get => _baudRate;
            set
            {
                _baudRate = BaudRatesCollection != null && BaudRatesCollection.Any() && BaudRatesCollection.Contains(value)
                    ? value
                    : SerialController.DEFAULT_BAUD_RATE;

                OnPropertyChanged(nameof(BaudRate));
            }
        }

        public ComPort SelectedDevice
        {
            get => _selectedDevice;
            set
            {
                _selectedDevice = value;
                OnPropertyChanged(nameof(SelectedDevice));
            }
        }


        //  GETTERS & SETTERS

        public string ArduinoLearnMoreLink
        {
            get => "https://github.com/chkam05/ArduinoClockOS";
        }

        public MenuItemType? SpecialMenu
        {
            get => MenuItemType.SETTINGS_MENU;
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> SettingsExternalDevicesPage class constructor. </summary>
        /// <param name="pagesManager"> Pages manager where page will be presented. </param>
        public SettingsExternalDevicesPage(IPagesManager pagesManager)
        {
            //  Setup modules.
            ConfigManager = ConfigManager.Instance;
            SerialController = SerialController.Instnace;

            //  Setup data containers.
            SetupDataContainers();

            //  Initialize interface and components.
            InitializeComponent();

            //  Setup initial data.
            PagesManager = pagesManager;
        }

        #endregion CLASS METHODS

        #region CONTROL BUTTONS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking Back ControlButton. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (PagesManager.CanGoBack)
                PagesManager.GoBack();
            else
                PagesManager.HideInterface();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking Close ControlButton. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            PagesManager.HideInterface();
        }

        #endregion CONTROL BUTTONS METHODS

        #region INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after pressing left mouse button when cursor is over title grid. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Mouse Button Event Arguments. </param>
        private void TitleGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.MainWindow.DragMove();
        }

        #endregion INTERACTION METHODS

        #region MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking on Connect/Disconnect button. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void ConnectionButtonEx_Click(object sender, RoutedEventArgs e)
        {
            if (!SerialController.IsConnected)
            {
                SerialController.PortCom = SelectedDevice.PortName;
                SerialController.BaudRate = BaudRate;
                SerialController.Connect();
            }
            else
            {
                SerialController.Disconnect();
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method ivoked after finish pressing Learn more link. </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LearnMoreLink_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start(ArduinoLearnMoreLink);
        }

        #endregion MANAGEMENT METHODS

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

        #region PAGE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked during page unloading. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            //  Save configuration.
            ConfigManager.SaveConfiguration();
        }

        #endregion PAGE METHODS

        #region SETUP METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Setup data containers. </summary>
        private void SetupDataContainers()
        {
            BaudRatesCollection = new ObservableCollection<int>(SerialController.BAUD_RATES);
            DevicesCollection = new ObservableCollection<ComPort>(SerialController.GetComDevices());
            BaudRate = SerialController.BaudRate;

            if (!string.IsNullOrEmpty(SerialController.PortCom))
                SelectedDevice = DevicesCollection.FirstOrDefault(d => d.PortName == SerialController.PortCom);
            else
                SelectedDevice = DevicesCollection.FirstOrDefault();
        }

        #endregion SETUP METHODS

    }
}
