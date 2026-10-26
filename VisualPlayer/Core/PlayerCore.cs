using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using MiniAudioEx.Core.AdvancedAPI;
using MiniAudioEx.Native;
using VisualPlayer.Contexts;
using VisualPlayer.Enums.Player;
using VisualPlayer.Models.Player;
using VisualPlayer.Events;
using VisualPlayer.ViewModels.Config;
using VisualPlayer.Utils;

namespace VisualPlayer.Core;

public class PlayerCore : IDisposable
{
    #region CONST

    public const double PLAYBACK_SPEED_MAXIMUM = 2;
    public const double PLAYBACK_SPEED_MINIMUM = 0.5;
    public const double PLAYBACK_SPEED_NORMAL = 1;
    private const uint AUDIO_PERIOD_SIZE_IN_FRAMES = 2048;

    #endregion CONST

    private readonly AudioEqualizerProcessor? _equalizer;
    private readonly MaEngine _engine;
    private readonly ma_device_data_proc _engineDataCallback;
    private readonly PlayerConfig _config;
    private readonly PlayListCore _playListCore;
    private readonly Stopwatch _crossfadeStopwatch = new();
    
    private PlaybackSound? _fadingSound;
    private bool _isDisposed;
    private bool _automaticCrossfadeAttempted;
    private PlaybackSound? _sound;
    private uint _outputChannels;

    #region EVENTS

    public event EventHandler? CurrentTrackChanged;
    public event EventHandler<PlaybackErrorEventArgs>? PlaybackFailed;

    #endregion EVENTS

    #region PROPERTIES

    public AudioAnalysisCore AudioAnalysis { get; }
    public PlayerDataContext DataContext { get; }
    

    public Track? CurrentTrack
    {
        get => _playListCore.CurrentTrack;
    }
    
    public double Duration
    {
        get => DataContext.Duration;
        private set => DataContext.Duration = double.IsFinite(value) ? Math.Max(0, value) : 0;
    }

    public bool IsMuted
    {
        get => DataContext.IsMuted;
        set => DataContext.IsMuted = value;
    }

    public bool IsPlaying
    {
        get => DataContext.IsPlaying;
        private set
        {
            DataContext.IsPlaying = value;
            _playListCore.SetIsPlaying(value);
        }
    }

    public bool IsShuffleEnabled
    {
        get => DataContext.IsShuffleEnabled;
        set => DataContext.IsShuffleEnabled = value;
    }

    public double Position
    {
        get => DataContext.Position;
        private set
        {
            if (double.IsFinite(value))
                DataContext.Position = Math.Clamp(value, 0, Duration);
        }
    }

    public double PlaybackSpeed
    {
        get => DataContext.PlaybackSpeed;
        set => DataContext.PlaybackSpeed = Math.Clamp(value, PLAYBACK_SPEED_MINIMUM, PLAYBACK_SPEED_MAXIMUM);
    }

    public RepeatMode RepeatMode
    {
        get => DataContext.RepeatMode;
        set => DataContext.RepeatMode = value;
    }

    public double Volume
    {
        get => DataContext.Volume;
        set => DataContext.Volume = value;
    }

    #endregion PROPERTIES

    #region CONSTRUCTORS

    public PlayerCore(
        PlayerConfig config,
        PlayListCore playListCore,
        EqualizerConfigurationContext equalizerConfig)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _playListCore = playListCore ?? throw new ArgumentNullException(nameof(playListCore));
        
        AudioAnalysis = new AudioAnalysisCore();
        _engine = new MaEngine();
        _engineDataCallback = EngineDataCallback;

        DataContext = new PlayerDataContext
        {
            IsMuted = config.IsMuted,
            IsShuffleEnabled = config.IsShuffleEnabled,
            PlaybackSpeed = PlaybackSpeedUtils.ToPlaybackSpeed(config.PlaybackSpeed),
            RepeatMode = config.RepeatMode,
            ShowRemainingTime = config.ShowRemainingTime,
            Volume = config.Volume
        };

        try
        {
            var engineConfig = _engine.GetConfig();
            engineConfig.periodSizeInFrames = AUDIO_PERIOD_SIZE_IN_FRAMES;
            engineConfig.SetDataProc(_engineDataCallback);
            var result = _engine.Initialize(engineConfig);

            if (result != ma_result.success)
                throw new InvalidOperationException($"Nie udało się zainicjalizować silnika audio: {result}.");

            ApplyVolume();
            _outputChannels = _engine.GetChannels();
            _equalizer = new AudioEqualizerProcessor(equalizerConfig, _engine.GetSampleRate());

            DataContext.PropertyChanged += DataContext_PropertyChanged;
            _config.PropertyChanged += Config_PropertyChanged;
            _playListCore.PlayRequested += PlayListCore_PlayRequested;
        }
        catch
        {
            _engine.Dispose();
            throw;
        }
    }

    public void Dispose()
    {
        if (_isDisposed)
            return;

        DataContext.PropertyChanged -= DataContext_PropertyChanged;
        _config.PropertyChanged -= Config_PropertyChanged;
        _playListCore.PlayRequested -= PlayListCore_PlayRequested;

        DisposeSound();

        _equalizer?.Dispose();
        _engine.Dispose();
        _isDisposed = true;
    }

    private void DisposeSound()
    {
        DisposeFadingSound();

        var sound = _sound;
        _sound = null;

        sound?.Stop();
        sound?.Dispose();
    }

    private void DisposeFadingSound()
    {
        _crossfadeStopwatch.Reset();

        var sound = _fadingSound;
        _fadingSound = null;

        sound?.Stop();
        sound?.Dispose();
    }

    #endregion CONSTRUCTORS

    #region AUDIO ANALYSIS

    private void EngineDataCallback(ma_device_ptr device, IntPtr output, IntPtr input, uint frameCount)
    {
        if (output == IntPtr.Zero || frameCount == 0)
            return;

        var channels = _outputChannels;
        if (channels == 0)
            return;

        var samples = new NativeArray<float>(output, checked((int)(frameCount * channels)));
        var result = MiniAudioNative.ma_engine_read_pcm_frames(_engine.Handle, output, frameCount);

        if (result != ma_result.success)
        {
            ClearSamples(samples);
            return;
        }

        _equalizer?.Process(samples, frameCount, channels);
        AudioAnalysis.WriteInterleavedSamples(samples, frameCount, channels);
    }

    private static void ClearSamples(NativeArray<float> samples)
    {
        for (var index = 0; index < samples.Length; index++)
            samples[index] = 0;
    }

    #endregion AUDIO ANALYSIS

    #region PLAYER METHODS

    public void Play(Track track)
    {
        ArgumentNullException.ThrowIfNull(track);

        if (!_playListCore.PlayList.Tracks.Contains(track))
            return;

        if (IsShuffleEnabled)
            _playListCore.ResetShuffle(track);

        // An explicit selection must report its failure instead of silently
        // starting another track from the playlist.
        TryPlayTracks([track]);
    }

    public void TogglePlayPause()
    {
        CompleteCrossfade();

        if (_sound is null)
        {
            TryPlayTracks(_playListCore.GetInitialTracks(
                IsShuffleEnabled,
                RepeatMode == RepeatMode.All));
            return;
        }

        var result = IsPlaying ? _sound.Stop() : _sound.Start();

        if (result != ma_result.success)
        {
            ReportPlaybackError(CurrentTrack, $"Play/Pause: {result} ({(int)result})");
            return;
        }

        IsPlaying = !IsPlaying;
    }

    public void Stop()
    {
        CompleteCrossfade();

        if (_playListCore.PlayList.Tracks.Count == 0)
        {
            UnloadTrack();
            return;
        }

        if (_sound is null)
            return;

        var result = _sound.Stop();
        
        if (result != ma_result.success)
        {
            ReportPlaybackError(CurrentTrack, $"Stop: {result} ({(int)result})");
            return;
        }

        IsPlaying = false;
        result = MiniAudioNative.ma_sound_seek_to_second(_sound.Handle, 0);

        if (result != ma_result.success)
        {
            ReportPlaybackError(CurrentTrack, $"Seek: {result} ({(int)result})");
            return;
        }

        Position = 0;
    }

    public void UnloadTrack()
    {
        DisposeSound();

        var hadTrack = CurrentTrack is not null;
        _playListCore.SetCurrentTrack(null, false);
        IsPlaying = false;
        Position = 0;
        Duration = 0;
        _automaticCrossfadeAttempted = false;
        _playListCore.ResetShuffle();

        if (hadTrack)
            CurrentTrackChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Next()
    {
        if (_playListCore.PlayList.Tracks.Count == 0)
            return;

        if (_sound is null)
        {
            TryPlayTracks(_playListCore.GetInitialTracks(
                IsShuffleEnabled,
                RepeatMode == RepeatMode.All));
            return;
        }

        TryPlayTracks(_playListCore.GetNextTracks(
            IsShuffleEnabled,
            RepeatMode == RepeatMode.All));
    }

    public void Previous()
    {
        if (_playListCore.PlayList.Tracks.Count == 0)
            return;

        if (_sound is null)
        {
            TryPlayTracks(_playListCore.GetInitialTracks(
                IsShuffleEnabled,
                RepeatMode == RepeatMode.All));
            return;
        }

        if (!TryPlayTracks(_playListCore.GetPreviousTracks(
                IsShuffleEnabled,
                RepeatMode == RepeatMode.All)))
            Seek(0);
    }

    public void RefreshPosition()
    {
        if (_sound is null)
            return;

        UpdateCrossfade();

        if (MiniAudioNative.ma_sound_get_cursor_in_seconds(_sound.Handle, out var seconds) == ma_result.success)
            Position = seconds;

        if (ShouldStartAutomaticCrossfade())
        {
            _automaticCrossfadeAttempted = true;
            TryPlayTracks(_playListCore.GetNextTracks(
                IsShuffleEnabled,
                RepeatMode == RepeatMode.All), reportErrors: false);
            return;
        }

        if (IsPlaying && MiniAudioNative.ma_sound_at_end(_sound.Handle) != 0)
        {
            if (_playListCore.PlayList.Tracks.Count == 0)
            {
                UnloadTrack();
                return;
            }

            if (RepeatMode == RepeatMode.One && CurrentTrack is { } currentTrack
                && _playListCore.PlayList.Tracks.Contains(currentTrack) && RestartCurrentTrack())
                return;

            var started = TryPlayTracks(_playListCore.GetNextTracks(
                IsShuffleEnabled,
                RepeatMode == RepeatMode.All));

            if (!started)
                UnloadTrack();

            return;
        }

        IsPlaying = _sound.IsPlaying();
    }

    public void Seek(double position)
    {
        if (_sound is null || !double.IsFinite(position))
            return;

        var target = Math.Clamp(position, 0, Duration);
        var result = MiniAudioNative.ma_sound_seek_to_second(_sound.Handle, (float)target);
        
        if (result != ma_result.success)
        {
            ReportPlaybackError(CurrentTrack, $"Seek: {result} ({(int)result})");
            return;
        }

        Position = target;
        _automaticCrossfadeAttempted = false;
    }

    #endregion PLAYER METHODS

    #region PLAYLIST NAVIGATION

    private bool RestartCurrentTrack()
    {
        if (_sound is null)
            return false;

        var result = MiniAudioNative.ma_sound_seek_to_second(_sound.Handle, 0);
        if (result != ma_result.success)
            return false;

        result = _sound.Start();
        if (result != ma_result.success)
            return false;

        Position = 0;
        IsPlaying = true;
        _automaticCrossfadeAttempted = false;
        return true;
    }

    private void ReportPlaybackError(Track? track, string details, int failedTrackCount = 1)
    {
        Debug.WriteLine($"Playback failed for '{track?.FilePath}': {details}");
        PlaybackFailed?.Invoke(this, new PlaybackErrorEventArgs(track, details, failedTrackCount));
    }

    private bool TryPlayTracks(IEnumerable<Track> tracks, bool reportErrors = true)
    {
        Track? firstFailedTrack = null;
        var firstError = string.Empty;
        var failedTrackCount = 0;
        var started = false;
        foreach (var track in tracks)
        {
            if (TryStartTrack(track, out var error))
            {
                started = true;
                break;
            }

            if (firstFailedTrack is null)
            {
                firstFailedTrack = track;
                firstError = error;
            }
            failedTrackCount++;
        }

        if (reportErrors && firstFailedTrack is not null)
            ReportPlaybackError(firstFailedTrack, firstError, failedTrackCount);

        return started;
    }

    private bool TryStartTrack(Track track, out string error)
    {
        error = string.Empty;
        var newSound = new PlaybackSound();
        double duration;

        try
        {
            var isMp3 = string.Equals(Path.GetExtension(track.FilePath), ".mp3", StringComparison.OrdinalIgnoreCase);
            var flags = ma_sound_flags.stream;
            // The MP3 decoder may scan the entire file to calculate its length
            // during initialization. TagLib has already supplied the duration.
            if (isMp3)
                flags |= ma_sound_flags.unknown_length;

            var result = newSound.InitializeFromFile(
                _engine,
                track.FilePath,
                flags);
            
            if (result != ma_result.success)
            {
                error = $"Open: {result} ({(int)result})";
                newSound.Dispose();
                return false;
            }

            duration = track.Duration.TotalSeconds;
            
            if (!isMp3 && newSound.GetLengthInSeconds(out var length) == ma_result.success
                && float.IsFinite(length) && length > 0)
            {
                duration = length;
            }

            newSound.SetPitch((float)PlaybackSpeed);
        }
        catch (Exception exception)
        {
            newSound.Dispose();
            error = exception.Message;
            Debug.WriteLine($"Nie udało się wczytać '{track.FilePath}': {exception}");
            return false;
        }

        CompleteCrossfade();

        var previousSound = _sound;
        var previousWasPlaying = previousSound?.IsPlaying() == true;
        var useCrossfade = _config.CrossfadeEnabled && previousWasPlaying;

        if (previousSound is not null && !useCrossfade)
        {
            var stopResult = previousSound.Stop();

            if (stopResult != ma_result.success)
            {
                error = $"Stop previous track: {stopResult} ({(int)stopResult})";
                newSound.Dispose();
                return false;
            }
        }

        try
        {
            if (useCrossfade)
                newSound.SetVolume(0);

            var result = newSound.Start();

            if (result != ma_result.success)
            {
                error = $"Start: {result} ({(int)result})";
                
                if (previousWasPlaying)
                    previousSound?.Start();
                
                newSound.Dispose();
                return false;
            }
        }
        catch (Exception exception)
        {
            if (previousWasPlaying)
                previousSound?.Start();
            
            newSound.Dispose();
            error = exception.Message;
            Debug.WriteLine($"Nie udało się rozpocząć odtwarzania '{track.FilePath}': {exception}");
            return false;
        }

        _sound = newSound;

        if (useCrossfade && previousSound is not null)
        {
            _fadingSound = previousSound;
            _crossfadeStopwatch.Restart();
        }
        else
        {
            previousSound?.Dispose();
            newSound.SetVolume(1);
        }

        var trackChanged = !ReferenceEquals(CurrentTrack, track);
        _playListCore.SetCurrentTrack(track, true);
        Duration = duration;
        Position = 0;
        IsPlaying = true;
        _automaticCrossfadeAttempted = false;

        if (trackChanged)
            CurrentTrackChanged?.Invoke(this, EventArgs.Empty);

        return true;
    }

    #endregion PLAYLIST NAVIGATION

    #region CROSSFADE

    private void CompleteCrossfade()
    {
        if (_fadingSound is null)
            return;

        _sound?.SetVolume(1);
        DisposeFadingSound();
    }

    private bool ShouldStartAutomaticCrossfade()
    {
        if (!_config.CrossfadeEnabled
            || _automaticCrossfadeAttempted
            || _fadingSound is not null
            || !IsPlaying
            || RepeatMode == RepeatMode.One
            || Duration <= 0)
        {
            return false;
        }

        var remaining = Duration - Position;
        return remaining > 0 && remaining <= _config.CrossfadeDuration;
    }

    private void UpdateCrossfade()
    {
        if (_fadingSound is null)
            return;

        if (!_config.CrossfadeEnabled)
        {
            CompleteCrossfade();
            return;
        }

        var duration = Math.Max(
            PlayerConfig.CROSSFADE_DURATION_MINIMUM,
            _config.CrossfadeDuration);
        var progress = Math.Clamp(_crossfadeStopwatch.Elapsed.TotalSeconds / duration, 0, 1);

        _fadingSound.SetVolume((float)(1 - progress));
        _sound?.SetVolume((float)progress);

        if (progress >= 1 || !_fadingSound.IsPlaying())
            CompleteCrossfade();
    }

    #endregion CROSSFADE

    #region PROPERTY CHANGES

    private void DataContext_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(PlayerDataContext.IsMuted):
                ApplyVolume();
                _config.IsMuted = IsMuted;
                break;
            case nameof(PlayerDataContext.PlaybackSpeed):
                _sound?.SetPitch((float)PlaybackSpeed);
                _config.PlaybackSpeed = PlaybackSpeedUtils.ToPercent(PlaybackSpeed);
                break;
            case nameof(PlayerDataContext.IsShuffleEnabled):
                if (IsShuffleEnabled)
                    _playListCore.ResetShuffle(CurrentTrack);
                _config.IsShuffleEnabled = IsShuffleEnabled;
                break;
            case nameof(PlayerDataContext.RepeatMode):
                _config.RepeatMode = RepeatMode;
                break;
            case nameof(PlayerDataContext.Volume):
                ApplyVolume();
                _config.Volume = Volume;
                break;
        }
    }

    private void Config_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(PlayerConfig.PlaybackSpeed):
                PlaybackSpeed = PlaybackSpeedUtils.ToPlaybackSpeed(_config.PlaybackSpeed);
                break;

            case nameof(PlayerConfig.ShowRemainingTime):
                DataContext.ShowRemainingTime = _config.ShowRemainingTime;
                break;
        }
    }

    private void PlayListCore_PlayRequested(object? sender, TrackEventArgs e)
    {
        Play(e.Track);
    }

    #endregion PROPERTY CHANGES

    #region VOLUME

    private void ApplyVolume()
    {
        var effectiveVolume = IsMuted ? 0f : (float)Volume;
        var result = _engine.SetVolume(effectiveVolume);

        if (result != ma_result.success)
            throw new InvalidOperationException($"Nie udało się ustawić głośności: {result}.");
    }

    #endregion VOLUME
}
