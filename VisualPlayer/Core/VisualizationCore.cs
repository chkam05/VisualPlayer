using System;
using System.ComponentModel;
using System.Diagnostics;
using Avalonia;
using Avalonia.Media;
using VisualPlayer.Contexts;
using VisualPlayer.Enums;
using VisualPlayer.Interfaces;
using VisualPlayer.Visualizations;
using VisualPlayer.Visualizations.Config;
using VisualPlayer.Visualizations.Config.Base;
using VisualPlayer.Visualizations.Data;

namespace VisualPlayer.Core;

public class VisualizationCore : IDisposable
{
    private readonly VisualizationConfigurationContext _config;
    private readonly Func<bool> _isPlayingProvider;
    private readonly IAudioSpectrumProvider _spectrumProvider;

    private VisualisationProfile? _activeProfile;
    private Rect _drawingArea;
    private bool _isDisposed;
    private long _lastFrameTimestamp;
    private float[] _rawSpectrum = [];
    private double _renderScaling = 1;
    private float[] _spectrum = [];
    private float[] _waveform = [];
    private VisualizationGlobalConfig _subscribedGlobalConfig;

    #region PROPERTIES

    public IVisualization? CurrentVisualization { get; private set; }

    public bool FillScreen
    {
        get => _config.Global.FillScreen;
    }

    public TimeSpan FrameInterval
    {
        get
        {
            var configuredFrameRate = Math.Max(15, _config.Global.FrameRateLimit);
            var maximumFrameRate = CurrentVisualization?.MaximumFrameRate ?? int.MaxValue;
            return TimeSpan.FromSeconds(1d / Math.Min(configuredFrameRate, maximumFrameRate));
        }
    }
    
    #endregion PROPERTIES

    #region CONSTRUCTOR

    public VisualizationCore(
        VisualizationConfigurationContext config,
        IAudioSpectrumProvider spectrumProvider,
        Func<bool> isPlayingProvider)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _spectrumProvider = spectrumProvider ?? throw new ArgumentNullException(nameof(spectrumProvider));
        _isPlayingProvider = isPlayingProvider ?? throw new ArgumentNullException(nameof(isPlayingProvider));
        _subscribedGlobalConfig = _config.Global;
        ConfigureAnalysis();

        _config.PropertyChanged += Config_PropertyChanged;
        _subscribedGlobalConfig.PropertyChanged += GlobalConfig_PropertyChanged;
        ActivateProfile();
    }

    public void Dispose()
    {
        if (_isDisposed)
            return;

        _config.PropertyChanged -= Config_PropertyChanged;
        _subscribedGlobalConfig.PropertyChanged -= GlobalConfig_PropertyChanged;

        if (_activeProfile is not null)
            _activeProfile.PropertyChanged -= ActiveProfile_PropertyChanged;

        CurrentVisualization?.Dispose();
        CurrentVisualization = null;
        _isDisposed = true;
    }

    #endregion CONSTRUCTOR

    #region PROPERTY CHANGES

    private void ActiveProfile_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(VisualisationProfile.VisualizationType)
            || e.PropertyName == nameof(VisualisationProfile.Config))
        {
            SetVisualization(_activeProfile?.VisualizationType ?? VisualizationType.None);
        }
    }

    private void Config_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(VisualizationConfigurationContext.SelectedProfileName)
            || e.PropertyName == nameof(VisualizationConfigurationContext.Profiles))
        {
            ActivateProfile();
        }

        if (e.PropertyName == nameof(VisualizationConfigurationContext.Global))
        {
            _subscribedGlobalConfig.PropertyChanged -= GlobalConfig_PropertyChanged;
            _subscribedGlobalConfig = _config.Global;
            _subscribedGlobalConfig.PropertyChanged += GlobalConfig_PropertyChanged;
            ConfigureAnalysis();
        }
    }

    private void GlobalConfig_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        ConfigureAnalysis();
    }

    #endregion PROPERTY CHANGES

    public void Draw(DrawingContext drawingContext, IBrush brush)
    {
        if (CurrentVisualization is null)
            return;

        var timestamp = Stopwatch.GetTimestamp();
        var elapsed = _lastFrameTimestamp == 0
            ? TimeSpan.FromSeconds(1d / 60)
            : Stopwatch.GetElapsedTime(_lastFrameTimestamp, timestamp);

        _lastFrameTimestamp = timestamp;

        var isPlaying = _isPlayingProvider();

        if (isPlaying)
        {
            _spectrumProvider.TryGetSpectrum(_rawSpectrum);
            _spectrumProvider.TryGetWaveform(_waveform);
        }
        else
        {
            Array.Clear(_rawSpectrum);
            Array.Clear(_waveform);
        }

        ProcessSpectrum(elapsed);

        var frame = new VisualizationFrame(elapsed, _spectrum, _waveform, isPlaying);
        CurrentVisualization.PreCalculate(frame);
        CurrentVisualization.Draw(drawingContext, brush);
    }

    public void Reset()
    {
        Array.Clear(_rawSpectrum);
        Array.Clear(_spectrum);
        Array.Clear(_waveform);
        _lastFrameTimestamp = 0;
        CurrentVisualization?.Reset();
    }

    public double GetFrequencyLevel(int minimumFrequency, int maximumFrequency)
    {
        if (_spectrum.Length == 0)
            return 0;

        const double nyquist = 24000d;
        var minimum = Math.Clamp(Math.Min(minimumFrequency, maximumFrequency), 0, nyquist);
        var maximum = Math.Clamp(Math.Max(minimumFrequency, maximumFrequency), minimum, nyquist);
        var globalMinimum = Math.Clamp(_config.Global.MinimumFrequency, 0, nyquist - 1);
        var globalMaximum = Math.Clamp(_config.Global.MaximumFrequency, globalMinimum + 1, nyquist);
        double maximumLevel = 0;

        for (var index = 0; index < _spectrum.Length; index++)
        {
            var position = _spectrum.Length <= 1
                ? 0
                : (double)index / (_spectrum.Length - 1);
            var frequency = MapFrequency(
                position,
                globalMinimum,
                globalMaximum,
                _config.Global.ScalingStrategy);

            if (frequency >= minimum && frequency <= maximum)
                maximumLevel = Math.Max(maximumLevel, _spectrum[index]);
        }

        return maximumLevel;
    }

    public void SetVisualization(VisualizationType visualizationType)
    {
        CurrentVisualization?.Dispose();
        CurrentVisualization = visualizationType switch
        {
            VisualizationType.Peaks when _activeProfile is not null
                => new PeaksVisualization(_activeProfile.GetConfig<PeaksVisualizationConfig>(), _activeProfile.GetColorConfig()),
            VisualizationType.Stripes when _activeProfile is not null
                => new StripesVisualization(_activeProfile.GetConfig<StripesVisualizationConfig>(), _activeProfile.GetColorConfig()),
            VisualizationType.PeaksBouncing when _activeProfile is not null
                => new PeaksBouncingVisualization(_activeProfile.GetConfig<PeaksBouncingVisualizationConfig>(), _activeProfile.GetColorConfig()),
            VisualizationType.StripesBouncing when _activeProfile is not null
                => new StripesBouncingVisualization(_activeProfile.GetConfig<StripesBouncingVisualizationConfig>(), _activeProfile.GetColorConfig()),
            VisualizationType.PeaksMirrored when _activeProfile is not null
                => new PeaksMirroredVisualization(_activeProfile.GetConfig<PeaksMirroredVisualizationConfig>(), _activeProfile.GetColorConfig()),
            VisualizationType.StripesMirrored when _activeProfile is not null
                => new StripesMirroredVisualization(_activeProfile.GetConfig<StripesMirroredVisualizationConfig>(), _activeProfile.GetColorConfig()),
            VisualizationType.PeaksFloating when _activeProfile is not null
                => new PeaksFloatingVisualization(_activeProfile.GetConfig<PeaksFloatingVisualizationConfig>(), _activeProfile.GetColorConfig()),
            VisualizationType.PeaksFloatingMirrored when _activeProfile is not null
                => new PeaksFloatingMirroredVisualization(_activeProfile.GetConfig<PeaksFloatingMirroredVisualizationConfig>(), _activeProfile.GetColorConfig()),
            VisualizationType.Oscilloscope when _activeProfile is not null
                => new OscilloscopeVisualization(_activeProfile.GetConfig<OscilloscopeVisualizationConfig>(), _activeProfile.GetColorConfig()),
            VisualizationType.OscilloscopeDouble when _activeProfile is not null
                => new OscilloscopeDoubleVisualization(_activeProfile.GetConfig<OscilloscopeDoubleVisualizationConfig>(), _activeProfile.GetColorConfig()),
            VisualizationType.OscilloscopeFilled when _activeProfile is not null
                => new OscilloscopeFilledVisualization(_activeProfile.GetConfig<OscilloscopeFilledVisualizationConfig>(), _activeProfile.GetColorConfig()),
            VisualizationType.OscilloscopeCenter when _activeProfile is not null
                => new OscilloscopeCenterVisualization(_activeProfile.GetConfig<OscilloscopeCenterVisualizationConfig>(), _activeProfile.GetColorConfig()),
            VisualizationType.Waveform when _activeProfile is not null
                => new WaveformVisualization(_activeProfile.GetConfig<WaveformVisualizationConfig>(), _activeProfile.GetColorConfig()),
            VisualizationType.WaveformMirrored when _activeProfile is not null
                => new WaveformMirroredVisualization(_activeProfile.GetConfig<WaveformMirroredVisualizationConfig>(), _activeProfile.GetColorConfig()),
            VisualizationType.CircleStripes when _activeProfile is not null
                => new CircleStripesVisualization(_activeProfile.GetConfig<CircleStripesVisualizationConfig>(), _activeProfile.GetColorConfig()),
            VisualizationType.CircleWaveform when _activeProfile is not null
                => new CircleWaveformVisualization(_activeProfile.GetConfig<CircleWaveformVisualizationConfig>(), _activeProfile.GetColorConfig()),
            _ => null
        };

        CurrentVisualization?.UpdateGraphics(_drawingArea, _renderScaling);
        _lastFrameTimestamp = 0;
    }

    public void UpdateGraphics(Rect drawingArea, double renderScaling)
    {
        _drawingArea = drawingArea;
        _renderScaling = renderScaling;
        CurrentVisualization?.UpdateGraphics(drawingArea, renderScaling);
    }


    private void ActivateProfile()
    {
        if (_activeProfile is not null)
            _activeProfile.PropertyChanged -= ActiveProfile_PropertyChanged;

        _config.EnsureProfiles();
        _activeProfile = _config.SelectedProfile;
        _activeProfile.PropertyChanged += ActiveProfile_PropertyChanged;
        SetVisualization(_activeProfile.VisualizationType);
    }

    private void ConfigureAnalysis()
    {
        _spectrumProvider.Configure(_config.Global);
        var spectrumSize = _spectrumProvider.SpectrumSize;

        if (_rawSpectrum.Length != spectrumSize)
        {
            _rawSpectrum = new float[spectrumSize];
            _spectrum = new float[spectrumSize];
        }


        if (_waveform.Length != _spectrumProvider.WaveformSize)
            _waveform = new float[_spectrumProvider.WaveformSize];
    }

    private static double MapFrequency(double position, double minimum, double maximum, ScalingStrategy strategy)
    {
        if (strategy == ScalingStrategy.Linear)
            return minimum + (maximum - minimum) * position;

        if (strategy == ScalingStrategy.Mel)
        {
            var minimumMel = 2595 * Math.Log10(1 + minimum / 700);
            var maximumMel = 2595 * Math.Log10(1 + maximum / 700);
            return 700 * (Math.Pow(10, (minimumMel + (maximumMel - minimumMel) * position) / 2595) - 1);
        }

        if (strategy == ScalingStrategy.Bark)
        {
            var minimumBark = 6 * Math.Asinh(minimum / 600);
            var maximumBark = 6 * Math.Asinh(maximum / 600);
            return 600 * Math.Sinh((minimumBark + (maximumBark - minimumBark) * position) / 6);
        }

        var safeMinimum = Math.Max(1, minimum);
        return safeMinimum * Math.Pow(maximum / safeMinimum, position);
    }

    private void ProcessSpectrum(TimeSpan elapsed)
    {
        var config = _config.Global;
        var nyquist = 24000d;
        var minimumFrequency = Math.Clamp(config.MinimumFrequency, 0, nyquist - 1);
        var maximumFrequency = Math.Clamp(config.MaximumFrequency, minimumFrequency + 1, nyquist);
        var minimumDecibels = Math.Min(config.MinimumDecibels, config.MaximumDecibels - 1);
        var maximumDecibels = Math.Max(config.MaximumDecibels, minimumDecibels + 1);

        for (var index = 0; index < _spectrum.Length; index++)
        {
            var position = _spectrum.Length <= 1 ? 0 : (double)index / (_spectrum.Length - 1);
            var frequency = MapFrequency(position, minimumFrequency, maximumFrequency, config.ScalingStrategy);
            var sourcePosition = Math.Clamp(
                frequency / nyquist * (_rawSpectrum.Length - 1),
                0,
                _rawSpectrum.Length - 1);
            var lowerSourceIndex = (int)Math.Floor(sourcePosition);
            var upperSourceIndex = Math.Min(lowerSourceIndex + 1, _rawSpectrum.Length - 1);
            var sourceAmount = sourcePosition - lowerSourceIndex;
            var magnitude = Math.Max(
                _rawSpectrum[lowerSourceIndex]
                + (_rawSpectrum[upperSourceIndex] - _rawSpectrum[lowerSourceIndex]) * (float)sourceAmount,
                0.000001f);
            var decibels = 20 * Math.Log10(magnitude);
            var target = Math.Clamp((decibels - minimumDecibels) / (maximumDecibels - minimumDecibels), 0, 1);
            var time = target > _spectrum[index] ? config.AttackTime : config.ReleaseTime;
            var factor = time <= 0 ? 1 : 1 - Math.Exp(-elapsed.TotalMilliseconds / time);
            _spectrum[index] = (float)(_spectrum[index] + (target - _spectrum[index]) * factor);
        }
    }
}
