# Shared UI — stage B

This library targets .NET 10 / Avalonia 12.1.2 and references only the visualization
SDK, Avalonia/Fluent and Lucide. It does not reference the application or runtime.
Existing component/control namespaces are preserved for source and compiled XAML
compatibility during migration. A namespace beginning with `VisualPlayer` does not
imply a reference to the executable assembly.

The library includes all existing `Components`, shared settings/expand controls,
the palette and gradient editor, base control themes, shared enum converters,
fonts/licenses and the common color configuration editor. Player-specific controls,
dialogs and application settings remain in the application.

## Initialize resources

After initializing Avalonia, install Fluent once and import the library dictionary:

```csharp
app.Styles.Add(new FluentTheme());
app.Resources.MergedDictionaries.Add((ResourceDictionary)AvaloniaXamlLoader.Load(
    new Uri("avares://VisualPlayer.UI/Themes/Generic.axaml")));

var languages = new TranslationService("pl-PL");
var common = UiCatalogs.Register(languages);
var resourceSubscription = UiResources.BindTranslations(app.Resources, common);
```

Keep `resourceSubscription` for the lifetime of that resource host and dispose it
when the host closes. A standalone editor can publish into its own `Resources`
instead of application resources. No `ApplicationContext` is required.

English defaults are embedded in `Resources/EnglishTranslations.cs`; Polish is
packaged as `avares://VisualPlayer.UI/Lang/pl-PL.json`. An optional directory passed
to `UiCatalogs.Register` supplies catalog overrides. Plugin-local catalogs are
registered separately with the same `TranslationService`. One `SelectLanguage`
updates the catalogs without modifying edited configuration.

## Appearance

`Themes/Defaults.axaml` supplies defaults. A host can override keys in its resource
dictionary, for example `UI.ThemeForegroundBrush`, `UI.ThemeBackgroundBrush`,
`UI.ThemeShadeBackgroundBrush`, `UI.AccentColorBrush`, `UI.ControlSurfaceBrush` and
the `UI.Colorized.<normal-property>.<accent-property>` brush variants.
Themes use `DynamicResource`; changing a resource updates an open editor.
`UI.Text.<key>` and `UI.TranslationScope` are supplied by `BindTranslations`.
Font resources are `ApplicationFontFamily`, `ElectronicDisplayFontFamily` and
`Lcd14DisplayFontFamily`, with assets in this assembly.

The application's `UiResourceBridge` maps its own AppearanceConfig into these keys,
including opacity and colorization. The bridge is an application-side adapter, not
a dependency of this library. UI defaults remain useful in standalone hosts.

## Common color editor

`VisualPlayer.UI.Views.VisualisationColorEditor` accepts the SDK context explicitly:

```csharp
return new VisualisationColorEditor { EditorContext = context };
```

It edits `context.Color` in place and uses `context.Services.CommonTranslations`.
It does not know the runtime profile type or replace the plugin's configuration.
All built-in settings editors receive this context directly as of stage C; the old
profile adapter has been removed. Custom plugin settings can use their own UserControl and
include the common editor as a child; see `tests/VisualPlayer.SampleUiPlugin`.

`ColorPaletteControl.AddColorRequested` is still a routed event. A host provides its
color selection dialog, as the application already does. The shared library does
not create application-specific message windows.

## Enum labels and live translation

The shared `*ToTitleConverter` types accept both single and multi bindings. A single
binding defaults to English unless its converter receives an explicit scope.
For live language updates use three MultiBinding inputs: the enum value, its scope,
and `scope.CurrentLanguageCode`. Language notifications refresh labels while leaving
the selected enum unchanged.

For popup templates, carry the scope in each item's data using
`LocalizedEnumValue<T>`. Bind SelectedValue through its `Value` property, and pass
`Value`, `Translations`, `Translations.CurrentLanguageCode` to the converter.
Do not search for an editor ancestor from popup item templates: their visual tree
can be detached. Both the gradient selector and color-mode selector use this pattern.

Language registration/selection and resource publication for an attached editor
must execute on the UI thread. The catalogs are not a cross-thread UI dispatcher.

## Validation

```powershell
dotnet run --project tests/VisualPlayer.UiChecks -c Release -p:UsedAvaloniaProducts=
dotnet run --project tests/VisualPlayer.VisualizationChecks -c Release -p:UsedAvaloniaProducts=
powershell -NoProfile -ExecutionPolicy Bypass -File scripts/Test-Translations.ps1
```

The first executable references only SDK/UI and a separately compiled sample view.
It verifies that the application assembly is never loaded. It checks open-editor
and open-popup language changes, resource updates, fonts, independent catalogs,
English fallback, color/gradient editing, configuration replacement, detach/reattach,
numeric clamping and ScrollTextBlock overflow behavior. These are headless Windows
checks, not screenshot comparisons or macOS/Linux release validation.
