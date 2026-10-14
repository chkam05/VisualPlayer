using Avalonia.Media.Imaging;

namespace VisualPlayer.Models.Player;

public class CoverEntry
{
    public byte[]? Data { get; set; }
    public Bitmap? Image { get; set; }
    public bool ImageLoadAttempted { get; set; }
    public int ReferenceCount { get; set; }
}