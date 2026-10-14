using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using MiniAudioEx.Native;
using VisualPlayer.Contexts;
using VisualPlayer.Models.Equalizer;
using VisualPlayer.ViewModels.Equalizer;

namespace VisualPlayer.Core;

public sealed class AudioEqualizerProcessor : IDisposable
{
    #region CONST

    private const double DEFAULT_Q = 1;
    private const int MAXIMUM_CHANNELS = 32;

    #endregion CONST

    private readonly EqualizerConfigurationContext _config;
    private readonly BiquadState[,] _states = new BiquadState[MAXIMUM_CHANNELS, 10];
    private readonly List<EqualizerBand> _subscribedBands = [];

    private FilterConfiguration _configuration = FilterConfiguration.Empty;
    private EqualizerProfile? _profile;

    #region PROPERTIES

    public uint SampleRate { get; }

    #endregion PROPERTIES

    #region CONSTRUCTOR

    public AudioEqualizerProcessor(EqualizerConfigurationContext config, uint sampleRate)
    {
        _config = config;
        SampleRate = Math.Max(1, sampleRate);
        _config.PropertyChanged += Config_PropertyChanged;
        BindProfile(_config.SelectedProfile);
    }

    public void Dispose()
    {
        _config.PropertyChanged -= Config_PropertyChanged;
        UnbindProfile();
    }

    #endregion CONSTRUCTOR

    #region CONFIGURATION

    private void BindProfile(EqualizerProfile profile)
    {
        UnbindProfile();
        _profile = profile;
        _profile.PropertyChanged += Profile_PropertyChanged;
        _profile.Bands.CollectionChanged += Bands_CollectionChanged;

        foreach (var band in _profile.Bands)
        {
            band.PropertyChanged += Band_PropertyChanged;
            _subscribedBands.Add(band);
        }

        RebuildConfiguration();
    }

    private BiquadCoefficients CreatePeakFilter(EqualizerBand band)
    {
        if (Math.Abs(band.Gain) < 0.001)
            return BiquadCoefficients.Disabled;

        var frequency = Math.Clamp(band.Frequency, 1, SampleRate * 0.49);
        var amplitude = Math.Pow(10, band.Gain / 40);
        var omega = 2 * Math.PI * frequency / SampleRate;
        var alpha = Math.Sin(omega) / (2 * DEFAULT_Q);
        var cosine = Math.Cos(omega);
        var a0 = 1 + alpha / amplitude;

        return new BiquadCoefficients(
            true,
            (1 + alpha * amplitude) / a0,
            (-2 * cosine) / a0,
            (1 - alpha * amplitude) / a0,
            (-2 * cosine) / a0,
            (1 - alpha / amplitude) / a0);
    }

    private void RebuildConfiguration()
    {
        if (_profile is null)
        {
            _configuration = FilterConfiguration.Empty;
            return;
        }

        var coefficients = new BiquadCoefficients[_profile.Bands.Count];
        for (var index = 0; index < coefficients.Length; index++)
            coefficients[index] = CreatePeakFilter(_profile.Bands[index]);

        var preampGain = Math.Pow(10, _profile.Preamp / 20);
        var isFlat = Math.Abs(_profile.Preamp) < 0.001;

        foreach (var coefficient in coefficients)
            isFlat &= !coefficient.Enabled;

        _configuration = new FilterConfiguration(coefficients, preampGain, isFlat);
    }

    private void UnbindProfile()
    {
        if (_profile is not null)
        {
            _profile.PropertyChanged -= Profile_PropertyChanged;
            _profile.Bands.CollectionChanged -= Bands_CollectionChanged;
        }

        foreach (var band in _subscribedBands)
            band.PropertyChanged -= Band_PropertyChanged;

        _subscribedBands.Clear();
        _profile = null;
    }

    #endregion CONFIGURATION

    #region PROPERTY CHANGES

    private void Bands_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (_profile is not null)
            BindProfile(_profile);
    }

    private void Band_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(EqualizerBand.Gain) or nameof(EqualizerBand.Frequency))
            RebuildConfiguration();
    }

    private void Config_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(EqualizerConfigurationContext.SelectedProfileName))
            BindProfile(_config.SelectedProfile);
    }

    private void Profile_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(EqualizerProfile.Preamp))
            RebuildConfiguration();
    }

    #endregion PROPERTY CHANGES

    #region PROCESSING

    public void Process(NativeArray<float> samples, uint frameCount, uint channels)
    {
        var configuration = _configuration;

        if (configuration.IsFlat || channels == 0 || samples.IsEmpty)
            return;

        var processedChannels = Math.Min((int)channels, MAXIMUM_CHANNELS);
        var availableFrames = Math.Min((int)frameCount, samples.Length / (int)channels);

        for (var frame = 0; frame < availableFrames; frame++)
        {
            var sampleOffset = frame * (int)channels;

            for (var channel = 0; channel < processedChannels; channel++)
            {
                var value = samples[sampleOffset + channel] * configuration.PreampGain;

                var bandCount = Math.Min(configuration.Coefficients.Length, 10);
                for (var band = 0; band < bandCount; band++)
                {
                    var coefficients = configuration.Coefficients[band];
                    if (!coefficients.Enabled)
                        continue;

                    ref var state = ref _states[channel, band];
                    var output = coefficients.B0 * value
                        + coefficients.B1 * state.Input1
                        + coefficients.B2 * state.Input2
                        - coefficients.A1 * state.Output1
                        - coefficients.A2 * state.Output2;

                    state.Input2 = state.Input1;
                    state.Input1 = value;
                    state.Output2 = state.Output1;
                    state.Output1 = output;
                    value = output;
                }

                samples[sampleOffset + channel] = (float)value;
            }
        }
    }

    #endregion PROCESSING

}
