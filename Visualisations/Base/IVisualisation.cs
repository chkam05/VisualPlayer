using chkam05.Visualisations.LogoDrawing;
using chkam05.Visualisations.Spectrum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace chkam05.Visualisations.Base
{
    public interface IVisualisation : IDisposable
    {

        //  VARIABLES

        bool Enabled { get; set; }
        bool Initialized { get; }
        ShapeDrawer LogoDrawer { get; }
        int SpectrumSize { get; set; }


        //  METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Pre calculate visualisation graphics before drawing it. </summary>
        void PreCalculate();

        //  --------------------------------------------------------------------------------
        /// <summary> Draw visualisation on canvas. </summary>
        /// <param name="includePreCalculation"> Include pre calculation visualisation graphics. </param>
        void Draw(bool includePreCalculation = false);

        //  --------------------------------------------------------------------------------
        /// <summary> Stop visualisation working. </summary>
        void Stop();

        //  --------------------------------------------------------------------------------
        /// <summary> Set spectrum provider. </summary>
        /// <param name="spectrumProvider"> Spectrum provider with FFT data. </param>
        /// <param name="recalculate"> Remap frequency. </param>
        void SetSpectrumProvider(SpectrumProvider spectrumProvider, bool recalculate = true);

        //  --------------------------------------------------------------------------------
        /// <summary> Set new canvas. </summary>
        /// <param name="canvas"> Canvas where visualisation will be presented. </param>
        /// <param name="recalculate"> Remap frequency. </param>
        void UpdateGraphics(Canvas canvas = null, bool recalculate = true);

    }
}
