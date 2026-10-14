using System;

namespace VisualPlayer.Interfaces;

internal interface IInternalContent
{
    event EventHandler? CloseRequested;

    void OnClosedByController();
}
