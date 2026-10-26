using System;
using MiniAudioEx.Core.AdvancedAPI;
using MiniAudioEx.Native;

namespace VisualPlayer.Core;

// Own the native sound so that Windows paths can use the Unicode entry point.
// MiniAudioExNET 3.3.5's MaSound.InitializeFromFile uses ANSI marshaling.
internal sealed class PlaybackSound : IDisposable
{
    private ma_sound_ptr _handle = new(true);
    private bool _initialized;
    private bool _disposed;

    public ma_sound_ptr Handle => _handle;

    public PlaybackSound()
    {
        if (_handle.pointer == IntPtr.Zero)
            throw new OutOfMemoryException("Could not allocate an audio sound.");
    }

    public ma_result InitializeFromFile(MaEngine engine, string filePath, ma_sound_flags flags)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (_initialized)
            throw new InvalidOperationException("The sound is already initialized.");

        var result = OperatingSystem.IsWindows()
            ? MiniAudioNative.ma_sound_init_from_file_w(engine.Handle, filePath, flags, default, default, _handle)
            : MiniAudioNative.ma_sound_init_from_file(engine.Handle, filePath, flags, default, default, _handle);

        _initialized = result == ma_result.success;
        return result;
    }

    public ma_result Start() => MiniAudioNative.ma_sound_start(_handle);

    public ma_result Stop() => MiniAudioNative.ma_sound_stop(_handle);

    public bool IsPlaying() => MiniAudioNative.ma_sound_is_playing(_handle) != 0;

    public void SetPitch(float pitch) => MiniAudioNative.ma_sound_set_pitch(_handle, pitch);

    public void SetVolume(float volume) => MiniAudioNative.ma_sound_set_volume(_handle, volume);

    public ma_result GetLengthInSeconds(out float length) =>
        MiniAudioNative.ma_sound_get_length_in_seconds(_handle, out length);

    public void Dispose()
    {
        if (_disposed)
            return;

        if (_initialized)
        {
            MiniAudioNative.ma_sound_uninit(_handle);
            _initialized = false;
        }

        _handle.Free();
        _disposed = true;
    }
}
