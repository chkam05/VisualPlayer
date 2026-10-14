using System;
using System.Collections.Generic;
using VisualPlayer.Enums;
using VisualPlayer.Visualizations.Config;
using VisualPlayer.Visualizations.Config.Base;

namespace VisualPlayer.Resources;

public static class DefaultVisualizationProfiles
{
    #region CONST

    public const string NONE_NAME = "None";
    public const string CIRCLE_STRIPES_NAME = "Circle - Stripes";
    public const string CIRCLE_WAVEFORM_NAME = "Circle - Waveform";
    public const string OSCILLOSCOPE_CENTER_NAME = "Oscilloscope - Center";
    public const string OSCILLOSCOPE_DOUBLE_NAME = "Oscilloscope - Double";
    public const string OSCILLOSCOPE_FILLED_NAME = "Oscilloscope - Filled";
    public const string OSCILLOSCOPE_NAME = "Oscilloscope";
    public const string PEAKS_FLOATING_MIRRORED_NAME = "Peaks - Floating Mirrored";
    public const string PEAKS_FLOATING_NAME = "Peaks - Floating";
    public const string PEAKS_MIRRORED_NAME = "Peaks - Mirrored";
    public const string PEAKS_NAME = "Peaks";
    public const string STRIPES_MIRRORED_NAME = "Stripes - Mirrored";
    public const string STRIPES_NAME = "Stripes";
    public const string WAVEFORM_NAME = "Waveform";
    public const string WAVEFORM_MIRRORED_NAME = "Waveform - Mirrored";

    #endregion CONST

    #region CREATE

    public static IReadOnlyList<VisualisationProfile> Create()
    {
        return
        [
            CreateNone(),
            CreateCircleStripes(),
            CreateCircleWaveform(),
            CreateOscilloscope(),
            CreateOscilloscopeCenter(),
            CreateOscilloscopeDouble(),
            CreateOscilloscopeFilled(),
            CreatePeaks(),
            CreatePeaksFloating(),
            CreatePeaksFloatingMirrored(),
            CreatePeaksMirrored(),
            CreateStripes(),
            CreateStripesMirrored(),
            CreateWaveform(),
            CreateWaveformMirrored()
        ];
    }

    public static VisualisationProfile? Create(string name)
    {
        return name.ToUpperInvariant() switch
        {
            "NONE" => CreateNone(),
            "CIRCLE - STRIPES" => CreateCircleStripes(),
            "CIRCLE - WAVEFORM" => CreateCircleWaveform(),
            "PEAKS" => CreatePeaks(),
            "STRIPES" => CreateStripes(),
            "PEAKS - MIRRORED" => CreatePeaksMirrored(),
            "STRIPES - MIRRORED" => CreateStripesMirrored(),
            "PEAKS - FLOATING" => CreatePeaksFloating(),
            "PEAKS - FLOATING MIRRORED" => CreatePeaksFloatingMirrored(),
            "OSCILLOSCOPE" => CreateOscilloscope(),
            "OSCILLOSCOPE - DOUBLE" => CreateOscilloscopeDouble(),
            "OSCILLOSCOPE - FILLED" => CreateOscilloscopeFilled(),
            "OSCILLOSCOPE - CENTER" => CreateOscilloscopeCenter(),
            "WAVEFORM" => CreateWaveform(),
            "WAVEFORM - MIRRORED" => CreateWaveformMirrored(),
            _ => null
        };
    }

    public static bool IsCompatible(VisualisationProfile profile)
    {
        return profile.VisualizationType switch
        {
            VisualizationType.None => profile.Config is null,
            VisualizationType.CircleStripes => profile.Config is CircleStripesVisualizationConfig,
            VisualizationType.CircleWaveform => profile.Config is CircleWaveformVisualizationConfig,
            VisualizationType.Peaks => profile.Config is PeaksVisualizationConfig,
            VisualizationType.Stripes => profile.Config is StripesVisualizationConfig,
            VisualizationType.PeaksMirrored => profile.Config is PeaksMirroredVisualizationConfig,
            VisualizationType.StripesMirrored => profile.Config is StripesMirroredVisualizationConfig,
            VisualizationType.PeaksFloating => profile.Config is PeaksFloatingVisualizationConfig,
            VisualizationType.PeaksFloatingMirrored => profile.Config is PeaksFloatingMirroredVisualizationConfig,
            VisualizationType.Oscilloscope => profile.Config is OscilloscopeVisualizationConfig,
            VisualizationType.OscilloscopeDouble => profile.Config is OscilloscopeDoubleVisualizationConfig,
            VisualizationType.OscilloscopeFilled => profile.Config is OscilloscopeFilledVisualizationConfig,
            VisualizationType.OscilloscopeCenter => profile.Config is OscilloscopeCenterVisualizationConfig,
            VisualizationType.Waveform => profile.Config is WaveformVisualizationConfig,
            VisualizationType.WaveformMirrored => profile.Config is WaveformMirroredVisualizationConfig,
            _ => false
        };
    }

    public static bool IsDefaultName(string name)
    {
        return Create(name) is not null;
    }


    #region Profiles

    private static VisualisationProfile CreateNone()
    {
        return new VisualisationProfile
        {
            IsDefault = true,
            Name = NONE_NAME,
            VisualizationType = VisualizationType.None,
            Config = null,
            Color = null,
        };
    }

    private static VisualisationProfile CreateCircleStripes()
    {
        return new VisualisationProfile
        {
            Color = new VisualizationColorConfig
            {
                Gradient = new GradientDefinition()
                {
                    Angle = 0,
                    Points =
                    [
                        CreateGradientPoint("#10893E", 0),
                        CreateGradientPoint("#10893E", 0.5),
                        CreateGradientPoint("#FFB900", 0.75),
                        CreateGradientPoint("#E81123", 1),
                    ],
                    Type = GradientType.Radial
                },
                Mode = VisualizationColorMode.Gradient,
                SolidColor = new("#10893E", string.Empty)
            },
            Config = new CircleStripesVisualizationConfig(),
            IsDefault = true,
            Name = CIRCLE_STRIPES_NAME,
            VisualizationType = VisualizationType.CircleStripes,
        };
    }

    private static VisualisationProfile CreateCircleWaveform()
    {
        return new VisualisationProfile
        {
            Color = new VisualizationColorConfig
            {
                Gradient = new GradientDefinition()
                {
                    Angle = 0,
                    Points =
                    [
                        CreateGradientPoint("#10893E", 0),
                        CreateGradientPoint("#10893E", 0.5),
                        CreateGradientPoint("#FFB900", 0.75),
                        CreateGradientPoint("#E81123", 1),
                    ],
                    Type = GradientType.Radial
                },
                Mode = VisualizationColorMode.Gradient,
                SolidColor = new("#10893E", string.Empty)
            },
            Config = new CircleWaveformVisualizationConfig(),
            IsDefault = true,
            Name = CIRCLE_WAVEFORM_NAME,
            VisualizationType = VisualizationType.CircleWaveform,
        };
    }

    private static VisualisationProfile CreateOscilloscope()
    {
        return new VisualisationProfile
        {
            Color = new VisualizationColorConfig
            {
                Gradient = new GradientDefinition()
                {
                    Angle = 270,
                    Points =
                    [
                        CreateGradientPoint("#E81123", 0),
                        CreateGradientPoint("#FFB900", 0.166),
                        CreateGradientPoint("#10893E", 0.332),
                        CreateGradientPoint("#10893E", 0.668),
                        CreateGradientPoint("#FFB900", 0.834),
                        CreateGradientPoint("#E81123", 1),
                    ],
                    Type = GradientType.Linear
                },
                Mode = VisualizationColorMode.Rainbow,
                SolidColor = new("#10893E", string.Empty)
            },
            Config = new OscilloscopeVisualizationConfig(),
            IsDefault = true,
            Name = OSCILLOSCOPE_NAME,
            VisualizationType = VisualizationType.Oscilloscope,
        };
    }

    private static VisualisationProfile CreateOscilloscopeCenter()
    {
        return new VisualisationProfile
        {
            Color = new VisualizationColorConfig
            {
                Gradient = new GradientDefinition()
                {
                    Angle = 270,
                    Points =
                    [
                        CreateGradientPoint("#E81123", 0),
                        CreateGradientPoint("#FFB900", 0.166),
                        CreateGradientPoint("#10893E", 0.332),
                        CreateGradientPoint("#10893E", 0.668),
                        CreateGradientPoint("#FFB900", 0.834),
                        CreateGradientPoint("#E81123", 1),
                    ],
                    Type = GradientType.Linear
                },
                Mode = VisualizationColorMode.Rainbow,
                SolidColor = new("#10893E", string.Empty)
            },
            Config = new OscilloscopeCenterVisualizationConfig(),
            IsDefault = true,
            Name = OSCILLOSCOPE_CENTER_NAME,
            VisualizationType = VisualizationType.OscilloscopeCenter,
        };
    }

    private static VisualisationProfile CreateOscilloscopeDouble()
    {
        return new VisualisationProfile
        {
            Color = new VisualizationColorConfig
            {
                Gradient = new GradientDefinition()
                {
                    Angle = 270,
                    Points =
                    [
                        CreateGradientPoint("#E81123", 0),
                        CreateGradientPoint("#FFB900", 0.166),
                        CreateGradientPoint("#10893E", 0.332),
                        CreateGradientPoint("#10893E", 0.668),
                        CreateGradientPoint("#FFB900", 0.834),
                        CreateGradientPoint("#E81123", 1),
                    ],
                    Type = GradientType.Linear
                },
                Mode = VisualizationColorMode.Rainbow,
                SolidColor = new("#10893E", string.Empty)
            },
            Config = new OscilloscopeDoubleVisualizationConfig(),
            IsDefault = true,
            Name = OSCILLOSCOPE_DOUBLE_NAME,
            VisualizationType = VisualizationType.OscilloscopeDouble,
        };
    }

    private static VisualisationProfile CreateOscilloscopeFilled()
    {
        return new VisualisationProfile
        {
            Color = new VisualizationColorConfig
            {
                Gradient = new GradientDefinition()
                {
                    Angle = 270,
                    Points =
                    [
                        CreateGradientPoint("#E81123", 0),
                        CreateGradientPoint("#FFB900", 0.166),
                        CreateGradientPoint("#10893E", 0.332),
                        CreateGradientPoint("#10893E", 0.668),
                        CreateGradientPoint("#FFB900", 0.834),
                        CreateGradientPoint("#E81123", 1),
                    ],
                    Type = GradientType.Linear
                },
                Mode = VisualizationColorMode.Rainbow,
                SolidColor = new("#10893E", string.Empty)
            },
            Config = new OscilloscopeFilledVisualizationConfig(),
            IsDefault = true,
            Name = OSCILLOSCOPE_FILLED_NAME,
            VisualizationType = VisualizationType.OscilloscopeFilled,
        };
    }

    private static VisualisationProfile CreatePeaks()
    {
        return new VisualisationProfile
        {
            Color = new VisualizationColorConfig
            {
                Gradient = new GradientDefinition()
                {
                    Angle = 270,
                    Points =
                    [
                        CreateGradientPoint("#10893E", 0),
                        CreateGradientPoint("#10893E", 0.33),
                        CreateGradientPoint("#FFB900", 0.66),
                        CreateGradientPoint("#E81123", 1),
                    ],
                    Type = GradientType.Linear
                },
                Mode = VisualizationColorMode.Gradient,
                SolidColor = new("#10893E", string.Empty)
            },
            Config = new PeaksVisualizationConfig(),
            IsDefault = true,
            Name = PEAKS_NAME,
            VisualizationType = VisualizationType.Peaks,
        };
    }

    private static VisualisationProfile CreatePeaksFloating()
    {
        return new VisualisationProfile
        {
            Color = new VisualizationColorConfig
            {
                Gradient = new GradientDefinition()
                {
                    Angle = 270,
                    Points =
                    [
                        CreateGradientPoint("#10893E", 0),
                        CreateGradientPoint("#10893E", 0.33),
                        CreateGradientPoint("#FFB900", 0.66),
                        CreateGradientPoint("#E81123", 1),
                    ],
                    Type = GradientType.Linear
                },
                Mode = VisualizationColorMode.Gradient,
                SolidColor = new("#10893E", string.Empty)
            },
            Config = new PeaksFloatingVisualizationConfig(),
            IsDefault = true,
            Name = PEAKS_FLOATING_NAME,
            VisualizationType = VisualizationType.PeaksFloating,
        };
    }

    private static VisualisationProfile CreatePeaksFloatingMirrored()
    {
        return new VisualisationProfile
        {
            Color = new VisualizationColorConfig
            {
                Gradient = new GradientDefinition()
                {
                    Angle = 270,
                    Points =
                    [
                        CreateGradientPoint("#E81123", 0),
                        CreateGradientPoint("#FFB900", 0.166),
                        CreateGradientPoint("#10893E", 0.332),
                        CreateGradientPoint("#10893E", 0.668),
                        CreateGradientPoint("#FFB900", 0.834),
                        CreateGradientPoint("#E81123", 1),
                    ],
                    Type = GradientType.Linear
                },
                Mode = VisualizationColorMode.Gradient,
                SolidColor = new("#10893E", string.Empty)
            },
            Config = new PeaksFloatingMirroredVisualizationConfig(),
            IsDefault = true,
            Name = PEAKS_FLOATING_MIRRORED_NAME,
            VisualizationType = VisualizationType.PeaksFloatingMirrored,
        };
    }

    private static VisualisationProfile CreatePeaksMirrored()
    {
        return new VisualisationProfile
        {
            Color = new VisualizationColorConfig
            {
                Gradient = new GradientDefinition()
                {
                    Angle = 270,
                    Points =
                    [
                        CreateGradientPoint("#E81123", 0),
                        CreateGradientPoint("#FFB900", 0.166),
                        CreateGradientPoint("#10893E", 0.332),
                        CreateGradientPoint("#10893E", 0.668),
                        CreateGradientPoint("#FFB900", 0.834),
                        CreateGradientPoint("#E81123", 1),
                    ],
                    Type = GradientType.Linear
                },
                Mode = VisualizationColorMode.Gradient,
                SolidColor = new("#10893E", string.Empty)
            },
            Config = new PeaksMirroredVisualizationConfig(),
            IsDefault = true,
            Name = PEAKS_MIRRORED_NAME,
            VisualizationType = VisualizationType.PeaksMirrored,
        };
    }

    private static VisualisationProfile CreateStripes()
    {
        return new VisualisationProfile
        {
            Color = new VisualizationColorConfig
            {
                Gradient = new GradientDefinition()
                {
                    Angle = 270,
                    Points =
                    [
                        CreateGradientPoint("#10893E", 0),
                        CreateGradientPoint("#10893E", 0.33),
                        CreateGradientPoint("#FFB900", 0.66),
                        CreateGradientPoint("#E81123", 1),
                    ],
                    Type = GradientType.Linear
                },
                Mode = VisualizationColorMode.Gradient,
                SolidColor = new("#10893E", string.Empty)
            },
            Config = new StripesVisualizationConfig(),
            IsDefault = true,
            Name = STRIPES_NAME,
            VisualizationType = VisualizationType.Stripes,
        };
    }

    private static VisualisationProfile CreateStripesMirrored()
    {
        return new VisualisationProfile
        {
            Color = new VisualizationColorConfig
            {
                Gradient = new GradientDefinition()
                {
                    Angle = 270,
                    Points =
                    [
                        CreateGradientPoint("#E81123", 0),
                        CreateGradientPoint("#FFB900", 0.166),
                        CreateGradientPoint("#10893E", 0.332),
                        CreateGradientPoint("#10893E", 0.668),
                        CreateGradientPoint("#FFB900", 0.834),
                        CreateGradientPoint("#E81123", 1),
                    ],
                    Type = GradientType.Linear
                },
                Mode = VisualizationColorMode.Gradient,
                SolidColor = new("#10893E", string.Empty)
            },
            Config = new StripesMirroredVisualizationConfig(),
            IsDefault = true,
            Name = STRIPES_MIRRORED_NAME,
            VisualizationType = VisualizationType.StripesMirrored,
        };
    }

    private static VisualisationProfile CreateWaveform()
    {
        return new VisualisationProfile
        {
            Color = new VisualizationColorConfig
            {
                Gradient = new GradientDefinition()
                {
                    Angle = 270,
                    Points =
                    [
                        CreateGradientPoint("#E81123", 0),
                        CreateGradientPoint("#FFB900", 0.166),
                        CreateGradientPoint("#10893E", 0.332),
                        CreateGradientPoint("#10893E", 0.668),
                        CreateGradientPoint("#FFB900", 0.834),
                        CreateGradientPoint("#E81123", 1),
                    ],
                    Type = GradientType.Linear
                },
                Mode = VisualizationColorMode.Rainbow,
                SolidColor = new("#10893E", string.Empty)
            },
            Config = new WaveformVisualizationConfig(),
            IsDefault = true,
            Name = WAVEFORM_NAME,
            VisualizationType = VisualizationType.Waveform,
        };
    }

    private static VisualisationProfile CreateWaveformMirrored()
    {
        return new VisualisationProfile
        {
            Color = new VisualizationColorConfig
            {
                Gradient = new GradientDefinition()
                {
                    Angle = 270,
                    Points =
                    [
                        CreateGradientPoint("#E81123", 0),
                        CreateGradientPoint("#FFB900", 0.166),
                        CreateGradientPoint("#10893E", 0.332),
                        CreateGradientPoint("#10893E", 0.668),
                        CreateGradientPoint("#FFB900", 0.834),
                        CreateGradientPoint("#E81123", 1),
                    ],
                    Type = GradientType.Linear
                },
                Mode = VisualizationColorMode.Rainbow,
                SolidColor = new("#10893E", string.Empty)
            },
            Config = new WaveformMirroredVisualizationConfig(),
            IsDefault = true,
            Name = WAVEFORM_MIRRORED_NAME,
            VisualizationType = VisualizationType.WaveformMirrored,
        };
    }

    #endregion Profiles

    private static GradientPoint CreateGradientPoint(string color, double position)
    {
        return new GradientPoint()
        {
            Color = new(color, string.Empty),
            Position = position
        };
    }

    #endregion CREATE
}
