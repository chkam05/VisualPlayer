using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.Visualisations.Spectrum
{
    public interface ISpectrumProvider
    {

        //  METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get FFT band index. </summary>
        /// <param name="frequency"> Frequency. </param>
        /// <returns> Index of band with specified frequency. </returns>
        int GetFFTBandIndex(float frequency);

        //  --------------------------------------------------------------------------------
        /// <summary> Get FFT data. </summary>
        /// <param name="fftResultBuffer"> Result buffer of FFT data. </param>
        /// <param name="context"> Context. </param>
        /// <returns> True - new data gets; False - otherwise. </returns>
        bool GetFFTData(float[] fftResultBuffer, object context);

    }
}
