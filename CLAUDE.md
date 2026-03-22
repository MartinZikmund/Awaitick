# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Awaitick is a cross-platform event countdown application built with **Uno Platform** targeting Android, iOS, Windows, macOS, Desktop, and WebAssembly. It allows users to create and manage countdowns for important events with customizable backgrounds, notifications, and themes.

**Technology Stack:**
- Framework: Uno Platform with .NET 10.0
- UI: XAML with WinUI controls
- Architecture: MVVM using CommunityToolkit.Mvvm
- Data Storage: File-based JSON serialization
- Dependencies: Plugin.InAppBilling, CommunityToolkit.WinUI, MZikmund.Toolkit.WinUI

## Build and Development Commands

### Setup and Build
```bash
# Restore .NET workloads (required for Uno Platform)
dotnet workload restore Awaitick.slnx
dotnet workload install wasm-tools

# Build the main project (builds all target frameworks)
msbuild src/Awaitick/Awaitick.csproj /r

# Build solution
msbuild Awaitick.slnx /r
```

### Platform-Specific Development
- **Solution file**: `Awaitick.slnx`
- **Main project**: `src/Awaitick/Awaitick.csproj`
- **Core library**: `src/Awaitick.Core/Awaitick.Core.csproj`

The project uses **Central Package Management** with versions defined in `src/Directory.Packages.props`.

## Architecture and Code Organization

### Project Structure
```
src/
├── Awaitick/              # Main application with UI and platform-specific implementations
│   ├── Views/            # XAML pages (MainView, CountdownEditorView, etc.)
│   ├── Controls/         # Custom UI controls
│   ├── Platforms/        # Platform-specific entry points (Android, iOS, Desktop, WASM)
│   ├── Services/         # Platform-specific service implementations
│   ├── Converters/       # XAML value converters
│   ├── Business/         # App-specific business logic
│   └── WindowShell.xaml  # App shell with navigation frame
│
└── Awaitick.Core/         # Platform-agnostic shared library
    ├── ViewModels/       # All ViewModels (MainViewModel, CountdownEditorViewModel, etc.)
    ├── Services/         # Service interfaces and shared implementations
    ├── Models/           # Data models and entities
    ├── Infrastructure/   # Core infrastructure (IoC, IApplication, IWindowShell)
    ├── Messages/         # CommunityToolkit.Mvvm messages for MVVM communication
    └── Configuration/    # App configuration classes
```

### Key Architectural Patterns

**Dependency Injection:**
- All services and ViewModels are registered in `App.xaml.cs` (`ConfigureServices` method)
- Uses Microsoft.Extensions.DependencyInjection with scoped lifetimes per window
- `WindowShell` creates a scoped `IServiceProvider` for each window instance
- Static `IoC` class (`Awaitick.Core.Infrastructure.IoC`) provides service locator pattern for scenarios where DI isn't available

**MVVM Pattern:**
- ViewModels use `[ObservableProperty]` and `[RelayCommand]` source generators from CommunityToolkit.Mvvm
- All ViewModels inherit from `ViewModelBase` (which extends `ObservableRecipient`)
- Views are linked to ViewModels via navigation service registration
- `IMessenger` (WeakReferenceMessenger) used for cross-ViewModel communication (e.g., `CountdownDeletedMessage`)

**Navigation:**
- `INavigationService` handles type-based navigation (e.g., `Navigate<MainViewModel>()`)
- Views registered automatically via `RegisterViewsFromAssembly`
- Navigation parameters passed as typed models (e.g., `CountdownEditorViewModel.NavigationModel`)
- ViewModels can implement `ViewNavigatedTo(object? parameter)` lifecycle method

**Data Layer:**
- `IDataService` interface abstracts data operations
- `FileDataService` implementation stores events as JSON in `events.data`
- Uses `System.Text.Json` with source-generated serialization context (`EventCountdownSerializerContext`)
- Data initialized asynchronously during app startup (`Host.Services.GetRequiredService<IDataService>().InitializeAsync()`)
- `ICountdownsDataService` provides higher-level countdown-specific operations

**Platform Abstraction:**
- Service interfaces defined in `Awaitick.Core/Services/` subdirectories
- Platform implementations in `Awaitick/Services/` or platform-specific using conditional compilation
- Common platform abstractions:
  - `IFileService`: File I/O operations
  - `IScheduledNotificationService`: Local notifications
  - `ITileService`: Live tiles (Windows)
  - `IImagePickerService`: Image selection
  - `IThemeManager`: Theme switching
  - `IStoreService`: In-app purchases (with `FakeStoreService` for DEBUG, `ProStoreService` for HAS_UNO)

**Conditional Compilation:**
- `HAS_UNO`: Defined for non-Windows platforms (Android, iOS, WASM, etc.)
- Platform-specific: `__ANDROID__`, `__IOS__`, `__WASM__`, etc.
- Example in `App.xaml.cs:149-155` for store service registration

## MVVM Conventions

**ViewModel Implementation:**
```csharp
public partial class MyViewModel : ViewModelBase
{
    [ObservableProperty]
    private string title = "Default";  // Generates public Title property

    [RelayCommand]
    private async Task LoadDataAsync()  // Generates LoadDataCommand
    {
        // Implementation
    }
}
```

**Service Registration:**
```csharp
// In App.xaml.cs ConfigureServices():
services.AddScoped<MainViewModel>();           // ViewModels typically scoped
services.AddSingleton<IDataService, FileDataService>();  // Data services singleton
services.AddTransient<CountdownEditorViewModel>();       // Editor can be transient
```

**Navigation with Parameters:**
```csharp
// Define navigation model as nested class in ViewModel
public class NavigationModel
{
    public EditorMode Mode { get; set; }
}

// Navigate with typed parameter
_navigationService.Navigate<CountdownEditorViewModel>(new NavigationModel { Mode = EditorMode.Add });

// Handle in target ViewModel
public override void ViewNavigatedTo(object? parameter)
{
    var model = parameter as NavigationModel;
    // Use model
}
```

## Key Components and Services

**Window Shell (`WindowShell.xaml`):**
- Root page containing navigation Frame (`InnerFrame`)
- Creates scoped ServiceProvider per window
- Handles custom title bar and Mica backdrop on Windows
- Initializes theme via `IThemeManager`

**Main ViewModel (`MainViewModel`):**
- Entry point showing list of countdowns
- Uses `IMessenger` to receive `CountdownDeletedMessage` notifications
- Manages countdown collection as `ObservableCollection<CountdownViewModel>`
- Integrates with store service for Pro license checking

**Countdown Editor (`CountdownEditorViewModel`):**
- Supports Add/Edit modes via `EditorMode` enum
- Handles background image selection via `IImagePickerService`
- Updates scheduled notifications after saving

**Services Organization:**
- Each service type in its own subdirectory under `Services/`
- Interface file (e.g., `INavigationService.cs`) alongside implementation
- Platform-specific implementations in main `Awaitick` project
- Shared implementations in `Awaitick.Core`

## Testing

Currently no automated tests exist in the codebase. When adding tests:
- Target `Awaitick.Core` for unit testing ViewModels and services
- Use mocking frameworks for service dependencies
- Test cross-platform compatibility where applicable

## Development Notes

**Nullable Reference Types:** Enabled project-wide via `<Nullable>enable</Nullable>`

**Implicit Usings:** Enabled via `<ImplicitUsings>enable</ImplicitUsings>` with global usings in `GlobalUsings.cs`

**Hot Reload:** Supported via Uno Platform - use during development for faster iteration

**Uno Features:** Configured in `.csproj` files via `<UnoFeatures>` (Hosting, Mvvm, Configuration, Localization, Serialization, Toolkit, SkiaRenderer)

**WebAssembly:** Enables IndexedDB file system via `<WasmShellEnableIDBFS>true</WasmShellEnableIDBFS>`

**Localization:** Configured via `UseLocalization()` in app builder, resources in `Strings/` directory

**Source Generators:** Uses CommunityToolkit.Mvvm source generators - be aware of generated code when debugging (partial properties/commands)
