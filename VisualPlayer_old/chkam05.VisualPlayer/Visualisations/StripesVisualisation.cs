using chkam05.VisualPlayer.Utilities;
using chkam05.VisualPlayer.Utilities.Data;
using chkam05.VisualPlayer.Visualisations.Data;
using chkam05.VisualPlayer.Visualisations.Spectrum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace chkam05.VisualPlayer.Visualisations
{
    public class StripesVisualisation : BaseVisualisation
    {

        //  CONST

        public const double FALL_SPEED_MAX = 16.0;
        public const double FALL_SPEED_MIN = 8.0;
        public const double FALL_SPEED_FLOATER_MIN = 2.0;
        public const double OPACITY_MAX = 1.0;
        public const double OPACITY_MIN = 0.0;
        public const double RAINBOW_CHANGE_TIME_MAX = 1.0;
        public const double RAINBOW_CHANGE_TIME_MIN = 0.01;


        //  VARIABLES

        protected double _fallSpeed = 8.0;
        protected double _fallSpeedFloater = 2.0;
        protected double _opacity = 0.8;
        protected double _peakSpaceX = 4.0;

        protected double _firstX = 0;
        protected double _spectrumHeight = 0;
        protected SpectrumLevel[] _spectrumData = null;
        protected SpectrumLevel[] _spectrumFloaterData = null;
        protected double _stripeWidth = 0;
        protected DateTime _startRun = DateTime.Now;

        protected Color _borderColor = Color.FromArgb(244, 0, 120, 215);
        protected Color _fillColor = Color.FromArgb(244, 0, 120, 215);

        protected AHSLColor _initBorderColor = AHSLColor.FromColor(Color.FromRgb(0, 255, 0));
        protected AHSLColor _initFillColor = AHSLColor.FromColor(Color.FromRgb(0, 255, 0));
        protected int _rainbowX = AHSLColor.HUE_MAX;
        protected int _rainbowY = 1000;
        protected double _rainbowChangeTime = 0.0;
        protected TimeSpan _runTime = new TimeSpan(0);

        public bool BorderEnabled { get; set; } = false;
        public VisualisationColorType ColorType { get; set; } = VisualisationColorType.SYSTEM;
        public bool RainbowShift { get; set; } = false;


        //  GETTERS & SETTERS

        public Color BorderColor
        {
            get => _borderColor;
            set
            {
                _borderColor = value;
                _initBorderColor = ColorUtilities.UpdateColor(
                    AHSLColor.FromColor(value), l: 50, s: 100);
            }
        }

        public double FallSpeed
        {
            get => _fallSpeed;
            set => _fallSpeed = Math.Min(Math.Max(value, FALL_SPEED_MIN), FALL_SPEED_MAX);
        }

        public double FallSpeedFloater
        {
            get => _fallSpeedFloater;
            set => _fallSpeedFloater = Math.Min(Math.Max(value, FALL_SPEED_FLOATER_MIN), FallSpeed);
        }

        public Color FillColor
        {
            get => _fillColor;
            set
            {
                _fillColor = value;
                _initFillColor = ColorUtilities.UpdateColor(
                    AHSLColor.FromColor(value), l: 50, s: 100);
            }
        }

        public double Opacity
        {
            get => _opacity;
            set => _opacity = Math.Min(Math.Max(value, OPACITY_MIN), OPACITY_MAX);
        }

        public double PeakSpaceX
        {
            get => _peakSpaceX;
            set => _peakSpaceX = Math.Max(1, value);
        }

        public int RainbowX
        {
            get => _rainbowX;
            set => _rainbowX = Math.Max(Math.Min(value, AHSLColor.HUE_MAX), 0);
        }

        public int RainbowY
        {
            get => _rainbowY;
            set => _rainbowY = Math.Max(Math.Min(value, AHSLColor.HUE_MAX), 0);
        }

        public double RainbowChangeTime
        {
            get => _rainbowChangeTime;
            set => _rainbowChangeTime = Math.Max(Math.Min(value, RAINBOW_CHANGE_TIME_MAX), RAINBOW_CHANGE_TIME_MIN);
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> StripesVisualisation class constructor. </summary>
        /// <param name="spectrumProvider"> Spectrum provider with FFT data. </param>
        public StripesVisualisation(SpectrumProvider spectrumProvider) : base(spectrumProvider)
        {
            //
        }

        #endregion CLASS METHODS

        #region CALCULATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Pre calculate visualisation graphics before drawing it. </summary>
        public override void PreCalculate()
        {
            if (_spectrumProvider == null)
                return;

            //  Get buffer data from Player.
            var fftBuffer = new float[_fftSize];
            _spectrumProvider.GetFFTData(fftBuffer, this);

            _spectrumHeight = DrawingAreaSize.Height - (Margin.Top + Margin.Bottom);
            BeatLevel.SpectrumMaxLevelValue = _spectrumHeight;

            //  Calculate spectrum line points.
            var spectrum = GetSpectrum(_spectrumHeight, fftBuffer, ScalingStrategy, true, true);

            if (_spectrumData == null || _spectrumData.Length != spectrum.Count)
            {
                _spectrumData = spectrum.ToArray();
                _spectrumFloaterData = spectrum.ToArray();
            }

            //  Pre calculation visualisation graphics.
            for (int iX = 0; iX < _spectrumData.Length; iX++)
            {
                if (_spectrumData[iX].Value < spectrum[iX].Value)
                    _spectrumData[iX] = spectrum[iX];
                else
                    _spectrumData[iX].Value = Math.Max(0, _spectrumData[iX].Value - _fallSpeed);

                if (_spectrumFloaterData[iX].Value < spectrum[iX].Value)
                    _spectrumFloaterData[iX] = spectrum[iX];
                else
                    _spectrumFloaterData[iX].Value = Math.Max(0, _spectrumFloaterData[iX].Value - _fallSpeedFloater);
            }
        }

        #endregion CALCULATION METHODS

        #region DRAWING METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Draw visualisation. </summary>
        /// <returns> Bitmap drawer. </returns>
        public override BitmapDrawer Draw()
        {
            if (_spectrumProvider == null)
                return null;

            var bitmapDrawer = new BitmapDrawer(DrawingAreaSize);

            Brush borderBrush = null;
            Brush fillBrush = null;

            UpdateRunTime();

            switch (ColorType)
            {
                case VisualisationColorType.RAINBOW_HORIZONTAL:
                    if (RainbowShift && IsRainbowTimeChangeReached())
                    {
                        _initBorderColor = ColorUtilities.UpdateColor(_initBorderColor, h: _initBorderColor.H + 1);
                        _initFillColor = ColorUtilities.UpdateColor(_initFillColor, h: _initFillColor.H + 1);
                    }
                    break;

                case VisualisationColorType.RAINBOW_VERTICAL:
                    if (RainbowShift && IsRainbowTimeChangeReached())
                    {
                        _initBorderColor = ColorUtilities.UpdateColor(_initBorderColor, h: _initBorderColor.H + 1);
                        _initFillColor = ColorUtilities.UpdateColor(_initFillColor, h: _initFillColor.H + 1);
                    }
                    break;

                case VisualisationColorType.CUSTOM:
                case VisualisationColorType.SYSTEM:
                default:
                    borderBrush = new SolidColorBrush(BorderColor) { Opacity = _opacity };
                    fillBrush = new SolidColorBrush(FillColor) { Opacity = _opacity };
                    break;
            }

            for (int iX = 0; iX < _spectrumData.Length; iX++)
            {
                var level = _spectrumData[iX];
                var pX = _firstX + (_stripeWidth * iX + _peakSpaceX * iX);
                var pY = DrawingAreaSize.Height - Margin.Bottom - level.Value;

                var point = new Point(pX, pY);
                var size = new Size(_stripeWidth, level.Value);

                Pen pen = null;

                switch (ColorType)
                {
                    case VisualisationColorType.RAINBOW_HORIZONTAL:
                        var hFColor = ColorUtilities.UpdateColor(_initFillColor, h: _initFillColor.H + (RainbowX / SpectrumSize) * iX);
                        fillBrush = new SolidColorBrush(hFColor.ToColor()) { Opacity = _opacity };

                        if (BorderEnabled)
                        {
                            var hBColor = ColorUtilities.UpdateColor(_initBorderColor, h: _initBorderColor.H + (RainbowX / SpectrumSize) * iX);
                            borderBrush = new SolidColorBrush(hBColor.ToColor()) { Opacity = _opacity };
                        }

                        pen = BorderEnabled ? new Pen(borderBrush, 1.0) : null;
                        break;

                    case VisualisationColorType.RAINBOW_VERTICAL:
                        var vFColor = ColorUtilities.UpdateColor(_initFillColor, h: _initFillColor.H + (RainbowX / SpectrumSize) * iX);
                        fillBrush = RainbowColorGenerator.GetRainbowGradient(vFColor, RainbowY, (int)level.Value, _spectrumHeight);
                        fillBrush.Opacity = _opacity;
                        
                        if (BorderEnabled)
                        {
                            var vBColor = ColorUtilities.UpdateColor(_initBorderColor, h: _initBorderColor.H + (RainbowX / SpectrumSize) * iX);
                            borderBrush = RainbowColorGenerator.GetRainbowGradient(vBColor, RainbowY, (int)level.Value, _spectrumHeight);
                            borderBrush.Opacity = _opacity;
                        }

                        pen = BorderEnabled ? new Pen(borderBrush, 1.0) : null;
                        break;

                    default:
                        pen = BorderEnabled ? new Pen(borderBrush, 1.0) : null;
                        break;
                }

                bitmapDrawer.DrawRectangle(fillBrush, pen, point, size);
            }

            return bitmapDrawer;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Stop drawing visualisation. </summary>
        /// <returns> Bitmap drawer. </returns>
        public override BitmapDrawer StopDrawing()
        {
            if (_spectrumProvider == null)
                return null;

            for (int iX = 0; iX < _spectrumData.Length; iX++)
            {
                _spectrumData[iX].Value = 0;
                _spectrumFloaterData[iX].Value = 0;
            }

            return new BitmapDrawer(DrawingAreaSize);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update graphics configuration. </summary>
        public override void UpdateGraphics()
        {
            double width = DrawingAreaSize.Width - (Margin.Left + Margin.Right + (_peakSpaceX * 2));
            double spacesSize = _peakSpaceX * (SpectrumSize - 1);

            _firstX = Margin.Left + _peakSpaceX;
            _stripeWidth = (width - spacesSize) / SpectrumSize;
        }

        #endregion DRAWING METHODS

        #region GET METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get beat level value. </summary>
        /// <returns> Beat level value. </returns>
        public override double GetBeatLevel() => BeatLevel.GetBeatLevel(_spectrumData);

        #endregion GET METHODS

        #region UTILITY METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Check if runtime reach rainbow change time. </summary>
        /// <returns> True - rainbow change time reached; False - other way. </returns>
        protected bool IsRainbowTimeChangeReached()
        {
            if (_rainbowChangeTime > 0)
                return _runTime.Milliseconds >= Math.Round(_rainbowChangeTime, 2);

            return false;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update run time. </summary>
        protected void UpdateRunTime()
        {
            if (_rainbowChangeTime > 0)
            {
                _runTime = _runTime.Add(DateTime.Now - _startRun);
                _startRun = DateTime.Now;
            }

            else if (_runTime.TotalMilliseconds > 0)
                _runTime = new TimeSpan(0);
        }

        #endregion UTILITY METHODS

    }
}
