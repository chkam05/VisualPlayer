using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace chkam05.VisualPlayer.Utilities
{
    public class RainbowColorGenerator
    {

        //  CONST

        private const int FULL_SCALE = 1530;


        //  VARIABLES

        private int _colorChangeStep = 0;
        private int _colorScaleStep = 1;
        private int _colorScale = FULL_SCALE;

        private byte _transparency = 255;
        private List<Color> _checkPoints;
        private Color _currentColor;
        private Color _startColor;


        #region GETTERS & SETTERS

        public Color Color
        {
            get => _currentColor;
        }

        public int CheckPointsCount
        {
            get => _checkPoints.Count;
        }

        public List<Color> CheckPoints
        {
            get => _checkPoints;
        }

        public int Scale
        {
            get => _colorScale;
            set
            {
                _colorScale = Math.Max(1, Math.Min(value, FULL_SCALE));
                _colorScaleStep = CalculateStepScale(_colorScale);
            }
        }

        public Color StartColor
        {
            get => _startColor;
            set => _startColor = value;
        }

        public byte Transparency
        {
            get => _transparency;
            set => _transparency = value;
        }

        #endregion GETTERS & SETTERS


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> RainbowColorGenerator class constructor. </summary>
        private RainbowColorGenerator()
        {
            _checkPoints = new List<Color>();
            _startColor = Color.FromArgb(_transparency, 255, 0, 0);

            Reset();
        }

        #endregion CLASS METHODS

        #region CALCULATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Calculate color scale step. </summary>
        /// <param name="scale"> Colors scale. </param>
        private int CalculateStepScale(int scale)
        {
            return FULL_SCALE / Math.Min(scale, FULL_SCALE);
        }

        #endregion CALCULATION METHODS

        #region CHECKPOINTS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Remove checkpoint with particular index. </summary>
        /// <param name="checkPointIndex"> Checkpoint index to remove. </param>
        public void RemoveCheckpoint(int checkPointIndex)
        {
            if (_checkPoints.Any() && checkPointIndex > -1 && checkPointIndex < _checkPoints.Count - 1)
                _checkPoints.RemoveAt(checkPointIndex);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Remove last checkpoint. </summary>
        public void RemoveLastCheckpoint()
        {
            if (_checkPoints.Any())
                _checkPoints.RemoveAt(_checkPoints.Count - 1);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Reset current color to particular color cehckpoint. </summary>
        /// <param name="checkPointIndex"> Checkpoint index. </param>
        public void ResetToCheckPoint(int checkPointIndex)
        {
            if (_checkPoints.Any() && checkPointIndex > -1 && checkPointIndex < _checkPoints.Count)
                _currentColor = _checkPoints[checkPointIndex];
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Reset current color to last checkpoint. </summary>
        public void ResetToLastCheckpoint()
        {
            if (_checkPoints.Any())
                _currentColor = _checkPoints[_checkPoints.Count - 1];
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Change existing checkpoint. </summary>
        /// <param name="checkPointIndex"> Index of checkpoint to change. </param>
        /// <returns> Index of changed checkpoint or -1 if indes is out of range. </returns>
        public int SetCheckPoint(int checkPointIndex)
        {
            if (_checkPoints.Any() && checkPointIndex > -1 && checkPointIndex < _checkPoints.Count)
            {
                _checkPoints[checkPointIndex] = _currentColor;
                return checkPointIndex;
            }

            return -1;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Add new checkpoint. </summary>
        /// <returns> Checkpoint index. </returns>
        public int SetCheckPoint()
        {
            _checkPoints.Add(_currentColor);
            return _checkPoints.Count - 1;
        }

        #endregion CHECKPOINTS METHODS

        #region INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Calculate next rainbow color. </summary>
        /// <returns> Next rainbow color. </returns>
        public Color Next()
        {
            byte r = _currentColor.R;
            byte g = _currentColor.G;
            byte b = _currentColor.B;

            byte[] cc;

            switch (_colorChangeStep)
            {
                //  G up
                case 0:
                    cc = IncreaseColor(g);
                    g = cc[0];
                    r = (byte)(r - cc[1]);
                    if (r <= g)
                        _colorChangeStep = 1;
                    break;

                //  R down
                case 1:
                    cc = DecreaseColor(r);
                    r = cc[0];
                    b = cc[1];
                    if (r == 0)
                        _colorChangeStep = 2;
                    break;

                //  B up
                case 2:
                    cc = IncreaseColor(b);
                    b = cc[0];
                    g = (byte)(g - cc[1]);
                    if (g <= b)
                        _colorChangeStep = 3;
                    break;

                //  G down
                case 3:
                    cc = DecreaseColor(g);
                    g = cc[0];
                    r = cc[1];
                    if (g == 0)
                        _colorChangeStep = 4;
                    break;

                //  R up
                case 4:
                    cc = IncreaseColor(r);
                    r = cc[0];
                    b = (byte)(b - cc[1]);
                    if (b < r)
                        _colorChangeStep = 5;
                    break;

                //  B down
                case 5:
                    cc = DecreaseColor(b);
                    b = cc[0];
                    g = cc[1];
                    if (b == 0)
                        _colorChangeStep = 0;
                    break;
            }

            _currentColor = Color.FromArgb(_transparency, r, g, b);
            return _currentColor;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Increase color component by color scale step. </summary>
        /// <param name="color"> Color component value. </param>
        /// <returns> Byte array: [new color component value, next color component decrease value]. </returns>
        private byte[] IncreaseColor(byte color)
        {
            int temp = color + _colorScaleStep;

            if (temp > 255)
                return new byte[] { 255, (byte)(255 - (temp - 255)) };
            else
                return new byte[] { (byte)temp, 0 };
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Decrease color component by color scale step. </summary>
        /// <param name="color"> Color component value. </param>
        /// <returns> Byte array: [new color component value, next color component increase value]. </returns>
        private byte[] DecreaseColor(byte color)
        {
            int temp = color - _colorScaleStep;

            if (temp < 0)
                return new byte[] { 0, (byte)(temp * -1) };
            else
                return new byte[] { (byte)temp, 0 };
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Reset current color to start color. </summary>
        public void Reset()
        {
            _currentColor = _startColor;
        }

        #endregion INTERACTION METHODS

    }
}
