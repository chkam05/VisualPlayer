# Visualization SDK — stages A/B

Experimental SDK/package version: `0.1.0`; public plugin API: major `1`, minor `0`.
The contract will be verified with the first real plugin in stage D before a public
compatibility promise. This assembly references Avalonia 12.1.2 and targets .NET 10.
It has no reference to the application, runtime, UI library or audio engine.

Existing shared types retain their namespaces (`VisualPlayer.Interfaces`,
`VisualPlayer.Visualizations`, `VisualPlayer.ViewModels`, etc.) to keep the built-in
visualizations and compiled XAML working during migration. They now have a single
implementation in this assembly. These namespaces do not imply a reference to the
`VisualPlayer` application. New plugin contracts are in
`VisualPlayer.VisualizationBase.Plugins`; localization is in `.Localization`.

## Plugin contract

Implement `IVisualizationPlugin` with:

- A stable descriptor ID, a localized name key, plugin version, config version and
  capabilities. API version and config version are independent.
- An English dictionary and preset descriptors. Preset configuration JSON must own
  its memory; its mutable color data must be copied by the host before editing.
- Fresh default configurations and explicit schema-aware read/write methods. Return
  owned JSON from `WriteConfiguration`, e.g. `JsonSerializer.SerializeToElement`.
  Throw for unsupported schemas/invalid data; the runtime will retain the original
  settings rather than silently save defaults. That runtime is implemented later.
- Visualization and optional editor factories. The editor context contains the live
  configuration, shared colors and scoped services; no runtime profile is required.

`IVisualizationConfig` requires `INotifyPropertyChanged`. The generic visualization
base accepts any reference type implementing it, including a custom observable model.
The retained `ViewModelBase` and `NotifyPropertyUpdateAttribute` are also available.

Factories, editor creation and rendering execute on the UI thread. `VisualizationFrame`
buffers belong to the host and may be reused after `PreCalculate`. Copy their content
if a snapshot is needed. The host disposes visualization instances on replacement or
shutdown. The base detaches config events and invokes `DisposeCore` once. Editors
must detach their own subscriptions when removed. Do not keep per-instance drawing
state in the shared plugin factory.

Host services expose separate plugin/common translation scopes and diagnostics.
They deliberately expose no application singleton, player, window or unrestricted
configuration service. A translation scope exposes an indexer and property/language
change notifications. The current application `TranslationContext` implements the
contract as a transition adapter.

## Scoped catalogs (stage B)

`TranslationService` owns a selected culture code and catalogs registered by stable
scope ID. `RegisterScope(id, english, optionalDirectory)` copies the English fallback
and loads additional JSON files in the supplied directory. A missing directory is
valid. Individual invalid files produce catalog-scoped `LoadErrors`; they do not
erase the fallback. `LoadLanguage(stream)` supports embedded resources and reports
malformed input to its caller. `AddLanguage` replaces that locale within this scope,
allowing explicit host overrides. Empty localized values are skipped.

Selecting a language updates existing scopes; a scope registered later inherits
the current choice. Keys resolve only within their own selected-language catalog,
then its English dictionary. Unknown keys remain visible and are reported once in
LoadErrors with the catalog ID. Explicit CommonTranslations is the only way to use
the shared UI catalog. Duplicate scope IDs are rejected.

Catalogs notify both `Item` and `Item[]` along with CurrentLanguageCode. `Item` is
needed by Avalonia indexer bindings. Consumers should bind to the catalog, not copy
a translated string once into a view model. Registration/language selection for
bound UI runs on the UI thread; this service does not dispatch threads.

See `tests/VisualPlayer.SamplePlugin` for an SDK-only compile/factory test. It is not
an installed visualization and has no loader, manifest or plugin packaging yet.
Production loader and registry belong to `VisualPlayer.Visualizations` in later stages.
