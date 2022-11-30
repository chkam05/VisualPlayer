using chkam05.VisualPlayer.Core.Events;
using chkam05.VisualPlayer.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Core.Data
{
    public class EqualizerPreset : INotifyPropertyChanged
    {

        //  CONST

        private const string EQ_DEFAULT_NAME = "Default";
        private const int EQ_VALUES_COUNT = 10;

        public const double EQ_MAX_VALUE = 20.0;
        public const double EQ_MIN_VALUE = -20.0;


        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<EqualizerPresetValueChangedEventArgs> PresetValueChanged;


        //  VARIABLES

        private string _name = EQ_DEFAULT_NAME;
        private double[] _values = new double[EQ_VALUES_COUNT] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };


        //  GETTERS & SETTERS

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public double Value0
        {
            get => GetValue(0);
            set
            {
                if (SetValue(0, value))
                    OnPropertyChanged(nameof(Value0));
            }
        }

        public double Value1
        {
            get => GetValue(1);
            set
            {
                if (SetValue(1, value))
                    OnPropertyChanged(nameof(Value1));
            }
        }

        public double Value2
        {
            get => GetValue(2);
            set
            {
                if (SetValue(2, value))
                    OnPropertyChanged(nameof(Value2));
            }
        }

        public double Value3
        {
            get => GetValue(3);
            set
            {
                if (SetValue(3, value))
                    OnPropertyChanged(nameof(Value3));
            }
        }

        public double Value4
        {
            get => GetValue(4);
            set
            {
                if (SetValue(4, value))
                    OnPropertyChanged(nameof(Value4));
            }
        }

        public double Value5
        {
            get => GetValue(5);
            set
            {
                if (SetValue(5, value))
                    OnPropertyChanged(nameof(Value5));
            }
        }

        public double Value6
        {
            get => GetValue(6);
            set
            {
                if (SetValue(6, value))
                    OnPropertyChanged(nameof(Value6));
            }
        }

        public double Value7
        {
            get => GetValue(7);
            set
            {
                if (SetValue(7, value))
                    OnPropertyChanged(nameof(Value7));
            }
        }

        public double Value8
        {
            get => GetValue(8);
            set
            {
                if (SetValue(8, value))
                    OnPropertyChanged(nameof(Value8));
            }
        }

        public double Value9
        {
            get => GetValue(9);
            set
            {
                if (SetValue(9, value))
                    OnPropertyChanged(nameof(Value9));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> EqualizerPreset class constructor. </summary>
        [JsonConstructor]
        public EqualizerPreset()
        {
            //
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Default equalizer preset. </summary>
        public static EqualizerPreset Default = new EqualizerPreset()
        {
            Name = EQ_DEFAULT_NAME,
            _values = new double[EQ_VALUES_COUNT] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
        };

        //  --------------------------------------------------------------------------------
        /// <summary> Bass equalizer preset. </summary>
        public static EqualizerPreset Bass = new EqualizerPreset()
        {
            Name = nameof(Bass),
            _values = new double[EQ_VALUES_COUNT] { 6, 6, 6, 5, 5, 4, 2, 1, 0, 0 }
        };

        //  --------------------------------------------------------------------------------
        /// <summary> Dance equalizer preset. </summary>
        public static EqualizerPreset Dance = new EqualizerPreset()
        {
            Name = nameof(Dance),
            _values = new double[EQ_VALUES_COUNT] { 5, 5, 4, 2, 2, 1, 0, 0, 1, 2 }
        };

        //  --------------------------------------------------------------------------------
        /// <summary> Live equalizer preset. </summary>
        public static EqualizerPreset Live = new EqualizerPreset()
        {
            Name = nameof(Live),
            _values = new double[EQ_VALUES_COUNT] { 0, 0, 1, 2, 4, 4, 2, 1, 0, 0 }
        };

        //  --------------------------------------------------------------------------------
        /// <summary> Pop equalizer preset. </summary>
        public static EqualizerPreset Pop = new EqualizerPreset()
        {
            Name = nameof(Pop),
            _values = new double[EQ_VALUES_COUNT] { 1, 1, 4, 5, 6, 5, 4, 1, 0, 0 }
        };

        //  --------------------------------------------------------------------------------
        /// <summary> Power equalizer preset. </summary>
        public static EqualizerPreset Power = new EqualizerPreset()
        {
            Name = nameof(Power),
            _values = new double[EQ_VALUES_COUNT] { 4, 5, 5, 2, -2, -1, 0, 1, 2, 2 }
        };

        //  --------------------------------------------------------------------------------
        /// <summary> Rock equalizer preset. </summary>
        public static EqualizerPreset Rock = new EqualizerPreset()
        {
            Name = nameof(Rock),
            _values = new double[EQ_VALUES_COUNT] { 4, 4, 2, 1, 0, 1, 2, 4, 4, 4 }
        };

        //  --------------------------------------------------------------------------------
        /// <summary> Treble equalizer preset. </summary>
        public static EqualizerPreset Treble = new EqualizerPreset()
        {
            Name = nameof(Treble),
            _values = new double[EQ_VALUES_COUNT] { 0, 0, 0, 1, 2, 4, 5, 5, 6, 6 }
        };

        //  --------------------------------------------------------------------------------
        /// <summary> Vocal equalizer preset. </summary>
        public static EqualizerPreset Vocal = new EqualizerPreset()
        {
            Name = nameof(Vocal),
            _values = new double[EQ_VALUES_COUNT] { 2, 2, 5, 5, 5, 2, 0, 0, 0, 0 }
        };

        //  --------------------------------------------------------------------------------
        /// <summary> Xplode1 equalizer preset. </summary>
        public static EqualizerPreset Xplode1 = new EqualizerPreset()
        {
            Name = nameof(Xplode1),
            _values = new double[EQ_VALUES_COUNT] { 6, 6, 6, 5, 4, 2, 4, 5, 6, 6 }
        };

        //  --------------------------------------------------------------------------------
        /// <summary> Xplode2 equalizer preset. </summary>
        public static EqualizerPreset Xplode2 = new EqualizerPreset()
        {
            Name = nameof(Xplode2),
            _values = new double[EQ_VALUES_COUNT] { 5, 5, 5, 8, 5, 2, 2, 5, 5, 2 }
        };

        //  --------------------------------------------------------------------------------
        /// <summary> Xplode3 equalizer preset. </summary>
        public static EqualizerPreset Xplode3 = new EqualizerPreset()
        {
            Name = nameof(Xplode3),
            _values = new double[EQ_VALUES_COUNT] { 8, 10, 13, 4, 1, 4, 5, 6, 5, 1 }
        };

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

        #region UTILITY METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get preset value. </summary>
        /// <param name="index"> Preset value index. </param>
        public double GetValue(int index)
        {
            if (_values == null)
                _values = new double[EQ_VALUES_COUNT] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            if (index >= 0 && index < EQ_VALUES_COUNT)
                return _values[index];

            return 0;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set preset value. </summary>
        /// <param name="index"> Preset value index. </param>
        /// <param name="value"> New preset value. </param>
        public bool SetValue(int index, double value)
        {
            if (_values == null)
                _values = new double[EQ_VALUES_COUNT] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            if (index >= 0 && index < EQ_VALUES_COUNT)
            {
                double newValue = Math.Max(Math.Min(value, EQ_MAX_VALUE), EQ_MIN_VALUE);
                _values[index] = newValue;
                PresetValueChanged?.Invoke(this, new EqualizerPresetValueChangedEventArgs(index, newValue));
                return true;
            }

            return false;
        }

        #endregion UTILITY METHODS

    }
}
