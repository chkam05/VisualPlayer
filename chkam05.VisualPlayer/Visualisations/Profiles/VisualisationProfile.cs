using chkam05.VisualPlayer.Visualisations.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace chkam05.VisualPlayer.Visualisations.Profiles
{
    public class VisualisationProfile
    {

        //  CONST

        private static readonly Color DEFAULT_COLOR = Color.FromArgb(255, 0, 120, 215);
        public static readonly string DEFAULT_PROFILE_NAME = "Default";


        //  VARIABLES

        public string Name { get; set; }
        public double AnimationSpeed { get; set; } = 8.0d;
        public Color BorderColor { get; set; } = DEFAULT_COLOR;
        public bool BorderEnabled { get; set; } = false;
        public Color Color { get; set; } = DEFAULT_COLOR;
        public double ColorOpacity { get; set; } = 0.8d;
        public VisualisationColorType ColorType { get; set; } = VisualisationColorType.SYSTEM;
        public bool FillEnabled { get; set; } = true;
        public double RainbowChangeTime { get; set; } = 0.0d;
        public bool RainbowShift { get; set; } = false;
        public int RainbowXShift { get; set; } = 0;
        public int RainbowYShift { get; set; } = 1000;
        public ScalingStrategy ScalingStrategy { get; set; } = ScalingStrategy.SQRT;
        public VisualisationType Type { get; set; } = VisualisationType.PeaksVisualisation;


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> VisualisationProfile class constructor. </summary>
        public VisualisationProfile()
        {
            //
        }

        //  --------------------------------------------------------------------------------
        /// <summary> VisualisationProfile default profile class constructor. </summary>
        /// <returns> Default VisualisationProfile. </returns>
        public static VisualisationProfile DefaultProfile => new VisualisationProfile()
        {
            Name = DEFAULT_PROFILE_NAME
        };

        #endregion CLASS METHODS

    }
}
