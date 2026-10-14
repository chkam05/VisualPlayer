using System;
using System.Numerics;
using System.Threading;
using MiniAudioEx.Native;
using VisualPlayer.Interfaces;
using VisualPlayer.Enums;
using VisualPlayer.Visualizations.Config;
using VisualPlayer.Visualizations.Config.Base;

namespace VisualPlayer.Core;

public class AudioAnalysisCore : IAudioSpectrumProvider
{
    #region CONST

    private const int BUFFER_SIZE = 16384;
    private const int WAVEFORM_SIZE = 1024;

    #endregion CONST

    private readonly float[] _sampleBuffer = new float[BUFFER_SIZE];
    private readonly object _sync = new();

    private VisualizationGlobalConfig _config = new();
    private FftWindowType _configuredWindow;
    private Complex[] _fftBuffer = [];
    private long _lastAnalyzedPosition;
    private float[] _window = [];
    private long _writePosition;
    
    #region PROPERTIES

    public int SpectrumSize
    {
        get => (int)_config.FftSize / 2;
    }

    public int WaveformSize
    {
        get => WAVEFORM_SIZE;
    }

    #endregion PROPERTIES

    #region CONSTRUCTOR

    public AudioAnalysisCore()
    {
        RebuildBuffers();
    }

    #endregion CONSTRUCTOR

    public void Configure(VisualizationGlobalConfig config)
    {
        lock (_sync)
        {
            _config = config;
            if (_fftBuffer.Length != (int)config.FftSize || _configuredWindow != config.FftWindow)
                RebuildBuffers();
        }
    }

    public bool TryGetSpectrum(Span<float> destination)
    {
        lock (_sync)
        {
            var fftSize = (int)_config.FftSize;
            if (destination.Length < SpectrumSize)
                throw new ArgumentException($"Bufor widma musi mieć co najmniej {SpectrumSize} elementów.", nameof(destination));

            var writePosition = Volatile.Read(ref _writePosition);

            if (writePosition == _lastAnalyzedPosition || writePosition < fftSize)
                return false;

            var startPosition = writePosition - fftSize;

            for (var i = 0; i < fftSize; i++)
            {
                var sample = _sampleBuffer[(int)((startPosition + i) & (BUFFER_SIZE - 1))];
                _fftBuffer[i] = new Complex(sample * _window[i], 0);
            }

            Transform(_fftBuffer);

            var normalization = 2f / fftSize;

            for (var i = 0; i < SpectrumSize; i++)
            {
                var magnitude = (float)_fftBuffer[i].Magnitude * normalization;
                destination[i] = magnitude;
            }

            _lastAnalyzedPosition = writePosition;
            return true;
        }
    }

    public bool TryGetWaveform(Span<float> destination)
    {
        if (destination.Length < WAVEFORM_SIZE)
            throw new ArgumentException($"Bufor przebiegu musi mieć co najmniej {WAVEFORM_SIZE} elementów.", nameof(destination));

        var writePosition = Volatile.Read(ref _writePosition);
        if (writePosition == 0)
            return false;

        var available = (int)Math.Min(writePosition, WAVEFORM_SIZE);
        var padding = WAVEFORM_SIZE - available;
        destination[..padding].Clear();
        var startPosition = writePosition - available;

        for (var index = 0; index < available; index++)
            destination[padding + index] = _sampleBuffer[(int)((startPosition + index) & (BUFFER_SIZE - 1))];

        return true;
    }

    public void Reset()
    {
        Array.Clear(_sampleBuffer);
        _lastAnalyzedPosition = 0;
        Volatile.Write(ref _writePosition, 0);
    }

    public void WriteInterleavedSamples(NativeArray<float> samples, uint frameCount, uint channels)
    {
        if (channels == 0 || frameCount == 0 || samples.IsEmpty)
            return;

        var writePosition = Volatile.Read(ref _writePosition);
        var availableFrames = Math.Min((int)frameCount, samples.Length / (int)channels);

        for (var frame = 0; frame < availableFrames; frame++)
        {
            var sampleOffset = frame * (int)channels;
            var left = samples[sampleOffset];
            var right = channels > 1 ? samples[sampleOffset + 1] : left;
            var monoSample = _config.ChannelMode switch
            {
                ChannelMode.Left => left,
                ChannelMode.Right => right,
                ChannelMode.Mid => (left + right) * 0.5f,
                ChannelMode.Side => (left - right) * 0.5f,
                ChannelMode.Stereo => (left + right) * 0.5f,
                _ => GetMixedSample(samples, sampleOffset, channels)
            };
            _sampleBuffer[(int)(writePosition & (BUFFER_SIZE - 1))] = monoSample;
            writePosition++;
        }

        Volatile.Write(ref _writePosition, writePosition);
    }

    #region UTILITIES

    private static float GetMixedSample(NativeArray<float> samples, int offset, uint channels)
    {
        var value = 0f;
        for (var channel = 0; channel < (int)channels; channel++)
            value += samples[offset + channel];
        return value / channels;
    }

    private void RebuildBuffers()
    {
        var fftSize = (int)_config.FftSize;
        _configuredWindow = _config.FftWindow;
        _fftBuffer = new Complex[fftSize];
        _window = new float[fftSize];

        for (var index = 0; index < fftSize; index++)
        {
            var phase = 2 * MathF.PI * index / Math.Max(1, fftSize - 1);
            _window[index] = _config.FftWindow switch
            {
                FftWindowType.Hamming => 0.54f - 0.46f * MathF.Cos(phase),
                FftWindowType.Blackman => 0.42f - 0.5f * MathF.Cos(phase) + 0.08f * MathF.Cos(2 * phase),
                FftWindowType.BlackmanHarris => 0.35875f - 0.48829f * MathF.Cos(phase) + 0.14128f * MathF.Cos(2 * phase) - 0.01168f * MathF.Cos(3 * phase),
                FftWindowType.Rectangular => 1,
                _ => 0.5f * (1 - MathF.Cos(phase))
            };
        }

        _lastAnalyzedPosition = 0;
    }

    private static void Transform(Complex[] buffer)
    {
        var count = buffer.Length;

        var reversed = 0;

        for (var i = 1; i < count; i++)
        {
            var bit = count >> 1;

            while ((reversed & bit) != 0)
            {
                reversed ^= bit;
                bit >>= 1;
            }

            reversed ^= bit;

            if (i < reversed)
                (buffer[i], buffer[reversed]) = (buffer[reversed], buffer[i]);
        }

        for (var length = 2; length <= count; length <<= 1)
        {
            var angle = -2 * Math.PI / length;
            var step = new Complex(Math.Cos(angle), Math.Sin(angle));

            for (var start = 0; start < count; start += length)
            {
                var rotation = Complex.One;
                var halfLength = length >> 1;

                for (var offset = 0; offset < halfLength; offset++)
                {
                    var even = buffer[start + offset];
                    var odd = buffer[start + offset + halfLength] * rotation;
                    buffer[start + offset] = even + odd;
                    buffer[start + offset + halfLength] = even - odd;
                    rotation *= step;
                }
            }
        }
    }

    #endregion UTILITIES
}
