using Avalonia.Controls;
using VisualPlayer.UI.Resources;
using VisualPlayer.VisualizationBase.Localization;

namespace VisualPlayer.UI.Localization;

/// <summary>Publishes shared text resources into the host/editor dictionary without accessing Application.Current.</summary>
public static class UiResources
{
    public static IDisposable BindTranslations(IResourceDictionary resources, ITranslationScope scope)
        => new Subscription(resources, scope);

    private sealed class Subscription : IDisposable
    {
        private readonly IResourceDictionary _resources;
        private readonly ITranslationScope _scope;
        public Subscription(IResourceDictionary resources, ITranslationScope scope)
        {
            _resources = resources;
            _scope = scope;
            _scope.LanguageChanged += Update;
            Publish();
        }
        private void Update(object? sender, EventArgs e) => Publish();
        private void Publish()
        {
            _resources["UI.TranslationScope"] = _scope;
            foreach (var key in EnglishTranslations.Values.Keys) _resources["UI.Text." + key] = _scope[key];
        }
        public void Dispose() => _scope.LanguageChanged -= Update;
    }
}
