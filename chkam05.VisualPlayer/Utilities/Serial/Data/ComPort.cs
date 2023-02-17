using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Utilities.Serial.Data
{
    public class ComPort : INotifyPropertyChanged
    {

        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private string _portName;


        //  GETTERS & SETTERS

        public string PortName
        {
            get => _portName;
            set
            {
                _portName = value;
                OnPropertyChanged(nameof(PortName));
            }
        }


        //  METHDOS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> ComPort class constructor. </summary>
        public ComPort()
        {
            //
        }

        //  --------------------------------------------------------------------------------
        /// <summary> ComPort class constructor. </summary>
        /// <param name="portName"> Port name. </param>
        public ComPort(string portName)
        {
            _portName = portName;
        }

        #endregion CLASS METHODS

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

    }
}
