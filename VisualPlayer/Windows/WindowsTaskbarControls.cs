using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Avalonia.Controls;
using Avalonia.Threading;
using VisualPlayer.Contexts;

namespace VisualPlayer.Windows;

/// <summary>Native thumbnail transport controls. Created only on Windows.</summary>
[SupportedOSPlatform("windows")]
internal sealed class WindowsTaskbarControls : IDisposable
{
    private const uint WmCommand = 0x0111;
    private const int ThbnClicked = 0x1800;
    private const uint PreviousId = 0x5101;
    private const uint PlayPauseId = 0x5102;
    private const uint NextId = 0x5103;

    private readonly Window _window;
    private readonly PlayerDataContext _player;
    private readonly TranslationContext _translations;
    private readonly Action _previous;
    private readonly Action _playPause;
    private readonly Action _next;
    private readonly uint _taskbarButtonCreated;
    private readonly IntPtr[] _icons = new IntPtr[4];
    private ITaskbarList3? _taskbar;
    private bool _buttonsAdded;
    private bool _disposed;

    public WindowsTaskbarControls(Window window, PlayerDataContext player,
        TranslationContext translations, Action previous, Action playPause, Action next)
    {
        _window = window;
        _player = player;
        _translations = translations;
        _previous = previous;
        _playPause = playPause;
        _next = next;
        _taskbarButtonCreated = RegisterWindowMessage("TaskbarButtonCreated");

        // Attach before Show: Explorer signals when the taskbar button is ready.
        Win32Properties.AddWndProcHookCallback(window, WndProc);
        _player.PropertyChanged += Player_PropertyChanged;
        _translations.LanguageChanged += Translations_LanguageChanged;
    }

    private IntPtr WndProc(IntPtr hwnd, uint message, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (_disposed)
            return IntPtr.Zero;

        if (_taskbarButtonCreated != 0 && message == _taskbarButtonCreated)
        {
            // Explorer also sends this again after its taskbar is recreated.
            _buttonsAdded = false;
            ReleaseTaskbar();
            UpdateButtons();
        }
        else if (message == WmCommand && ((wParam.ToInt64() >> 16) & 0xffff) == ThbnClicked)
        {
            Action? action = (uint)(wParam.ToInt64() & 0xffff) switch
            {
                PreviousId => _previous,
                PlayPauseId => _playPause,
                NextId => _next,
                _ => null
            };

            if (action is not null)
            {
                handled = true;
                // Leave the native callback before changing playback/UI state.
                Dispatcher.UIThread.Post(() => { if (!_disposed) action(); });
            }
        }

        return IntPtr.Zero;
    }

    private void Player_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(PlayerDataContext.IsPlaying))
            RequestUpdate();
    }

    private void Translations_LanguageChanged(object? sender, EventArgs e) => RequestUpdate();

    private void RequestUpdate()
    {
        if (Dispatcher.UIThread.CheckAccess())
        {
            if (_buttonsAdded)
                UpdateButtons();
        }
        else
        {
            Dispatcher.UIThread.Post(RequestUpdate);
        }
    }

    private void UpdateButtons()
    {
        if (_disposed || _window.TryGetPlatformHandle() is not { Handle: var hwnd } || hwnd == IntPtr.Zero)
            return;

        try
        {
            if (_taskbar is null)
            {
                var type = Type.GetTypeFromCLSID(new Guid("56FDF344-FD6D-11D0-958A-006097C9A090"), true)!;
                _taskbar = (ITaskbarList3)Activator.CreateInstance(type)!;
                if (!Succeeded(_taskbar.HrInit()))
                {
                    ReleaseTaskbar();
                    return;
                }
            }

            for (var i = 0; i < _icons.Length; i++)
            {
                if (_icons[i] == IntPtr.Zero)
                    _icons[i] = CreateTransportIcon(i);
            }

            ThumbButton[] buttons =
            [
                Button(PreviousId, _icons[0], "PLAYER_TOOLTIP_PREVIOUS_TRACK"),
                Button(PlayPauseId, _icons[_player.IsPlaying ? 2 : 1], "PLAYER_TOOLTIP_PLAY_PAUSE"),
                Button(NextId, _icons[3], "PLAYER_TOOLTIP_NEXT_TRACK")
            ];

            var result = _buttonsAdded
                ? _taskbar.ThumbBarUpdateButtons(hwnd, (uint)buttons.Length, buttons)
                : _taskbar.ThumbBarAddButtons(hwnd, (uint)buttons.Length, buttons);
            if (Succeeded(result))
                _buttonsAdded = true;
        }
        catch (Exception exception) when (exception is COMException or Win32Exception)
        {
            // A missing/restarting Explorer must not prevent playback.
            Debug.WriteLine($"Could not update taskbar transport controls: {exception.Message}");
        }
    }

    private static bool Succeeded(int result)
    {
        if (result >= 0)
            return true;

        Debug.WriteLine($"Taskbar transport controls returned HRESULT 0x{result:X8}.");
        return false;
    }

    private ThumbButton Button(uint id, IntPtr icon, string translationKey) => new()
    {
        Mask = 0x2 | 0x4 | 0x8, // THB_ICON | THB_TOOLTIP | THB_FLAGS
        Id = id,
        Icon = icon,
        Tooltip = _translations[translationKey],
        Flags = 0
    };

    private static IntPtr CreateTransportIcon(int kind)
    {
        // Monochrome icons use native AND/XOR masks: transparent outside the glyph.
        const int size = 32;
        var andMask = new byte[size * size / 8];
        var xorMask = new byte[andMask.Length];
        Array.Fill(andMask, (byte)0xff);
        for (var y = 6; y < 26; y++)
        {
            for (var x = 5; x < 27; x++)
            {
                var triangleWidth = Math.Min(y - 6, 25 - y);
                var filled = kind switch
                {
                    0 => x is >= 6 and <= 8 || x >= 22 - triangleWidth * 3 / 2 && x <= 22,
                    1 => x >= 10 && x <= 10 + triangleWidth * 3 / 2,
                    2 => x is >= 8 and <= 12 or >= 19 and <= 23,
                    _ => x is >= 23 and <= 25 || x >= 9 && x <= 9 + triangleWidth * 3 / 2
                };
                if (!filled)
                    continue;

                var offset = y * (size / 8) + x / 8;
                var bit = (byte)(0x80 >> (x % 8));
                andMask[offset] &= (byte)~bit;
                xorMask[offset] |= bit;
            }
        }

        var icon = CreateIcon(IntPtr.Zero, size, size, 1, 1, andMask, xorMask);
        if (icon == IntPtr.Zero)
            throw new Win32Exception(Marshal.GetLastWin32Error());
        return icon;
    }

    private void ReleaseTaskbar()
    {
        if (_taskbar is not null)
        {
            Marshal.ReleaseComObject(_taskbar);
            _taskbar = null;
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;
        _player.PropertyChanged -= Player_PropertyChanged;
        _translations.LanguageChanged -= Translations_LanguageChanged;
        Win32Properties.RemoveWndProcHookCallback(_window, WndProc);
        ReleaseTaskbar();
        foreach (var icon in _icons)
        {
            if (icon != IntPtr.Zero)
                DestroyIcon(icon);
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct ThumbButton
    {
        public uint Mask;
        public uint Id;
        public uint Bitmap;
        public IntPtr Icon;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string Tooltip;
        public uint Flags;
    }

    // Keep the inherited methods in COM vtable order, even when unused.
    [ComImport, Guid("EA1AFB91-9E28-4B86-90E9-9E9F8A5EEFAF")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface ITaskbarList3
    {
        [PreserveSig] int HrInit();
        [PreserveSig] int AddTab(IntPtr hwnd);
        [PreserveSig] int DeleteTab(IntPtr hwnd);
        [PreserveSig] int ActivateTab(IntPtr hwnd);
        [PreserveSig] int SetActiveAlt(IntPtr hwnd);
        [PreserveSig] int MarkFullscreenWindow(IntPtr hwnd, [MarshalAs(UnmanagedType.Bool)] bool fullscreen);
        [PreserveSig] int SetProgressValue(IntPtr hwnd, ulong completed, ulong total);
        [PreserveSig] int SetProgressState(IntPtr hwnd, uint flags);
        [PreserveSig] int RegisterTab(IntPtr tab, IntPtr mdi);
        [PreserveSig] int UnregisterTab(IntPtr tab);
        [PreserveSig] int SetTabOrder(IntPtr tab, IntPtr insertBefore);
        [PreserveSig] int SetTabActive(IntPtr tab, IntPtr mdi, uint flags);
        [PreserveSig] int ThumbBarAddButtons(IntPtr hwnd, uint count,
            [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] ThumbButton[] buttons);
        [PreserveSig] int ThumbBarUpdateButtons(IntPtr hwnd, uint count,
            [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] ThumbButton[] buttons);
    }

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern uint RegisterWindowMessage(string message);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr CreateIcon(IntPtr instance, int width, int height,
        byte planes, byte bitsPerPixel, byte[] andMask, byte[] xorMask);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DestroyIcon(IntPtr icon);
}
