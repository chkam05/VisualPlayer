using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using VisualPlayer.Contexts;
using VisualPlayer.Models.Player;
using VisualPlayer.ViewModels;

namespace VisualPlayer.Core;

public class PlayListLibraryCore
{
    #region PROPERTIES

    public ObservableCollection<SavedPlayListViewModel> PlayLists { get; } = [];

    #endregion PROPERTIES

    #region CONSTRUCTORS

    public PlayListLibraryCore()
    {
        Reload();
    }

    #endregion CONSTRUCTORS

    public static Track CloneTrack(Track track)
    {
        return new Track
        {
            FilePath = track.FilePath,
            Album = track.Album,
            Artist = track.Artist,
            Duration = track.Duration,
            Title = track.Title
        };
    }

    public SavedPlayListViewModel Create(string name, System.Collections.Generic.IEnumerable<Track>? tracks = null)
    {
        var normalizedName = GetUniqueName(name);
        var fileName = $"{Guid.NewGuid():N}.vpl";
        var playList = new SavedPlayListViewModel(
            normalizedName,
            Path.Combine(ConfigurationContext.PlayListsDirectoryPath, fileName),
            false,
            new ObservableCollection<Track>((tracks ?? []).Select(CloneTrack)));

        playList.Save();
        PlayLists.Add(playList);
        return playList;
    }

    public void Delete(SavedPlayListViewModel playList)
    {
        if (playList.IsNowPlaying)
            return;

        PlayLists.Remove(playList);
        if (File.Exists(playList.FilePath))
            File.Delete(playList.FilePath);
    }

    public SavedPlayListViewModel Duplicate(SavedPlayListViewModel source, string name)
    {
        return Create(name, source.Tracks);
    }

    public string GetUniqueName(string requestedName)
    {
        var baseName = string.IsNullOrWhiteSpace(requestedName) ? "Nowa playlista" : requestedName.Trim();
        var name = baseName;
        var suffix = 2;

        while (PlayLists.Any(item => string.Equals(item.Name, name, StringComparison.CurrentCultureIgnoreCase)))
            name = $"{baseName} ({suffix++})";

        return name;
    }

    public void Reload()
    {
        Directory.CreateDirectory(ConfigurationContext.PlayListsDirectoryPath);
        PlayLists.Clear();

        foreach (var filePath in Directory.EnumerateFiles(
            ConfigurationContext.PlayListsDirectoryPath,
            "*.vpl",
            SearchOption.TopDirectoryOnly))
        {
            try
            {
                var model = JsonSerializer.Deserialize<PlayListFile>(File.ReadAllText(filePath));
                if (model is null)
                    continue;

                PlayLists.Add(new SavedPlayListViewModel(
                    model.Name,
                    filePath,
                    false,
                    new ObservableCollection<Track>(model.Tracks)));
            }
            catch (JsonException)
            {
                // A damaged playlist is skipped without blocking the remaining library.
            }
            catch (IOException)
            {
                // A temporarily unavailable file is skipped.
            }
        }
    }
}
