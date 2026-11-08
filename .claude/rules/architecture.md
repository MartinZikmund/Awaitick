---
description: Project layout, MVVM, DI, navigation, messaging and localization conventions
---

# Architecture and MVVM

## Project layout
- **`src/Awaitick.Core`** is the testable layer: view models, service interfaces and shared implementations, models, messages, configuration. It references Uno, so UI types such as `ElementTheme` are available. Using one isn't a reason to move a view model to the head.
- **`src/Awaitick`** is the head: views, controls, platform-specific service implementations, and composition (`App.xaml.cs`, `WindowShell`).
- **Default to Core for view models and logic.**
- Each service type has its own folder under `Services/`, with the interface next to the implementation.

## ViewModels
- Derive from `PageViewModel` for pages, or `ViewModelBase` (which extends `ObservableRecipient`) otherwise.
- Observable state uses the partial-property form: `[ObservableProperty] public partial bool IsBusy { get; set; }`. React with the generated `partial void OnIsBusyChanged(...)`.
- Commands use `[RelayCommand]`.
- Override `ViewNavigatedTo(object? parameter)` for navigation parameters.
- View models never touch controls. When one must drive its view, define a small view-service interface in Core, like `ICountdownEditorViewService`.

## Views
- XAML can't reference a generic base, so each page has a non-generic intermediate base: `public abstract partial class XViewBase : PageBase<XViewModel>`, then `public sealed partial class XView : XViewBase`. The XAML root is `<local:XViewBase x:Class="Awaitick.Views.XView" ...>`.
- Prefer `x:Bind`.

## Dependency injection
- Everything is registered in `CountdownsApp.ConfigureServices` (`src/Awaitick/App.xaml.cs`).
- **Singleton** for app-wide state, **Scoped** for per-window services and view models resolved from the `WindowShell` scope, **Transient** for editor-style view models.
- `ValidateScopes` and `ValidateOnBuild` are on in Development. `OnLaunched` is `async void`, so a validation failure is logged and the window stays blank rather than crashing. Check the log.
- Some services differ per head through `#if`. Check `ConfigureServices` before assuming an implementation.

## Navigation
- `INavigationService.Navigate<TViewModel>(parameter)`. Views are found by `RegisterViewsFromAssembly` in `WindowShell`.
- Pass parameters as typed models, such as `CountdownEditorViewModel.NavigationModel`.

## Messaging
- Use `IMessenger` (registered as `WeakReferenceMessenger.Default`) for cross-view-model communication. Message types live in `Awaitick.Core/Messages`.

## Data
- `IDataService` is implemented by `SqliteDataService`. The database lives under `ApplicationData.Current.LocalFolder`, which keeps it isolated per channel and worktree.
- `ICountdownsDataService` provides the higher-level countdown operations.

## Conditional compilation
- `WINDOWS` for the WinAppSDK head, `HAS_UNO` for every other head, plus `__ANDROID__`, `__IOS__` and `__WASM__`.
- `APP_CHANNEL_DEV` on Dev channel builds.

## Adding a page
1. Create `XViewModel : PageViewModel` in Core.
2. Create `XViewBase : PageBase<XViewModel>` and `XView : XViewBase` in `src/Awaitick/Views`, with the XAML rooted at the base.
3. Register the view model in `ConfigureServices`.

## Localization
- **Never hardcode user-facing strings.** Add each key to both `src/Awaitick/Strings/en-US/Resources.resw` and `src/Awaitick/Strings/cs-CZ/Resources.resw`.
- In XAML use `{ex:Localize Key=MyKey}` with `xmlns:ex="using:Awaitick.Extensions.Xaml"`. In code inject `IStringLocalizer`, or use `Localizer.Instance` where DI isn't available.
- `appsettings.json` lists the supported cultures. The first entry is the fallback for any device language that doesn't match, so keep `en-US` first and only list languages that have strings.
