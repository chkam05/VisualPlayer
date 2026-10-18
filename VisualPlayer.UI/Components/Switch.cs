using System;
using Avalonia.Controls.Primitives;

namespace VisualPlayer.Components;

public class Switch : ToggleButton
{
    protected override Type StyleKeyOverride => typeof(Switch);
}