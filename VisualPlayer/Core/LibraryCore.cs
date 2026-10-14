using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VisualPlayer.Models.Player;
using VisualPlayer.Utils;
using VisualPlayer.ViewModels.Config;

namespace VisualPlayer.Core;

public class LibraryCore
{
    private readonly LibraryConfig _config;
    private readonly CoverCache _coverCache;
    private readonly SemaphoreSlim _scanLock = new(1, 1);

    #region PROPERTIES

    public ObservableCollection<Track> Tracks { get; } = [];
    public CoverCache CoverCache => _coverCache;

    #endregion PROPERTIES

    #region EVENTS

    public event EventHandler? Refreshed;

    #endregion EVENTS

    #region CONSTRUCTORS

    public LibraryCore(LibraryConfig config, CoverCache coverCache)
    {
        _config = config;
        _coverCache = coverCache;
    }

    #endregion CONSTRUCTORS

    #region SCANNING

    public Task RefreshAsync()
    {
        return ScanAsync(_config.Folders);
    }

    public Task RefreshFolderAsync(LibraryFolderConfig folder)
    {
        return ScanAsync(_config.Folders);
    }


    private static IEnumerable<string> EnumerateFiles(string root, bool recursive)
    {
        var pending = new Stack<string>();
        pending.Push(root);

        while (pending.TryPop(out var directory))
        {
            string[] files;

            try
            {
                files = Directory.GetFiles(directory);
            }
            catch
            {
                continue;
            }

            foreach (var file in files)
                yield return file;

            if (!recursive)
                continue;

            string[] directories;

            try
            {
                directories = Directory.GetDirectories(directory);
            }
            catch
            {
                continue;
            }

            foreach (var child in directories)
                pending.Push(child);
        }
    }

    private async Task ScanAsync(IEnumerable<LibraryFolderConfig> folders)
    {
        await _scanLock.WaitAsync();

        try
        {
            var folderSnapshot = folders
                .Where(folder => Directory.Exists(folder.Path))
                .Select(folder => (folder.Path, folder.IncludeSubdirectories))
                .ToArray();

            var tracks = await Task.Run(() => ScanFolders(folderSnapshot));
            Tracks.Clear();

            foreach (var track in tracks)
                Tracks.Add(track);

            Refreshed?.Invoke(this, EventArgs.Empty);
        }
        finally
        {
            _scanLock.Release();
        }
    }

    private List<Track> ScanFolders(IEnumerable<(string Path, bool Recursive)> folders)
    {
        var result = new List<Track>();
        var knownPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var folder in folders)
        {
            foreach (var path in EnumerateFiles(folder.Path, folder.Recursive))
            {
                if (!AudioFileUtils.IsSupported(path) || !knownPaths.Add(path))
                    continue;

                var track = new Track { FilePath = path };

                if (TrackMetadataUtils.TryLoadMetadata(track, out var coverData) && coverData is not null)
                    track.CoverId = _coverCache.Add(coverData);

                result.Add(track);
            }
        }

        return result
            .OrderBy(track => track.DisplayTitle, StringComparer.CurrentCultureIgnoreCase)
            .ToList();
    }

    #endregion SCANNING
}
