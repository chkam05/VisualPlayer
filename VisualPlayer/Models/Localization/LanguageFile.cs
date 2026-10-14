using System.Collections.Generic;

namespace VisualPlayer.Models.Localization;

public class LanguageFile
{
    public LanguageHeader? Header { get; set; }
    public Dictionary<string, string?>? Translations { get; set; }
}
