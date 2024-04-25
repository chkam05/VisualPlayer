using chkam05.VisualPlayer.Utilities;
using chkam05.VisualPlayer.Utilities.Data;
using chkam05.VisualPlayer.Visualisations.Data;
using chkam05.VisualPlayer.Visualisations.Spectrum;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;


namespace chkam05.VisualPlayer.Visualisations
{
    public class WaveVisualisation : BaseVisualisation
    {

        //  VARIABLES

        protected double _fallSpeed = 8.0;
        protected double _opacity = 0.8;

        protected SpectrumLevel[] _spectrumData = null;
        protected DateTime _startRun = DateTime.Now;

        protected double _borderedHeight = 0;
        protected double _borderedWidth = 0;
        protected double _startX = 0;
        protected double _startY = 0;

        protected Color _borderColor = Color.FromArgb(244, 0, 120, 215);
        protected Color _fillColor = Color.FromArgb(244, 0, 120, 215);

        protected double _borderThickness = 4;
        protected AHSLColor _initBorderColor = AHSLColor.FromColor(Color.FromRgb(0, 255, 0));
        protected AHSLColor _initFillColor = AHSLColor.FromColor(Color.FromRgb(0, 255, 0));
        protected int _rainbowX = AHSLColor.HUE_MAX;
        protected int _rainbowY = 1000;
        protected double _rainbowChangeTime = 0.0;
        protected TimeSpan _runTime = new TimeSpan(0);

        public bool BorderEnabled { get; set; } = false;
        public VisualisationColorType ColorType { get; set; } = VisualisationColorType.SYSTEM;
        public bool FillEnabled { get; set; } = true;
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
            set => _fallSpeed = Math.Min(
                Math.Max(value, StripesVisualisation.FALL_SPEED_MIN),
                StripesVisualisation.FALL_SPEED_MAX);
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
            set => _opacity = Math.Min(
                Math.Max(value, StripesVisualisation.OPACITY_MIN),
                StripesVisualisation.OPACITY_MAX);
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
            set => _rainbowChangeTime = Math.Max(
                Math.Min(value, StripesVisualisation.RAINBOW_CHANGE_TIME_MAX),
                StripesVisualisation.RAINBOW_CHANGE_TIME_MIN);
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> WaveVisualisation class constructor. </summary>
        /// <param name="spectrumProvider"> Spectrum provider with FFT data. </param>
        public WaveVisualisation(SpectrumProvider spectrumProvider) : base(spectrumProvider)
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

            var spectrumHeight = DrawingAreaSize.Height - (Margin.Top + Margin.Bottom);
            BeatLevel.SpectrumMaxLevelValue = spectrumHeight;

            //  Calculate spectrum line points.
            var spectrum = GetSpectrum(spectrumHeight, fftBuffer, ScalingStrategy, true, true);

            if (_spectrumData == null || _spectrumData.Length != spectrum.Count)
            {
                _spectrumData = spectrum.ToArray();
            }

            //  Pre calculation visualisation graphics.
            for (int iX = 0; iX < _spectrumData.Length; iX++)
            {
                _spectrumData[iX].Value = _spectrumData[iX].Value;

                if (_spectrumData[iX].Value < spectrum[iX].Value)
                    _spectrumData[iX] = spectrum[iX];
                else
                    _spectrumData[iX].Value = Math.Max(0, _spectrumData[iX].Value - _fallSpeed);
            }
        }

        //  --------------------------------------------------------------------------------
        protected Point[] CalculateSharpPoints()
        {
            double halfHeight = _borderedHeight / 2;
            int intWidth = (int)_borderedWidth;
            double xStep = _borderedWidth / (SpectrumSize - 1);

            Point[] points = new Point[intWidth];

            for (int i = 0; i < intWidth; i++)
            {
                double x = i / xStep;
                int index1 = (int)x;
                int index2 = index1 + 1;
                double alpha = x - index1;

                if (index1 < 0)
                    index1 = 0;

                if (index2 >= SpectrumSize)
                    index2 = SpectrumSize - 1;

                double value1 = GetSinLevel(index1);
                double value2 = GetSinLevel(index2);
                double y = (1 - alpha) * value1 + alpha * value2;

                points[i] = new Point(
                    _startX + i,
                    _startY + halfHeight + (y * halfHeight));
            }

            return points;
        }

        //  --------------------------------------------------------------------------------
        protected Point[] CalculateSmoothPoints()
        {
            double halfHeight = _borderedHeight / 2;
            double xStep = _borderedWidth / (SpectrumSize - 1);

            Point[] points = new Point[SpectrumSize];

            for (int i = 0; i < SpectrumSize; i++)
            {
                points[i] = new Point(
                    _startX + i * xStep,
                    _startY + halfHeight + (GetSinLevel(i) * halfHeight));
            }

            return points;
        }

        #endregion CALCULATION METHODS

        #region DRAWING METHODS

        //  --------------------------------------------------------------------------------
        private void AdvancedDraw(BitmapDrawer bitmapDrawer)
        {
            Point startPoint = new Point(
                _startX,
                _startY + _borderedHeight / 2.0);

            Point endPoint = new Point(
                _startX + _borderedWidth,
                _startY + _borderedHeight / 2.0);

            Point[] points = CalculateSharpPoints();

            Brush borderBrush = null;
            Brush fillBrush = null;
            int pointsCount = points.Length;

            UpdateRunTime();

            switch (ColorType)
            {
                case VisualisationColorType.RAINBOW_HORIZONTAL:
                    if (RainbowShift && IsRainbowTimeChangeReached())
                    {
                        _initBorderColor = ColorUtilities.UpdateColor(_initBorderColor, h: _initBorderColor.H + 1);
                        _initFillColor = ColorUtilities.UpdateColor(_initFillColor, h: _initFillColor.H + 1);
                    }

                    int hFHue = _initFillColor.H + (RainbowX / (int)_borderedWidth) * pointsCount;
                    var hFColor = ColorUtilities.UpdateColor(_initFillColor, h: hFHue);

                    fillBrush = RainbowColorGenerator.GetRainbowGradient(hFColor, RainbowX, pointsCount, pointsCount, direction: 180);
                    fillBrush.Opacity = _opacity;

                    if (BorderEnabled)
                    {
                        int hBHue = _initBorderColor.H + (RainbowX / (int)_borderedWidth) * pointsCount;
                        var hBColor = ColorUtilities.UpdateColor(_initBorderColor, h: hBHue);

                        borderBrush = RainbowColorGenerator.GetRainbowGradient(hBColor, RainbowX, pointsCount, pointsCount, direction: 180);
                        borderBrush.Opacity = _opacity;
                    }

                    break;

                case VisualisationColorType.RAINBOW_VERTICAL:
                    if (RainbowShift && IsRainbowTimeChangeReached())
                    {
                        _initBorderColor = ColorUtilities.UpdateColor(_initBorderColor, h: _initBorderColor.H + 1);
                        _initFillColor = ColorUtilities.UpdateColor(_initFillColor, h: _initFillColor.H + 1);
                    }

                    int vFHue = _initFillColor.H + (RainbowX / (int)_borderedWidth) * pointsCount;
                    var vFColor = ColorUtilities.UpdateColor(_initFillColor, h: vFHue);
                    int rDirection = GetRainbowDirection();
                    int rJump = GetRainbowJump();

                    fillBrush = RainbowColorGenerator.GetRainbowGradient(vFColor, rJump, pointsCount, pointsCount, direction: rDirection);
                    fillBrush.Opacity = _opacity;

                    if (BorderEnabled)
                    {
                        int vBHue = _initBorderColor.H + (RainbowX / (int)_borderedWidth) * pointsCount;
                        var vBColor = ColorUtilities.UpdateColor(_initBorderColor, h: vBHue);

                        borderBrush = RainbowColorGenerator.GetRainbowGradient(vBColor, rJump, pointsCount, pointsCount, direction: rDirection);
                        borderBrush.Opacity = _opacity;
                    }

                    break;

                case VisualisationColorType.CUSTOM:
                case VisualisationColorType.SYSTEM:
                default:
                    borderBrush = new SolidColorBrush(BorderColor) { Opacity = _opacity };
                    fillBrush = new SolidColorBrush(FillColor) { Opacity = _opacity };
                    break;
            }

            /*double minY = _yPoints.Min();
            double maxY = _yPoints.Max();
            double rangeY = maxY - minY;
            double centerY = _borderedHeight / 2.0;

            double stepY = rangeY != 0 ? _borderedHeight / rangeY : 0;*/

            StreamGeometry geometry = new StreamGeometry();

            /*using (StreamGeometryContext ctx = geometry.Open())
            {
                ctx.BeginFigure(new Point(_startX, _startY + centerY), false, false);

                for (int i = 0; i < _borderedWidth; i++)
                {
                    double x = _startX + i;
                    double y = _startY + centerY + _yPoints[i] * stepY;
                    ctx.LineTo(new Point(x, y), true, true);
                }
            }*/

            using (StreamGeometryContext ctx = geometry.Open())
            {
                ctx.BeginFigure(startPoint, false, false);

                for (int i = 1; i < points.Length - 1; i++)
                    ctx.LineTo(points[i], true, true);

                ctx.LineTo(endPoint, true, false);
            }

            bitmapDrawer.DrawFigure(FillEnabled ? fillBrush : null, new Pen(borderBrush ?? fillBrush, _borderThickness), geometry);
        }

        //  --------------------------------------------------------------------------------
        private void WaveDraw(BitmapDrawer bitmapDrawer)
        {
            Point startPoint = new Point(
                _startX,
                _startY + _borderedHeight / 2.0);

            Point endPoint = new Point(
                _startX + _borderedWidth,
                _startY + _borderedHeight / 2.0);

            Point[] points = CalculateSmoothPoints();

            Brush borderBrush = null;
            Brush fillBrush = null;
            int pointsCount = points.Length;

            UpdateRunTime();

            switch (ColorType)
            {
                case VisualisationColorType.RAINBOW_HORIZONTAL:
                    if (RainbowShift && IsRainbowTimeChangeReached())
                    {
                        _initBorderColor = ColorUtilities.UpdateColor(_initBorderColor, h: _initBorderColor.H + 1);
                        _initFillColor = ColorUtilities.UpdateColor(_initFillColor, h: _initFillColor.H + 1);
                    }

                    int hFHue = _initFillColor.H + (RainbowX / (int)_borderedWidth) * pointsCount;
                    var hFColor = ColorUtilities.UpdateColor(_initFillColor, h: hFHue);

                    fillBrush = RainbowColorGenerator.GetRainbowGradient(hFColor, RainbowX, pointsCount, pointsCount, direction: 180);
                    fillBrush.Opacity = _opacity;

                    if (BorderEnabled)
                    {
                        int hBHue = _initBorderColor.H + (RainbowX / (int)_borderedWidth) * pointsCount;
                        var hBColor = ColorUtilities.UpdateColor(_initBorderColor, h: hBHue);

                        borderBrush = RainbowColorGenerator.GetRainbowGradient(hBColor, RainbowX, pointsCount, pointsCount, direction: 180);
                        borderBrush.Opacity = _opacity;
                    }

                    break;

                case VisualisationColorType.RAINBOW_VERTICAL:
                    if (RainbowShift && IsRainbowTimeChangeReached())
                    {
                        _initBorderColor = ColorUtilities.UpdateColor(_initBorderColor, h: _initBorderColor.H + 1);
                        _initFillColor = ColorUtilities.UpdateColor(_initFillColor, h: _initFillColor.H + 1);
                    }

                    int vFHue = _initFillColor.H + (RainbowX / (int)_borderedWidth) * pointsCount;
                    var vFColor = ColorUtilities.UpdateColor(_initFillColor, h: vFHue);
                    int rDirection = GetRainbowDirection();
                    int rJump = GetRainbowJump();

                    fillBrush = RainbowColorGenerator.GetRainbowGradient(vFColor, rJump, pointsCount, pointsCount, direction: rDirection);
                    fillBrush.Opacity = _opacity;

                    if (BorderEnabled)
                    {
                        int vBHue = _initBorderColor.H + (RainbowX / (int)_borderedWidth) * pointsCount;
                        var vBColor = ColorUtilities.UpdateColor(_initBorderColor, h: vBHue);

                        borderBrush = RainbowColorGenerator.GetRainbowGradient(vBColor, rJump, pointsCount, pointsCount, direction: rDirection);
                        borderBrush.Opacity = _opacity;
                    }

                    break;

                case VisualisationColorType.CUSTOM:
                case VisualisationColorType.SYSTEM:
                default:
                    borderBrush = new SolidColorBrush(BorderColor) { Opacity = _opacity };
                    fillBrush = new SolidColorBrush(FillColor) { Opacity = _opacity };
                    break;
            }

            StreamGeometry geometry = new StreamGeometry();

            using (StreamGeometryContext ctx = geometry.Open())
            {
                ctx.BeginFigure(startPoint, true, false);

                for (int i = 0; i < points.Length - 1; i++)
                {
                    Point point1 = (i == 0) ? startPoint : points[i];
                    Point point2 = (i == points.Length - 2) ? endPoint : points[i+1];

                    Point midpoint = new Point((point1.X + point2.X) / 2, (point1.Y + point2.Y) / 2);

                    ctx.QuadraticBezierTo(point1, midpoint, true, true);
                }

                ctx.LineTo(endPoint, true, true);
            }

            bitmapDrawer.DrawFigure(FillEnabled ? fillBrush : null, new Pen(borderBrush ?? fillBrush, _borderThickness), geometry);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Draw visualisation. </summary>
        /// <returns> Bitmap drawer. </returns>
        public override BitmapDrawer Draw()
        {
            if (_spectrumProvider == null)
                return null;

            var bitmapDrawer = new BitmapDrawer(DrawingAreaSize);

            //AdvancedDraw(bitmapDrawer);
            //SimpleDraw(bitmapDrawer);
            WaveDraw(bitmapDrawer);

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
            }

            return new BitmapDrawer(DrawingAreaSize);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update graphics configuration. </summary>
        public override void UpdateGraphics()
        {
            _borderedHeight = DrawingAreaSize.Height - (Margin.Top + Margin.Bottom);
            _borderedWidth = DrawingAreaSize.Width - (Margin.Left + Margin.Right);
            _startX = Margin.Left;
            _startY = Margin.Top;
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
        /// <summary> Get spectrum level data as cos value. </summary>
        /// <param name="frequencyIndex"></param>
        /// <returns></returns>
        protected double GetCosLevel(int frequencyIndex)
        {
            return Math.Cos(_spectrumData[frequencyIndex].Value / (double)SpectrumSize * Math.PI * 2.0);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get spectrum level data as sin value. </summary>
        /// <param name="frequencyIndex"></param>
        /// <returns></returns>
        protected double GetSinLevel(int frequencyIndex)
        {
            return Math.Sin(_spectrumData[frequencyIndex].Value / (double)SpectrumSize * Math.PI * 2.0);
        }

        //  --------------------------------------------------------------------------------
        protected int GetRainbowDirection()
        {
            int baseDirection = 135;

            int xShift = 45 * RainbowX / 1530;
            int yShift = 45 * RainbowY / 1530;

            return baseDirection + xShift - yShift;
        }

        //  --------------------------------------------------------------------------------
        protected int GetRainbowJump()
        {
            if (RainbowX == 0)
                return RainbowY;

            if (RainbowY == 0)
                return RainbowX;

            return (RainbowX + RainbowY) / 2;
        }

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
