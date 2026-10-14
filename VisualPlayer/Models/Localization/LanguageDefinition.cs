using System;
using System.Collections.Generic;

namespace VisualPlayer.Models.Localization;

public class LanguageDefinition
{
    internal IReadOnlyDictionary<string, string> Translations { get; }

    public string Code { get; }
    public bool IsBuiltIn { get; }
    public string Name { get; }


    internal LanguageDefinition(string code, string name, IReadOnlyDictionary<string, string> translations, bool isBuiltIn = false)
    {
        Code = code;
        Name = name;
        Translations = translations;
        IsBuiltIn = isBuiltIn;
    }

    public override string ToString()
    {
        return Name;
    }
}
