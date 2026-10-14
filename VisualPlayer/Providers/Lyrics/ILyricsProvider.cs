using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VisualPlayer.Models.Player;

namespace VisualPlayer.Providers.Lyrics;

public interface ILyricsProvider
{
    string Name { get; }

    Task<IReadOnlyList<LyricsProviderResult>> SearchAsync(
        string query,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LyricsProviderResult>> SearchAsync(
        Track track,
        CancellationToken cancellationToken = default);
}
