using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
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
        LibraryFolderConfig[] folderSnapshot = [];

        try
        {
            folderSnapshot = folders.ToArray();
            var tracks = new List<Track>();
            var knownPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var folder in folderSnapshot)
            {
                folder.ScanPercentage = null;
                folder.IsScanning = true;
            }

            foreach (var folder in folderSnapshot)
            {
                var path = folder.Path;
                var recursive = folder.IncludeSubdirectories;
                var files = await Task.Run(() => EnumerateFiles(path, recursive)
                    .Where(AudioFileUtils.IsSupported).ToArray());
                folder.ScanPercentage = 0;
                tracks.AddRange(await Task.Run(() => ScanFolder(folder, files, knownPaths)));
                folder.IsScanning = false;
                folder.ScanPercentage = null;
            }

            tracks.Sort((left, right) => StringComparer.CurrentCultureIgnoreCase.Compare(
                left.DisplayTitle, right.DisplayTitle));
            Tracks.Clear();

            foreach (var track in tracks)
                Tracks.Add(track);

            Refreshed?.Invoke(this, EventArgs.Empty);
        }
        finally
        {
            foreach (var folder in folderSnapshot)
            {
                folder.IsScanning = false;
                folder.ScanPercentage = null;
            }
            _scanLock.Release();
        }
    }

    private async Task<List<Track>> ScanFolder(
        LibraryFolderConfig folder, string[] files, HashSet<string> knownPaths)
    {
        var result = new List<Track>();
        var progressTimer = Stopwatch.StartNew();

        for (var index = 0; index < files.Length; index++)
        {
            var path = files[index];
            if (knownPaths.Add(path))
            {
                var track = new Track { FilePath = path };

                if (TrackMetadataUtils.TryLoadMetadata(track, out var coverData) && coverData is not null)
                    track.CoverId = _coverCache.Add(coverData);

                result.Add(track);
            }

            // Keep large libraries from flooding the UI dispatcher with updates.
            if (progressTimer.ElapsedMilliseconds >= 100 || index == files.Length - 1)
            {
                var percentage = (int)((index + 1L) * 100 / files.Length);
                await Dispatcher.UIThread.InvokeAsync(() => folder.ScanPercentage = percentage);
                progressTimer.Restart();
            }
        }

        return result;
    }

    #endregion SCANNING
}
