using chkam05.VisualPlayer.Data.Config;
using chkam05.VisualPlayer.Data.Config.Events;
using chkam05.VisualPlayer.Visualisations.Data;
using chkam05.VisualPlayer.Visualisations.Spectrum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace chkam05.VisualPlayer.Visualisations
{
    public class VisualisationsManager
    {

        //  CONST

        public readonly Dictionary<string, string> ConfigurationMapping = new Dictionary<string, string>
        {
            { nameof(Configuration.VisualisationBorderColor), nameof(StripesVisualisation.BorderColor) },
            { nameof(Configuration.VisualisationBorderEnabled), nameof(StripesVisualisation.BorderEnabled) },
            { nameof(Configuration.VisualisationColor), nameof(StripesVisualisation.FillColor) },
            { nameof(Configuration.VisualisationColorOpacity), nameof(StripesVisualisation.Opacity) },
            { nameof(Configuration.VisualisationColorType), nameof(StripesVisualisation.ColorType) },
            { nameof(Configuration.VisualisationRainbowChangeTime), nameof(StripesVisualisation.RainbowChangeTime) },
            { nameof(Configuration.VisualisationRainbowShift), nameof(StripesVisualisation.RainbowShift) },
            { nameof(Configuration.VisualisationRainbowXShift), nameof(StripesVisualisation.RainbowX) },
            { nameof(Configuration.VisualisationRainbowYShift), nameof(StripesVisualisation.RainbowY) },
            { nameof(Configuration.VisualisationScalingStrategy), nameof(BaseVisualisation.ScalingStrategy) },
            { nameof(Configuration.VisualisationStripesFallSpeed), nameof(StripesVisualisation.FallSpeed) }
        };


        //  VARIABLES

        private static VisualisationsManager _instance;

        private IVisualisation _visualisation;


        //  GETTERS & SETTERS

        public static VisualisationsManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new VisualisationsManager();

                return _instance;
            }
        }

        public bool Created
        {
            get => _visualisation != null;
        }

        public bool Initialized
        {
            get => _visualisation != null && _visualisation.Initialized;
        }

        public IVisualisation Visualisation
        {
            get => _visualisation;
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> VisualisationsManager private class constructor. </summary>
        private VisualisationsManager()
        {
            //
        }

        #endregion CLASS METHODS

        #region MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Create visualisation. </summary>
        /// <param name="type"> Visualisation type. </param>
        /// <param name="spectrumProvider"> Spectrum provider with FFT data. </param>
        /// <param name="drawingAreaSize"> Size of drawing visualisation area. </param>
        public void Create(VisualisationType type, SpectrumProvider spectrumProvider = null, Size? drawingAreaSize = null)
        {
            switch (type)
            {
                case VisualisationType.None:
                    _visualisation = null;
                    break;

                case VisualisationType.StripesVisualisation:
                    _visualisation = new StripesVisualisation(spectrumProvider);
                    break;

                case VisualisationType.PeaksVisualisation:
                    _visualisation = new PeaksVisualisation(spectrumProvider);
                    break;
            }

            if (_visualisation != null && drawingAreaSize.HasValue)
                _visualisation.DrawingAreaSize = drawingAreaSize.Value;
        }

        #endregion MANAGEMENT METHODS

        #region UPDATE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Update visualisation configuration from settings. </summary>
        /// <param name="config"> Configuration with visualisation settings. </param>
        public void UpdateConfiguration(Configuration config)
        {
            if (config != null && _visualisation != null)
            {
                //  Update standard configuration.
                foreach (var map in ConfigurationMapping)
                {
                    if (config.TryGetValue(map.Key, out object value))
                        Visualisation.SetProperty(map.Value, value);
                }

                //  Update extended configuration.
                _visualisation.BeatLevel.BeatFallSpeed = config.VisualisationBeatFallSpeed;
                _visualisation.BeatLevel.BeatSensitivity = config.VisualisationBeatSensitivity;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update single visualisation configuration from settings. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Configuration Update Event Arguments. </param>
        public void UpdateConfiguration(object sender, ConfigurationUpdateEventArgs e)
        {
            if (e.PropertyValue != null && _visualisation != null)
            {
                //  Update standard configuration.
                if (ConfigurationMapping.TryGetValue(e.PropertyName, out string propertyName))
                    Visualisation.SetProperty(propertyName, e.PropertyValue);

                //  Update extended configuration
                else
                {
                    var config = sender as Configuration;

                    switch (e.PropertyName)
                    {
                        case nameof(Configuration.VisualisationBeatFallSpeed):
                            _visualisation.BeatLevel.BeatFallSpeed = config.VisualisationBeatFallSpeed;
                            break;

                        case nameof(Configuration.VisualisationBeatSensitivity):
                            _visualisation.BeatLevel.BeatSensitivity = config.VisualisationBeatSensitivity;
                            break;
                    }
                }
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update visualisation drawing area size. </summary>
        /// <param name="drawingAreaSize"> Visualisation drawing area size. </param>
        public void UpdateDrawingAreaSize(Size drawingAreaSize)
        {
            if (_visualisation != null)
                _visualisation.DrawingAreaSize = drawingAreaSize;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update visualisation spectrum provider. </summary>
        /// <param name="spectrumProvider"> Spectrum provider. </param>
        public void UpdateSpectrumProvider(SpectrumProvider spectrumProvider)
        {
            if (_visualisation != null)
                _visualisation.SetupSpectrumProvider(spectrumProvider, true);
        }

        #endregion UPDATE METHODS

    }
}
