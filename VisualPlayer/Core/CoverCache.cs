using System;
using System.Collections.Concurrent;
using System.IO;
using System.Security.Cryptography;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace VisualPlayer.Core;

public class CoverCache
{
    private readonly ConcurrentDictionary<string, byte[]> _covers = new();
    private readonly ConcurrentDictionary<string, IImage> _images = new();

    
    public string Add(byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var id = Convert.ToHexString(SHA256.HashData(data));

        if (!_covers.ContainsKey(id))
            _covers.TryAdd(id, data);

        return id;
    }

    public IImage? GetImage(string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;

        if (_images.TryGetValue(id, out var image))
            return image;

        image = CreateImage(id);

        if (image is null)
            return null;

        return _images.GetOrAdd(id, image);
    }

    public void Remove(string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return;

        _covers.TryRemove(id, out _);

        if (_images.TryRemove(id, out var image))
        {
            if (image is IDisposable disposable)
                disposable.Dispose();
        }
    }

    #region UTILITIES

    private IImage? CreateImage(string id)
    {
        if (!_covers.TryGetValue(id, out var data))
            return null;

        try
        {
            using var stream = new MemoryStream(data);
            var image = new Bitmap(stream);

            _covers.TryRemove(id, out _);
            return image;
        }
        catch
        {
            _covers.TryRemove(id, out _);
            return null;
        }
    }

    #endregion UTILITIES
}