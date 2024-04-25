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
    public class PeaksCenterVisualisation : PeaksVisualisation
    {

        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> PeaksCenterVisualisation class constructor. </summary>
        /// <param name="spectrumProvider"> Spectrum provider with FFT data. </param>
        public PeaksCenterVisualisation(SpectrumProvider spectrumProvider) : base(spectrumProvider)
        {
            //
        }

        #endregion CLASS METHODS

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

            Brush borderBrush2 = null;
            Brush fillBrush2 = null;

            UpdateRunTime();

            switch (ColorType)
            {
                case VisualisationColorType.RAINBOW_HORIZONTAL:
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

                    borderBrush2 = new SolidColorBrush(BorderColor) { Opacity = _opacity };
                    fillBrush2 = new SolidColorBrush(FillColor) { Opacity = _opacity };
                    break;
            }

            for (int iX = 0; iX < _spectrumData.Length; iX++)
            {
                var level = _spectrumData[iX];
                var pX = _firstX + (_stripeWidth * iX + _peakSpaceX * iX);
                var pY = Margin.Top + (DrawingAreaSize.Height - Margin.Top - Margin.Bottom ) / 2;
                var pJump = _peakHeight + _peakSpaceY;
                double rainbowY = RainbowY * pJump / (int)_peakCountY;

                AHSLColor fColor = _initFillColor;
                AHSLColor bColor = _initBorderColor;

                switch (ColorType)
                {
                    case VisualisationColorType.RAINBOW_HORIZONTAL:
                    case VisualisationColorType.RAINBOW_VERTICAL:
                        fColor = ColorUtilities.UpdateColor(_initFillColor, h: _initFillColor.H + (RainbowX / SpectrumSize) * iX);
                        fillBrush = new SolidColorBrush(fColor.ToColor()) { Opacity = _opacity };
                        fillBrush2 = new SolidColorBrush(fColor.ToColor()) { Opacity = _opacity };

                        if (BorderEnabled)
                        {
                            bColor = ColorUtilities.UpdateColor(_initBorderColor, h: _initBorderColor.H + (RainbowX / SpectrumSize) * iX);
                            borderBrush = new SolidColorBrush(bColor.ToColor()) { Opacity = _opacity };
                            borderBrush2 = new SolidColorBrush(bColor.ToColor()) { Opacity = _opacity };
                        }

                        break;

                    default:
                        break;
                }

                var yLimiter = pY - level.Value / 2 + pJump;

                for (double iY = pY; iY > yLimiter; iY -= pJump)
                {
                    var point = new Point(pX, iY + _peakHeight);
                    var size = new Size(_stripeWidth, _peakHeight);

                    var point2 = new Point(pX, pY + _peakHeight + (pY - iY));
                    var size2 = new Size(_stripeWidth, _peakHeight);

                    if (ColorType == VisualisationColorType.RAINBOW_VERTICAL)
                    {
                        fColor = ColorUtilities.UpdateColor(fColor, h: fColor.H + (int)Math.Round(rainbowY));
                        fillBrush = new SolidColorBrush(fColor.ToColor()) { Opacity = _opacity };
                        fillBrush2 = new SolidColorBrush(fColor.ToColor()) { Opacity = _opacity };

                        if (BorderEnabled)
                        {
                            bColor = ColorUtilities.UpdateColor(bColor, h: bColor.H + (int)Math.Round(rainbowY));
                            borderBrush = new SolidColorBrush(bColor.ToColor()) { Opacity = _opacity };
                            borderBrush2 = new SolidColorBrush(bColor.ToColor()) { Opacity = _opacity };
                        }
                    }

                    var pen = BorderEnabled ? new Pen(borderBrush, 1.0) : null;

                    if (point.Y > Margin.Top)
                        bitmapDrawer.DrawRectangle(fillBrush, pen, point, size);
                    
                    if (point2.Y < DrawingAreaSize.Height - Margin.Bottom - pJump)
                        bitmapDrawer.DrawRectangle(fillBrush, pen, point2, size2);
                }
            }

            return bitmapDrawer;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update graphics configuration. </summary>
        public override void UpdateGraphics()
        {
            double width = DrawingAreaSize.Width - (Margin.Left + Margin.Right + (_peakSpaceX * 2));
            double spacesSize = _peakSpaceX * (SpectrumSize - 1);

            _firstX = Margin.Left + _peakSpaceX;
            _stripeWidth = (width - spacesSize) / SpectrumSize;
            _peakHeight = _stripeWidth * 0.5;
            _peakCountY = (DrawingAreaSize.Height - (Margin.Bottom + Margin.Top) - (_peakHeight * 2) / (_peakHeight + _peakSpaceY));
        }

        #endregion DRAWING METHODS

    }
}
