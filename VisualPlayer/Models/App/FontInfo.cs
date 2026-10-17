namespace VisualPlayer.Models.App;

public sealed class FontInfo
{
    public string Name { get; }
    public string Author { get; }
    public string Website { get; }

    public FontInfo(string name, string author, string website)
    {
        Name = name;
        Author = author;
        Website = website;
    }
}
