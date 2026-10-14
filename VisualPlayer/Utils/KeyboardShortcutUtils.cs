using System;
using System.Collections.Generic;
using Avalonia.Input;

namespace VisualPlayer.Utils;

public static class KeyboardShortcutUtils
{
    public static string? FromKeyEvent(KeyEventArgs e)
    {
        if (IsModifierKey(e.Key))
            return null;

        var parts = new List<string>();

        if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
            parts.Add("Ctrl");
        if (e.KeyModifiers.HasFlag(KeyModifiers.Shift))
            parts.Add("Shift");
        if (e.KeyModifiers.HasFlag(KeyModifiers.Alt))
            parts.Add("Alt");
        if (e.KeyModifiers.HasFlag(KeyModifiers.Meta))
            parts.Add("Meta");

        parts.Add(e.Key.ToString());
        return string.Join('+', parts);
    }

    public static bool Matches(KeyEventArgs e, string? shortcut)
    {
        if (string.IsNullOrWhiteSpace(shortcut))
            return false;

        var parts = shortcut.Split('+', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var modifiers = KeyModifiers.None;
        Key? key = null;

        foreach (var part in parts)
        {
            if (part.Equals("Ctrl", StringComparison.OrdinalIgnoreCase))
                modifiers |= KeyModifiers.Control;
            else if (part.Equals("Shift", StringComparison.OrdinalIgnoreCase))
                modifiers |= KeyModifiers.Shift;
            else if (part.Equals("Alt", StringComparison.OrdinalIgnoreCase))
                modifiers |= KeyModifiers.Alt;
            else if (part.Equals("Meta", StringComparison.OrdinalIgnoreCase))
                modifiers |= KeyModifiers.Meta;
            else if (Enum.TryParse<Key>(part, true, out var parsedKey))
                key = parsedKey;
            else
                return false;
        }

        return key == e.Key && modifiers == e.KeyModifiers;
    }

    private static bool IsModifierKey(Key key)
    {
        return key is Key.LeftCtrl or Key.RightCtrl
            or Key.LeftShift or Key.RightShift
            or Key.LeftAlt or Key.RightAlt
            or Key.LWin or Key.RWin;
    }
}
