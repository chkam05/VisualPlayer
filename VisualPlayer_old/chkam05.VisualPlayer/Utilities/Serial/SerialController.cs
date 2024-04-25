using chkam05.VisualPlayer.Data.PlayLists;
using chkam05.VisualPlayer.Utilities.Serial.Data;
using chkam05.VisualPlayer.Utilities.Serial.Events;
using chkam05.VisualPlayer.Utilities.Serial.Static;
using CSCore.DSP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Utilities.Serial
{
    public class SerialController : IDisposable, INotifyPropertyChanged
    {

        //  CONST

        private const string COM_SCOPE = "root\\WMI";
        private const string COM_Q_SEARCH = "SELECT * FROM MSSerial_PortName";
        public const int DEFAULT_BAUD_RATE = 9600;

        public static readonly List<int> BAUD_RATES = new List<int>()
        {
            300,
            600,
            1200,
            2400,
            4800,
            9600,
            14400,
            19200,
            28800,
            31250,
            38400,
            57600,
            115200
        };


        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<SerialPortReceivedMessageEventArgs> ReceivedMessage;


        //  VARIABLES

        private static SerialController _instance;

        private BackgroundWorker _bgReceiverService;
        private bool _disconnectRequested;
        private int _baudRate = DEFAULT_BAUD_RATE;
        private string _portCom;
        private SerialPort _serialPort;


        //  GETTERS & SETTERS

        public static SerialController Instnace
        {
            get
            {
                if (_instance == null)
                    _instance = new SerialController();

                return _instance;
            }
        }

        public int BaudRate
        {
            get => _baudRate;
            set
            {
                _baudRate = value;
                OnPropertyChanged(nameof(BaudRate));
            }
        }

        public string PortCom
        {
            get => _portCom;
            set
            {
                _portCom = value;
                OnPropertyChanged(nameof(PortCom));
            }
        }

        public bool IsConnected
        {
            get => _serialPort != null ? _serialPort.IsOpen : false;
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> SerialController private class constructor. </summary>
        private SerialController()
        {
            //
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Performs application-defined tasks associated with freeing, releasing, 
        /// or resetting unmanaged resources. </summary>
        public void Dispose()
        {
            if (IsConnected)
                Disconnect();
        }

        #endregion CLASS METHODS

        #region CONNECTION MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Connect to selected communication port device. </summary>
        /// <returns> True - connected; False - otherwise. </returns>
        public bool Connect()
        {
            _disconnectRequested = false;
            _serialPort = new SerialPort();
            _serialPort.PortName = _portCom;
            _serialPort.BaudRate = _baudRate;
            _serialPort.Encoding = Encoding.UTF8;

            try
            {
                ReceivedMessage?.Invoke(this,
                        new SerialPortReceivedMessageEventArgs("Connecting...", ReceivedMessageState.System));

                _serialPort.Open();
                SendMessage("/vp start");

                ConfigureAndStartReciver();
            }
            catch (UnauthorizedAccessException e)
            {
                ReceivedMessage?.Invoke(this,
                    new SerialPortReceivedMessageEventArgs(e.Message, ReceivedMessageState.Error));
            }
            catch (ArgumentOutOfRangeException e)
            {
                ReceivedMessage?.Invoke(this,
                    new SerialPortReceivedMessageEventArgs(e.Message, ReceivedMessageState.Error));
            }
            catch (ArgumentException e)
            {
                ReceivedMessage?.Invoke(this,
                    new SerialPortReceivedMessageEventArgs(e.Message, ReceivedMessageState.Error));
            }
            catch (InvalidOperationException e)
            {
                ReceivedMessage?.Invoke(this,
                    new SerialPortReceivedMessageEventArgs(e.Message, ReceivedMessageState.Error));
            }

            OnPropertyChanged(nameof(IsConnected));
            return _serialPort?.IsOpen ?? false;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Disconnect communication port device. </summary>
        /// <param name="skipStoppingReciverService"> Skip stopping receiver service. </param>
        /// <returns> True - disconnection successed; False - disconnection failed or not required. </returns>
        public bool Disconnect(bool skipStoppingReciverService = false)
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _disconnectRequested = true;

                try
                {
                    if (!skipStoppingReciverService)
                        StopReciver();

                    ReceivedMessage?.Invoke(this,
                        new SerialPortReceivedMessageEventArgs("Disconnecting...", ReceivedMessageState.System));

                    SendMessage("/vp stop");
                    _serialPort.Dispose();
                }
                catch (IOException e)
                {
                    ReceivedMessage?.Invoke(this,
                        new SerialPortReceivedMessageEventArgs(e.Message, ReceivedMessageState.Error));
                }
            }

            OnPropertyChanged(nameof(IsConnected));
            return _serialPort?.IsOpen ?? false;
        }

        #endregion CONNECTION MANAGEMENT METHODS

        #region MESSAGING METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Send data to device connectied via communication port. </summary>
        /// <param name="message"> Data as string message. </param>
        public void SendMessage(string message)
        {
            if (!string.IsNullOrWhiteSpace(message) && _serialPort != null && _serialPort.IsOpen)
            {
                try
                {
                    _serialPort.Write(message);
                }
                catch (InvalidOperationException e)
                {
                    ReceivedMessage?.Invoke(this,
                        new SerialPortReceivedMessageEventArgs(e.Message, ReceivedMessageState.Error));
                }
                catch (ArgumentNullException e)
                {
                    ReceivedMessage?.Invoke(this,
                        new SerialPortReceivedMessageEventArgs(e.Message, ReceivedMessageState.Error));
                }
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Configure and start background reciver service task. </summary>
        private void ConfigureAndStartReciver()
        {
            _bgReceiverService = new BackgroundWorker();
            _bgReceiverService.WorkerReportsProgress = true;
            _bgReceiverService.WorkerSupportsCancellation = true;
            _bgReceiverService.DoWork += ReciverDoWork;
            _bgReceiverService.ProgressChanged += ReciverProgressChanged;
            _bgReceiverService.RunWorkerCompleted += ReciverWorkerCompleted;
            _bgReceiverService.RunWorkerAsync(_serialPort);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Stop background reciver service task. </summary>
        private void StopReciver()
        {
            if (_bgReceiverService != null && _bgReceiverService.IsBusy)
                _bgReceiverService.CancelAsync();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Background reciver service task working method. </summary>
        /// <param name="sender"> Object from which mmethod has been invoked. </param>
        /// <param name="e"> Do work event arguments. </param>
        private void ReciverDoWork(object sender, DoWorkEventArgs e)
        {
            var bgWorker = (BackgroundWorker)sender;
            string message = string.Empty;
            var serialPort = (SerialPort)e.Argument;

            while (!e.Cancel || serialPort.IsOpen)
            {
                string messagePart = serialPort.ReadExisting();
                message += messagePart;

                if (!string.IsNullOrWhiteSpace(message) && message.EndsWith(Environment.NewLine))
                {
                    bgWorker.ReportProgress(0, message);
                    message = string.Empty;
                }
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Background reciver service task report method. </summary>
        /// <param name="sender"> Object from which mmethod has been invoked. </param>
        /// <param name="e"> Progress changed event arguments. </param>
        private void ReciverProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            string message = (string)e.UserState;

            if (message.Contains("/vp exit"))
                Disconnect();

            ReceivedMessage?.Invoke(this,
                new SerialPortReceivedMessageEventArgs(message, ReceivedMessageState.Message));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Background reciver service task finish method. </summary>
        /// <param name="sender"> Object from which mmethod has been invoked. </param>
        /// <param name="e"> Run worker completed event arguments. </param>
        private void ReciverWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                ReceivedMessage?.Invoke(this,
                    new SerialPortReceivedMessageEventArgs(e.Error.Message, ReceivedMessageState.Error));

                OnPropertyChanged(nameof(IsConnected));
            }
            else if (e.Cancelled)
            {
                if (!_disconnectRequested && _serialPort.IsOpen)
                    Disconnect(true);

                ReceivedMessage?.Invoke(this,
                    new SerialPortReceivedMessageEventArgs("Stopping receiver service...", ReceivedMessageState.System));
            }
            else
            {
                ReceivedMessage?.Invoke(this,
                    new SerialPortReceivedMessageEventArgs("Stopping receiver service...", ReceivedMessageState.System));
            }
        }

        #endregion MESSAGING METHODS

        #region NOTIFY PROPERTIES CHANGED INTERFACE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method for invoking PropertyChangedEventHandler event. </summary>
        /// <param name="propertyName"> Changed property name. </param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion NOTIFY PROPERTIES CHANGED INTERFACE METHODS

        #region STATIC METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get list of available com port devices. </summary>
        /// <returns> List of com port devices. </returns>
        public static List<ComPort> GetComDevices()
        {
            List<ComPort> result = new List<ComPort>();
            string[] ports = SerialPort.GetPortNames();

            if (ports != null && ports.Any())
                foreach (var port in ports)
                    result.Add(new ComPort(port));

            return result;
        }

        #endregion STATIC METHODS

    }
}
