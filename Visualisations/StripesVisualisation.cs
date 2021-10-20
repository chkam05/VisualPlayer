using chkam05.Visualisations.Base;
using chkam05.Visualisations.Spectrum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;

namespace chkam05.Visualisations
{
    public class StripesVisualisation : BaseVisualisation, IVisualisation
    {

        //  VARIABLES

        private List<Rectangle> _peakSegments;

        private double _fallSpeed = 1;
        private Color _fillColor = (Color) ColorConverter.ConvertFromString("#0078D7");
        private Thickness _margin = new Thickness(0);
        private double _peakSpace = 4.0;

        private double _beatAverage = 1.0;
        private double _beatAverageMax = 0.75;
        private double _beatFallSpeed = 0.02;
        private double _beatSensitivity = 0.5;

        private double _firstX = 0;
        private double _spectrumHeight = 0;
        private SpectrumPosition[] _spectrumData = null;


        #region GETTERS & SETTERS
        
        public double FallSpeed
        {
            get => _fallSpeed;
            set => _fallSpeed = Math.Max(0.1, value);
        }

        public Color FillColor
        {
            get => _fillColor;
            set
            {
                _fillColor = value;

                if (_peakSegments.Any())
                    foreach (var peakSegment in _peakSegments)
                        peakSegment.Fill = new SolidColorBrush(value);
            }
        }

        public Thickness Margin
        {
            get => _margin;
            set => _margin = new Thickness(
                Math.Max(0, value.Left), Math.Max(0, value.Top), Math.Max(0, value.Right), Math.Max(0, value.Bottom));
        }

        public double PeakSpace
        {
            get => _peakSpace;
            set => _peakSpace = Math.Max(1, value);
        }

        #endregion GETTERS & SETTERS


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> BaseVisualisation class constructor. </summary>
        /// <param name="canvas"> Canvas where visualisation will be presented. </param>
        /// <param name="spectrumProvider"> Spectrum provider with FFT data. </param>
        public StripesVisualisation(Canvas canvas, SpectrumProvider spectrumProvider) : base(canvas, spectrumProvider)
        {
            _peakSegments = new List<Rectangle>();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Cleanup and free all used resources and variables. </summary>
        public override void Dispose()
        {
            ClearCanvas();
        }

        #endregion CLASS METHODS

        #region CALCULATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Calculate beat spectrum part. </summary>
        /// <param name="index"> Spectrum position index. </param>
        /// <param name="p"> Spectrum position. </param>
        /// <param name="currentBeatAverage"> Current calculated beat spectrum average. </param>
        /// <param name="beatAverageCount"> Current included spectrum parts for beat. </param>
        private void CalculateBeatPart(int index, SpectrumPosition p, ref double currentBeatAverage, ref int beatAverageCount)
        {
            double limiter = _spectrumHeight * _beatSensitivity;

            if (index < SpectrumSize && p.Value > limiter)
            {
                currentBeatAverage += (p.Value - limiter);
                beatAverageCount++;
            };
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Calculate beat final spectrum. </summary>
        /// <param name="average"> Beat spectrum average. </param>
        /// <param name="count"> Included spectrum parts for beat. </param>
        private void CalculateBeat(double average, int count)
        {
            double limiter = _spectrumHeight * _beatSensitivity;
            double newAverage = 1.0 + (count > 0 ? (((average / count) * _beatAverageMax) / limiter) : 0);

            _beatAverage = Math.Max(1.0,
                Math.Min(2.0,
                    newAverage > _beatAverage ? newAverage : _beatAverage - _beatFallSpeed));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Pre calculate visualisation graphics before drawing it. </summary>
        public override void PreCalculate()
        {
            if (_canvas == null || _spectrumProvider == null)
                return;

            //  Get buffer data from Player.
            var fftBuffer = new float[_fftSize];
            _spectrumProvider.GetFFTData(fftBuffer, this);

            _spectrumHeight = CanvasHeight - (Margin.Top + Margin.Bottom) - _firstX;

            //  Calculate spectrum line points.
            var spectrum = GetSpectrum(_spectrumHeight, fftBuffer, ScalingStrategy.SQRT, true, true);

            if (_spectrumData == null || _spectrumData.Length != spectrum.Count)
                _spectrumData = spectrum.ToArray();

            double beatAverage = 0;
            int beatAverageCount = 0;

            //  Pre calculation visualisation graphics.
            for (int iX = 0; iX < _spectrumData.Length; iX++)
            {
                if (_spectrumData[iX].Value < spectrum[iX].Value)
                {
                    _spectrumData[iX] = spectrum[iX];
                }
                else
                {
                    _spectrumData[iX].Value = Math.Max(0, _spectrumData[iX].Value - _fallSpeed);
                }

                if (_logoDrawer != null)
                    CalculateBeatPart(iX, _spectrumData[iX], ref beatAverage, ref beatAverageCount);
            }

            if (_logoDrawer != null)
                CalculateBeat(beatAverage, beatAverageCount);
        }

        #endregion CALCULATION METHODS

        #region DRAW METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Draw visualisation on canvas. </summary>
        /// <param name="includePreCalculation"> Include pre calculation visualisation graphics. </param>
        public override void Draw(bool includePreCalculation = false)
        {
            if (_canvas == null || _spectrumProvider == null)
                return;

            if (_enabled)
            {
                if (includePreCalculation || (_spectrumData == null || _spectrumData.Length == 0))
                    PreCalculate();

                for (int iX = 0; iX < _spectrumData.Length; iX++)
                {
                    var p = _spectrumData[iX];
                    _peakSegments[p.Index].Height = p.Value;
                }

                if (_logoDrawer != null)
                    _logoDrawer.ApplyScale(_beatAverage, _beatAverage);
            }   
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Stop visualisation working. </summary>
        public override void Stop()
        {
            if (_canvas != null && _spectrumData != null)
            {
                for (int iX = 0; iX < _spectrumData.Length; iX++)
                {
                    _spectrumData[iX].Value = 0;
                    var p = _spectrumData[iX];
                    _peakSegments[p.Index].Height = p.Value;
                }
            }
        }

        #endregion DRAW METHODS

        #region SETUP METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Setup canvas for drawing visualisation. </summary>
        private void SetupCanvas()
        {
            ClearCanvas();

            double width = CanvasWidth - (Margin.Left + Margin.Right + (_peakSpace * 2));
            double spacesSize = _peakSpace * (SpectrumSize - 1);
            double peakSize = (width - spacesSize) / SpectrumSize;

            _firstX = Margin.Left + _peakSpace;

            for (int iX = 0; iX < SpectrumSize; iX++)
            {
                double x = _firstX + (peakSize * iX) + (_peakSpace * iX);

                var peakSegment = new Rectangle();
                peakSegment.Fill = new SolidColorBrush(_fillColor);
                peakSegment.Height = 0;
                peakSegment.StrokeThickness = 0;
                peakSegment.Stroke = null;
                peakSegment.Width = peakSize;

                Canvas.SetLeft(peakSegment, x);
                Canvas.SetBottom(peakSegment, 0 + Margin.Bottom);

                _peakSegments.Add(peakSegment);
                Canvas.Children.Add(peakSegment);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Clear canvas. </summary>
        private void ClearCanvas()
        {
            if (_peakSegments.Count > 0)
                RemovePeaksFromCanvas(_peakSegments);

            _peakSegments.Clear();
            _initialized = false;
        }

        //  --------------------------------------------------------------------------------
        public override void UpdateGraphics(Canvas canvas = null, bool recalculate = true)
        {
            base.UpdateGraphics(canvas, recalculate);
            SetupCanvas();
        }

        #endregion SETUP METHODS

        #region UTILITY METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Remove peak segments from canvas. </summary>
        /// <param name="peakSegments"> List of peak segments to remove. </param>
        private void RemovePeaksFromCanvas(IEnumerable<Rectangle> peakSegments)
        {
            if (peakSegments != null && peakSegments.Any())
            {
                foreach (var peakSegment in peakSegments)
                {
                    if (Canvas.Children.Contains(peakSegment))
                        Canvas.Children.Remove(peakSegment);
                }
            }
        }

        #endregion UTILITY METHODS

    }
}
