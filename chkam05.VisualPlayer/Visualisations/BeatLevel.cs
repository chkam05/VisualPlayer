using chkam05.VisualPlayer.Visualisations.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Visualisations
{
    public class BeatLevel
    {

        //  CONST

        public const double FALL_SPEED_MAX = 0.1;
        public const double FALL_SPEED_MIN = 0.01;
        public const double SENSITIVITY_MAX = 1.0;
        public const double SENSITIVITY_MIN = 0.0;


        //  VARIABLES

        private double _beatFallSpeed = 0.02;
        private double _beatSensitivity = 0.5;

        private double _spectrumMaxLevel = 0;
        private int _spectrumSize = 0;

        private double _beatAverage = 0.0;
        double _tempAverage = 0.0;
        int _tempCount = 0;


        //  GETTERS & SETTERS

        public double Level
        {
            get => Math.Max(0, _beatAverage);
        }

        public double BeatFallSpeed
        {
            get => _beatFallSpeed;
            set => _beatFallSpeed = Math.Max(Math.Min(value, FALL_SPEED_MAX), FALL_SPEED_MIN);
        }

        public double BeatSensitivity
        {
            get => 1.0 - _beatSensitivity;
            set
            {
                _beatSensitivity = 1.0 - Math.Max(Math.Min(value, SENSITIVITY_MAX), SENSITIVITY_MIN);
            }
        }

        public double SpectrumMaxLevelValue
        {
            get => _spectrumMaxLevel;
            set => _spectrumMaxLevel = Math.Max(0, value);
        }

        public int SpectrumSize
        {
            get => _spectrumSize;
            set => _spectrumSize = Math.Max(0, value);
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> BeatUtility class constructor. </summary>
        /// <param name="maxLevel"> Max spectrum level value. </param>
        /// <param name="size"> Spectrum size. </param>
        public BeatLevel(double maxLevel, int size)
        {
            SpectrumMaxLevelValue = maxLevel;
            SpectrumSize = size;
        }

        #endregion CLASS METHODS

        #region CALCULATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Calculate beat from spectrum part level. </summary>
        /// <param name="level"> Spectrum part level. </param>
        private void CalculateBeatPart(SpectrumLevel level)
        {
            double limiter = SpectrumMaxLevelValue * _beatSensitivity;

            if (level.Index < SpectrumSize && level.Value > limiter)
            {
                _tempAverage += (level.Value - limiter);
                _tempCount++;
            };
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Calculate beat level. </summary>
        /// <param name="spectrum"> Spectrum from visualisation. </param>
        private void CalculateBeatLevel(SpectrumLevel[] spectrum)
        {
            _tempAverage = 0.0;
            _tempCount = 0;

            if (spectrum != null && spectrum.Any())
            {
                foreach (var level in spectrum)
                    CalculateBeatPart(level);

                double average = 0.0;
                double limiter = SpectrumMaxLevelValue * _beatSensitivity;

                if (_tempCount > 0 && limiter > 0)
                    average += ((_tempAverage / _tempCount) * 1.0) / limiter;

                _beatAverage = Math.Max(0.0, average > _beatAverage ? average : _beatAverage - _beatFallSpeed);
            }
        }

        #endregion CALCULATION METHODS

        #region GET METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get beat level. </summary>
        /// <param name="spectrum"> Spectrum from visualisation. </param>
        /// <param name="maxLevel"> Max spectrum level value. </param>
        /// <param name="size"> Spectrum size. </param>
        public double GetBeatLevel(SpectrumLevel[] spectrum)
        {
            CalculateBeatLevel(spectrum);
            return _beatAverage;
        }

        #endregion GET METHODS

    }
}
