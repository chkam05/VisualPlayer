using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using VisualPlayer.Models.Player;
using VisualPlayer.Utils;

namespace VisualPlayer.Providers.Lyrics;

public class LrcLibLyricsProvider : ILyricsProvider, IDisposable
{
    #region CONST

    private const string API_URL = "https://lrclib.net/api/search";
    private const int CACHE_LIFETIME_HOURS = 24;
    private const int REQUEST_TIMEOUT_SECONDS = 15;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    #endregion CONST

    private readonly string _cacheDirectoryPath;
    private readonly HttpClient _httpClient;

    #region PROPERTIES

    public string Name
    {
        get => "LRCLIB";
    }

    #endregion PROPERTIES

    #region CONSTRUCTORS

    public LrcLibLyricsProvider(string cacheDirectoryPath)
    {
        _cacheDirectoryPath = cacheDirectoryPath;
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(REQUEST_TIMEOUT_SECONDS)
        };
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
            $"{AppInfoGetter.AppName}/{AppInfoGetter.AppVersion} (https://github.com/chkam05/VisualPlayer)");
    }

    #endregion CONSTRUCTORS

    #region SEARCH

    public Task<IReadOnlyList<LyricsProviderResult>> SearchAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
            return Task.FromResult<IReadOnlyList<LyricsProviderResult>>([]);

        var url = $"{API_URL}?q={Uri.EscapeDataString(query.Trim())}";
        return SendSearchAsync(url, cancellationToken);
    }

    public Task<IReadOnlyList<LyricsProviderResult>> SearchAsync(
        Track track,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(track);

        var query = new StringBuilder(API_URL)
            .Append("?track_name=")
            .Append(Uri.EscapeDataString(track.DisplayTitle));

        if (!string.IsNullOrWhiteSpace(track.Artist))
        {
            query.Append("&artist_name=")
                .Append(Uri.EscapeDataString(track.Artist));
        }

        if (!string.IsNullOrWhiteSpace(track.Album))
        {
            query.Append("&album_name=")
                .Append(Uri.EscapeDataString(track.Album));
        }

        return SendSearchAsync(query.ToString(), cancellationToken);
    }

    private async Task<IReadOnlyList<LyricsProviderResult>> SendSearchAsync(
        string url,
        CancellationToken cancellationToken)
    {
        var cached = await ReadCacheAsync(url, cancellationToken);

        if (cached is not null)
            return cached;

        using var response = await _httpClient.GetAsync(url, cancellationToken);

        if (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            var retryAfter = response.Headers.RetryAfter?.Delta;
            throw new HttpRequestException(retryAfter.HasValue
                ? $"LRCLIB ograniczył liczbę zapytań. Spróbuj ponownie za {Math.Ceiling(retryAfter.Value.TotalSeconds)} s."
                : "LRCLIB ograniczył liczbę zapytań. Spróbuj ponownie później.");
        }

        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var results = JsonSerializer.Deserialize<LyricsProviderResult[]>(json, JsonOptions) ?? [];

        await WriteCacheAsync(url, json, cancellationToken);
        return results;
    }

    #endregion SEARCH

    #region CACHE

    private async Task<IReadOnlyList<LyricsProviderResult>?> ReadCacheAsync(
        string key,
        CancellationToken cancellationToken)
    {
        var filePath = GetCacheFilePath(key);

        if (!File.Exists(filePath))
            return null;

        try
        {
            var content = await File.ReadAllTextAsync(filePath, cancellationToken);
            var separatorIndex = content.IndexOf('\n');

            if (separatorIndex <= 0
                || !DateTimeOffset.TryParse(
                    content[..separatorIndex],
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.RoundtripKind,
                    out var createdAt)
                || DateTimeOffset.UtcNow - createdAt > TimeSpan.FromHours(CACHE_LIFETIME_HOURS))
            {
                File.Delete(filePath);
                return null;
            }

            return JsonSerializer.Deserialize<LyricsProviderResult[]>(
                content[(separatorIndex + 1)..],
                JsonOptions);
        }
        catch (JsonException)
        {
            File.Delete(filePath);
            return null;
        }
        catch (IOException)
        {
            return null;
        }
        catch (UnauthorizedAccessException)
        {
            return null;
        }
    }

    private async Task WriteCacheAsync(
        string key,
        string json,
        CancellationToken cancellationToken)
    {
        try
        {
            Directory.CreateDirectory(_cacheDirectoryPath);
            var content = $"{DateTimeOffset.UtcNow:O}\n{json}";
            await File.WriteAllTextAsync(GetCacheFilePath(key), content, cancellationToken);
        }
        catch (IOException)
        {
            // A cache write failure must not invalidate a successful response.
        }
        catch (UnauthorizedAccessException)
        {
            // A read-only cache directory must not invalidate a successful response.
        }
    }

    private string GetCacheFilePath(string key)
    {
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(key)));
        return Path.Combine(_cacheDirectoryPath, $"{hash}.json");
    }

    #endregion CACHE

    #region DISPOSE

    public void Dispose()
    {
        _httpClient.Dispose();
    }

    #endregion DISPOSE
}
