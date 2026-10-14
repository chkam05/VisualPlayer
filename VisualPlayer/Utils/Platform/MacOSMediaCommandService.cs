using System;
using System.Runtime.InteropServices;
using Avalonia.Threading;

namespace VisualPlayer.Utils.Platform;

public class MacOSMediaCommandService : IDisposable
{
    private const string LIBOBJC = "/usr/lib/libobjc.A.dylib";
    private const string MEDIA_PLAYER_FRAMEWORK = "/System/Library/Frameworks/MediaPlayer.framework/MediaPlayer";
    private const string TARGET_CLASS_NAME = "VisualPlayerMediaCommandTarget";

    private static readonly CommandCallback PlayPauseCallback = HandlePlayPause;
    private static readonly CommandCallback NextCallback = HandleNext;
    private static readonly CommandCallback PreviousCallback = HandlePrevious;

    private static Action? _playPauseAction;
    private static Action? _nextAction;
    private static Action? _previousAction;

    private IntPtr _commandCenter;
    private readonly IntPtr _mediaPlayerHandle;
    private IntPtr _nextCommand;
    private IntPtr _playPauseCommand;
    private IntPtr _previousCommand;
    private IntPtr _target;
    private IntPtr _nowPlayingInfoCenter;
    private bool _isEnabled;

    public MacOSMediaCommandService(Action playPause, Action next, Action previous)
    {
        _playPauseAction = playPause;
        _nextAction = next;
        _previousAction = previous;

        _mediaPlayerHandle = NativeLibrary.Load(MEDIA_PLAYER_FRAMEWORK);
        InitializeTarget();
        InitializeCommands();
        InitializeNowPlayingInfoCenter();
    }

    public void SetEnabled(bool enabled)
    {
        if (_target == IntPtr.Zero || _commandCenter == IntPtr.Zero || enabled == _isEnabled)
            return;

        if (enabled)
            AddTargets();
        else
        {
            RemoveTargets();
            ClearNowPlayingInfo();
        }

        _isEnabled = enabled;
    }

    public void Dispose()
    {
        SetEnabled(false);

        if (_target != IntPtr.Zero)
        {
            SendVoid(_target, GetSelector("release"));
            _target = IntPtr.Zero;
        }

        _playPauseAction = null;
        _nextAction = null;
        _previousAction = null;
    }

    public void UpdateNowPlaying(
        string? title,
        string? album,
        string? artist,
        double duration,
        double position,
        bool isPlaying)
    {
        if (!_isEnabled || _nowPlayingInfoCenter == IntPtr.Zero)
            return;

        if (string.IsNullOrWhiteSpace(title))
        {
            ClearNowPlayingInfo();
            return;
        }

        var dictionaryClass = objc_getClass("NSMutableDictionary");
        var dictionary = SendIntPtr(
            SendIntPtr(dictionaryClass, GetSelector("alloc")),
            GetSelector("init"));

        try
        {
            AddString(dictionary, "MPMediaItemPropertyTitle", title);
            AddString(dictionary, "MPMediaItemPropertyAlbumTitle", album);
            AddString(dictionary, "MPMediaItemPropertyArtist", artist);
            AddNumber(dictionary, "MPMediaItemPropertyPlaybackDuration", duration);
            AddNumber(dictionary, "MPNowPlayingInfoPropertyElapsedPlaybackTime", position);
            AddNumber(dictionary, "MPNowPlayingInfoPropertyPlaybackRate", isPlaying ? 1 : 0);

            SendVoid(
                _nowPlayingInfoCenter,
                GetSelector("setNowPlayingInfo:"),
                dictionary);
            UpdatePlaybackState((nuint)(title is null ? 3 : isPlaying ? 1 : 2));
        }
        finally
        {
            SendVoid(dictionary, GetSelector("release"));
        }
    }

    private void InitializeTarget()
    {
        var targetClass = objc_getClass(TARGET_CLASS_NAME);
        if (targetClass == IntPtr.Zero)
        {
            var nsObjectClass = objc_getClass("NSObject");
            targetClass = objc_allocateClassPair(nsObjectClass, TARGET_CLASS_NAME, 0);

            AddCommandMethod(targetClass, "handlePlayPause:", PlayPauseCallback);
            AddCommandMethod(targetClass, "handleNext:", NextCallback);
            AddCommandMethod(targetClass, "handlePrevious:", PreviousCallback);
            objc_registerClassPair(targetClass);
        }

        _target = SendIntPtr(SendIntPtr(targetClass, GetSelector("alloc")), GetSelector("init"));
    }

    private void InitializeCommands()
    {
        var commandCenterClass = objc_getClass("MPRemoteCommandCenter");
        _commandCenter = SendIntPtr(commandCenterClass, GetSelector("sharedCommandCenter"));
        _playPauseCommand = SendIntPtr(_commandCenter, GetSelector("togglePlayPauseCommand"));
        _nextCommand = SendIntPtr(_commandCenter, GetSelector("nextTrackCommand"));
        _previousCommand = SendIntPtr(_commandCenter, GetSelector("previousTrackCommand"));
    }

    private void InitializeNowPlayingInfoCenter()
    {
        var infoCenterClass = objc_getClass("MPNowPlayingInfoCenter");
        _nowPlayingInfoCenter = SendIntPtr(infoCenterClass, GetSelector("defaultCenter"));
    }

    private void ClearNowPlayingInfo()
    {
        if (_nowPlayingInfoCenter == IntPtr.Zero)
            return;

        SendVoid(_nowPlayingInfoCenter, GetSelector("setNowPlayingInfo:"), IntPtr.Zero);
        UpdatePlaybackState(3);
    }

    private void UpdatePlaybackState(nuint state)
    {
        SendVoid(_nowPlayingInfoCenter, GetSelector("setPlaybackState:"), state);
    }

    private void AddString(IntPtr dictionary, string keyName, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;

        var stringClass = objc_getClass("NSString");
        var nativeValue = SendIntPtr(stringClass, GetSelector("stringWithUTF8String:"), value);
        AddObject(dictionary, keyName, nativeValue);
    }

    private void AddNumber(IntPtr dictionary, string keyName, double value)
    {
        var numberClass = objc_getClass("NSNumber");
        var nativeValue = SendIntPtr(numberClass, GetSelector("numberWithDouble:"), value);
        AddObject(dictionary, keyName, nativeValue);
    }

    private void AddObject(IntPtr dictionary, string keyName, IntPtr value)
    {
        if (!NativeLibrary.TryGetExport(_mediaPlayerHandle, keyName, out var symbol))
            return;

        var key = Marshal.ReadIntPtr(symbol);
        SendVoid(dictionary, GetSelector("setObject:forKey:"), value, key);
    }

    private void AddTargets()
    {
        AddTarget(_playPauseCommand, "handlePlayPause:");
        AddTarget(_nextCommand, "handleNext:");
        AddTarget(_previousCommand, "handlePrevious:");
    }

    private void RemoveTargets()
    {
        RemoveTarget(_playPauseCommand, "handlePlayPause:");
        RemoveTarget(_nextCommand, "handleNext:");
        RemoveTarget(_previousCommand, "handlePrevious:");
    }

    private void AddTarget(IntPtr command, string selectorName)
    {
        SendVoid(command, GetSelector("setEnabled:"), true);
        SendVoid(
            command,
            GetSelector("addTarget:action:"),
            _target,
            GetSelector(selectorName));
    }

    private void RemoveTarget(IntPtr command, string selectorName)
    {
        SendVoid(
            command,
            GetSelector("removeTarget:action:"),
            _target,
            GetSelector(selectorName));
    }

    private static void AddCommandMethod(IntPtr targetClass, string selectorName, CommandCallback callback)
    {
        class_addMethod(
            targetClass,
            GetSelector(selectorName),
            Marshal.GetFunctionPointerForDelegate(callback),
            "q@:@");
    }

    private static long HandlePlayPause(IntPtr self, IntPtr command, IntPtr commandEvent)
    {
        Dispatch(_playPauseAction);
        return 0;
    }

    private static long HandleNext(IntPtr self, IntPtr command, IntPtr commandEvent)
    {
        Dispatch(_nextAction);
        return 0;
    }

    private static long HandlePrevious(IntPtr self, IntPtr command, IntPtr commandEvent)
    {
        Dispatch(_previousAction);
        return 0;
    }

    private static void Dispatch(Action? action)
    {
        if (action is not null)
            Dispatcher.UIThread.Post(action);
    }

    private static IntPtr GetSelector(string name)
    {
        return sel_registerName(name);
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate long CommandCallback(IntPtr self, IntPtr command, IntPtr commandEvent);

    [DllImport(LIBOBJC)]
    private static extern IntPtr objc_getClass(string name);

    [DllImport(LIBOBJC)]
    private static extern IntPtr objc_allocateClassPair(IntPtr superclass, string name, nuint extraBytes);

    [DllImport(LIBOBJC)]
    private static extern void objc_registerClassPair(IntPtr targetClass);

    [DllImport(LIBOBJC)]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool class_addMethod(IntPtr targetClass, IntPtr selector, IntPtr implementation, string types);

    [DllImport(LIBOBJC)]
    private static extern IntPtr sel_registerName(string name);

    [DllImport(LIBOBJC, EntryPoint = "objc_msgSend")]
    private static extern IntPtr SendIntPtr(IntPtr receiver, IntPtr selector);

    [DllImport(LIBOBJC, EntryPoint = "objc_msgSend")]
    private static extern void SendVoid(IntPtr receiver, IntPtr selector);

    [DllImport(LIBOBJC, EntryPoint = "objc_msgSend")]
    private static extern void SendVoid(IntPtr receiver, IntPtr selector, IntPtr argument1, IntPtr argument2);

    [DllImport(LIBOBJC, EntryPoint = "objc_msgSend")]
    private static extern void SendVoid(IntPtr receiver, IntPtr selector, IntPtr argument);

    [DllImport(LIBOBJC, EntryPoint = "objc_msgSend")]
    private static extern void SendVoid(IntPtr receiver, IntPtr selector, nuint argument);

    [DllImport(LIBOBJC, EntryPoint = "objc_msgSend")]
    private static extern void SendVoid(
        IntPtr receiver,
        IntPtr selector,
        [MarshalAs(UnmanagedType.I1)] bool argument);

    [DllImport(LIBOBJC, EntryPoint = "objc_msgSend")]
    private static extern IntPtr SendIntPtr(
        IntPtr receiver,
        IntPtr selector,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string argument);

    [DllImport(LIBOBJC, EntryPoint = "objc_msgSend")]
    private static extern IntPtr SendIntPtr(IntPtr receiver, IntPtr selector, double argument);
}
