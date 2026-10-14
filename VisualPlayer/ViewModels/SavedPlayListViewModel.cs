using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using VisualPlayer.Attributes;
using VisualPlayer.Models.Player;

namespace VisualPlayer.ViewModels;

public class SavedPlayListViewModel : ViewModelBase
{
    private string _name;

    #region PROPERTIES

    public string FilePath { get; }
    public bool IsNowPlaying { get; }
    public ObservableCollection<Track> Tracks { get; }

    [NotifyPropertyUpdate]
    public string Name
    {
        get => _name;
        set => UpdateProperty(ref _name, value.Trim());
    }
    #endregion PROPERTIES

    #region CONSTRUCTORS

    public SavedPlayListViewModel(string name, string filePath, bool isNowPlaying, ObservableCollection<Track> tracks)
    {
        _name = name;
        FilePath = filePath;
        IsNowPlaying = isNowPlaying;
        Tracks = tracks;
    }

    #endregion CONSTRUCTORS

    public void Save()
    {
        if (IsNowPlaying || string.IsNullOrWhiteSpace(FilePath))
            return;

        var model = new PlayListFile
        {
            Name = Name,
            Tracks = [.. Tracks]
        };

        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
        File.WriteAllText(
            FilePath,
            JsonSerializer.Serialize(model, new JsonSerializerOptions { WriteIndented = true }));
    }
}
