using chkam05.Visualisations.Base;
using chkam05.Visualisations.Spectrum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace chkam05.Visualisations
{
    public static class VisualisationManager
    {

        //  METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Create visualisation. </summary>
        /// <param name="visualisationType"> Visualisation type. </param>
        /// <param name="canvas"> Canvas where visualisation will be presented. </param>
        /// <param name="spectrumProvider"> Spectrum provider with FFT data. </param>
        /// <returns> Created visualisation. </returns>
        public static IVisualisation CreateVisualisation(
            VisualisationType visualisationType, Canvas canvas, SpectrumProvider spectrumProvider = null)
        {
            switch (visualisationType)
            {
                case VisualisationType.StripesVisualisation:
                    var spectrumVisualisation = new StripesVisualisation(canvas, spectrumProvider);
                    return spectrumVisualisation;
            }

            return null;
        }

    }
}
