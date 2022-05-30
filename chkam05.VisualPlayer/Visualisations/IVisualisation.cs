using chkam05.VisualPlayer.Visualisations.Data;
using chkam05.VisualPlayer.Visualisations.Spectrum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace chkam05.VisualPlayer.Visualisations
{
    public interface IVisualisation
    {

        //  GETTERS & SETTERS

        BeatLevel BeatLevel { get; }
        Size DrawingAreaSize { get; set; }
        bool Initialized { get; }
        Thickness Margin { get; set; }
        PropertyInfo[] Properties { get; }
        ScalingStrategy ScalingStrategy { get; set; }
        int SpectrumSize { get; set; }


        //  METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Pre calculate visualisation graphics before drawing it. </summary>
        void PreCalculate();

        //  --------------------------------------------------------------------------------
        /// <summary> Draw visualisation. </summary>
        /// <returns> Bitmap drawer. </returns>
        BitmapDrawer Draw();

        //  --------------------------------------------------------------------------------
        /// <summary> Get beat level value. </summary>
        /// <returns> Beat level value. </returns>
        double GetBeatLevel();

        //  --------------------------------------------------------------------------------
        /// <summary> Set visualisation property. </summary>
        /// <param name="propertyName"> Property name. </param>
        /// <param name="value"> Value to set. </param>
        /// <returns> True - value set; False - otherwise. </returns>
        bool SetProperty(string propertyName, object value);

        //  --------------------------------------------------------------------------------
        /// <summary> Stop drawing visualisation. </summary>
        /// <returns> Bitmap drawer. </returns>
        BitmapDrawer StopDrawing();

        //  --------------------------------------------------------------------------------
        /// <summary> Update graphics configuration. </summary>
        void UpdateGraphics();

        //  --------------------------------------------------------------------------------
        /// <summary> Setup spectrum provider. </summary>
        /// <param name="spectrumProvider"> Spectrum provider. </param>
        /// <param name="remap"> Remap frequency. </param>
        void SetupSpectrumProvider(SpectrumProvider spectrumProvider, bool remap = false);

    }
}
