namespace VisualPlayer.Models.App;

public class NuGetPackageInfo
{
    public string Name { get; }
    public string Version { get; }


    public NuGetPackageInfo(string name, string version)
    {
        Name = name;
        Version = version;
    }
}
