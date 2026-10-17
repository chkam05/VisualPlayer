using System;

namespace VisualPlayer.Contexts;

public sealed class PlayerMiniDataContext
{
    public InfoBarContext InfoBar { get; }
    public PlayerDataContext Player { get; }

    public PlayerMiniDataContext(PlayerDataContext player, InfoBarContext infoBar)
    {
        Player = player ?? throw new ArgumentNullException(nameof(player));
        InfoBar = infoBar ?? throw new ArgumentNullException(nameof(infoBar));
    }
}
