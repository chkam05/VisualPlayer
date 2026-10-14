using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using VisualPlayer.ViewModels;

namespace VisualPlayer.Contexts;

public abstract class ConfigurationContextBase : ViewModelBase
{
    #region CONST

    protected const string CATALOG_PATH = "VisualPlayer";

    protected static readonly JsonSerializerOptions JsonOptions = new()
    {
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };

    #endregion CONST

    #region PROPERTIES

    public static string DirectoryPath
    {
        get
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(appDataPath, CATALOG_PATH);
        }
    }

    #endregion PROPERTIES

    protected static void EnsureConfigDirectoryExists()
    {
        if (!Directory.Exists(DirectoryPath))
            Directory.CreateDirectory(DirectoryPath);
    }

    protected static T LoadFromFile<T>(string filePath, Func<T> createDefault)
    {
        EnsureConfigDirectoryExists();

        if (!File.Exists(filePath))
            return createDefault();

        try
        {
            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<T>(json, JsonOptions) ?? createDefault();
        }
        catch (JsonException)
        {
            return createDefault();
        }
    }

    protected void SaveToFile<T>(string filePath, T value)
    {
        EnsureConfigDirectoryExists();
        var json = JsonSerializer.Serialize(value, JsonOptions);
        File.WriteAllText(filePath, json);
    }
}