using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace chkam05.VisualPlayer.Controls.Messages.Data
{
    public class AwaitIndicatorData : INotifyPropertyChanged
    {

        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;

        //  VARIABLES

        private Point _innerArcEndPoint;
        private bool _innerArcLarge = false;
        private double _innerArcRotationAngle = 45;
        private Size _innerArcSize;
        private Point _innerArcStartPoint;
        private SweepDirection _innerArcSweepDirection = SweepDirection.Counterclockwise;

        private Point _outerArcEndPoint;
        private bool _outerArcLarge = false;
        private double _outerArcRotationAngle = 45;
        private Size _outerArcSize;
        private Point _outerArcStartPoint;
        private SweepDirection _outerArcSweepDirection = SweepDirection.Clockwise;


        //  GETTERS & SETTERS

        public Point InnerArcEndPoint
        {
            get => _innerArcEndPoint;
            set
            {
                _innerArcEndPoint = value;
                OnPropertyChanged(nameof(InnerArcEndPoint));
            }
        }

        public bool InnerArcLarge
        {
            get => _innerArcLarge;
            set
            {
                _innerArcLarge = value;
                OnPropertyChanged(nameof(InnerArcLarge));
            }
        }

        public double InnerArcRotationAngle
        {
            get => _innerArcRotationAngle;
            set
            {
                _innerArcRotationAngle = Math.Max(Math.Min(value, 360.0), -360.0);
                OnPropertyChanged(nameof(InnerArcRotationAngle));
            }
        }

        public Size InnerArcSize
        {
            get => _innerArcSize;
            set
            {
                _innerArcSize = value;
                OnPropertyChanged(nameof(InnerArcSize));
            }
        }

        public Point InnerArcStartPoint
        {
            get => _innerArcStartPoint;
            set
            {
                _innerArcStartPoint = value;
                OnPropertyChanged(nameof(InnerArcStartPoint));
            }
        }

        public SweepDirection InnerArcSweepDirection
        {
            get => _innerArcSweepDirection;
            set
            {
                _innerArcSweepDirection = value;
                OnPropertyChanged(nameof(InnerArcSweepDirection));
            }
        }

        public Point OuterArcEndPoint
        {
            get => _outerArcEndPoint;
            set
            {
                _outerArcEndPoint = value;
                OnPropertyChanged(nameof(OuterArcEndPoint));
            }
        }

        public bool OuterArcLarge
        {
            get => _outerArcLarge;
            set
            {
                _outerArcLarge = value;
                OnPropertyChanged(nameof(OuterArcLarge));
            }
        }

        public double OuterArcRotationAngle
        {
            get => _outerArcRotationAngle;
            set
            {
                _outerArcRotationAngle = Math.Max(Math.Min(value, 360.0), -360.0);
                OnPropertyChanged(nameof(OuterArcRotationAngle));
            }
        }

        public Size OuterArcSize
        {
            get => _outerArcSize;
            set
            {
                _outerArcSize = value;
                OnPropertyChanged(nameof(OuterArcSize));
            }
        }

        public Point OuterArcStartPoint
        {
            get => _outerArcStartPoint;
            set
            {
                _outerArcStartPoint = value;
                OnPropertyChanged(nameof(OuterArcStartPoint));
            }
        }

        public SweepDirection OuterArcSweepDirection
        {
            get => _outerArcSweepDirection;
            set
            {
                _outerArcSweepDirection = value;
                OnPropertyChanged(nameof(OuterArcSweepDirection));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> AwaitIndicatorData class constructor. </summary>
        public AwaitIndicatorData()
        {
            //
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
