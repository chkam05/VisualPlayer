using Avalonia.Platform;
using VisualPlayer.UI.Resources;
using VisualPlayer.VisualizationBase.Localization;

namespace VisualPlayer.UI.Localization;

public static class UiCatalogs
{
    public static ITranslationScope English { get; } = new TranslationService().RegisterScope("visualplayer.ui", EnglishTranslations.Values);

    public static TranslationCatalog Register(TranslationService service, string? directory = null)
    {
        var catalog = service.RegisterScope("visualplayer.ui", EnglishTranslations.Values);
        using (var stream = AssetLoader.Open(new Uri("avares://VisualPlayer.UI/Lang/pl-PL.json")))
            catalog.LoadLanguage(stream);
        if (directory is not null) catalog.LoadDirectory(directory);
        return catalog;
    }
}
