using chkam05.VisualPlayer.Data.Config;
using chkam05.VisualPlayer.Data.Configuration;
using chkam05.VisualPlayer.Data.Configuration.Events;
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
                
                case VisualisationType.StripesReversedVisualisation:
                    _visualisation = new StripesReversedVisualisation(spectrumProvider);
                    break;
                
                case VisualisationType.StripesCenterVisualisation:
                    _visualisation = new StripesCenterVisualisation(spectrumProvider);
                    break;
                
                case VisualisationType.StripesCenterCollapseVisualisation:
                    _visualisation = new StripesCenterCollapseVisualisation(spectrumProvider);
                    break;

                case VisualisationType.StripesCenterCollapseExtendedVisualisation:
                    _visualisation = new StripesCenterCollapseExtendedVisualisation(spectrumProvider);
                    break;

                case VisualisationType.PeaksVisualisation:
                    _visualisation = new PeaksVisualisation(spectrumProvider);
                    break;
                
                case VisualisationType.PeaksReversedVisualisation:
                    _visualisation = new PeaksReversedVisualisation(spectrumProvider);
                    break;
                
                case VisualisationType.PeaksCenterVisualisation:
                    _visualisation = new PeaksCenterVisualisation(spectrumProvider);
                    break;
                
                case VisualisationType.PeaksCenterCollapseVisualisation:
                    _visualisation = new PeaksCenterCollapseVisualisation(spectrumProvider);
                    break;

                case VisualisationType.PeaksCenterCollapseExtendedVisualisation:
                    _visualisation = new PeaksCenterCollapseExtendedVisualisation(spectrumProvider);
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
        public void UpdateConfiguration(ConfigManager configManager)
        {
            if (configManager != null && _visualisation != null)
            {
                //  Update standard configuration.
                Visualisation.SetProperty("BorderColor", configManager.VisualisationBorderColor);
                Visualisation.SetProperty("BorderEnabled", configManager.VisualisationBorderEnabled);
                Visualisation.SetProperty("FillColor", configManager.VisualisationColor);
                Visualisation.SetProperty("Opacity", configManager.VisualisationColorOpacity);
                Visualisation.SetProperty("ColorType", configManager.VisualisationColorType);
                Visualisation.SetProperty("RainbowChangeTime", configManager.VisualisationRainbowChangeTime);
                Visualisation.SetProperty("RainbowShift", configManager.VisualisationRainbowShift);
                Visualisation.SetProperty("RainbowX", configManager.VisualisationRainbowXShift);
                Visualisation.SetProperty("RainbowY", configManager.VisualisationRainbowYShift);
                Visualisation.SetProperty("ScalingStrategy", configManager.VisualisationScalingStrategy);
                Visualisation.SetProperty("FallSpeed", configManager.VisualisationAnimationSpeed);

                //  Update extended configuration.
                _visualisation.BeatLevel.BeatFallSpeed = configManager.LogoAnimationSpeed;
                _visualisation.BeatLevel.BeatSensitivity = configManager.LogoAnimationSensitivity;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update single visualisation configuration from settings. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Configuration Update Event Arguments. </param>
        public void UpdateConfiguration(object sender, ConfigUpdateEventArgs e)
        {
            UpdateConfiguration((ConfigManager)sender);
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
