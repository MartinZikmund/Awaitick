# GitHub Copilot Instructions for Awaitick

## Project Overview
Awaitick is a cross-platform event countdown application built with Uno Platform. It allows users to create and manage countdowns for important events and dates.

## Technology Stack
- **Framework**: Uno Platform (cross-platform .NET UI framework)
- **Language**: C# with .NET 9.0
- **UI Technology**: XAML with WinUI controls
- **Architecture**: MVVM pattern with CommunityToolkit.Mvvm
- **Target Platforms**: Android, iOS, Windows, macOS, WebAssembly, Desktop
- **Build System**: MSBuild with Uno SDK

## Code Organization

### Project Structure
- `src/Awaitick/` - Main application project with UI and platform-specific code
- `src/Awaitick.Core/` - Shared business logic, view models, and services
- `src/Awaitick.Core/ViewModels/` - View models following MVVM pattern
- `src/Awaitick.Core/Services/` - Application services and business logic
- `src/Awaitick.Core/Models/` - Data models and entities
- `src/Awaitick/Views/` - XAML views and pages
- `src/Awaitick/Controls/` - Custom user controls
- `src/Awaitick/Services/` - Platform-specific service implementations

### Key Technologies Used
- **CommunityToolkit.Mvvm** - For MVVM implementation (ObservableObject, RelayCommand, etc.)
- **Microsoft.Extensions.DependencyInjection** - For dependency injection
- **CommunityToolkit.WinUI** - For additional WinUI controls and converters
- **Plugin.InAppBilling** - For in-app purchase functionality
- **MZikmund.Toolkit.WinUI** - Custom toolkit with additional helpers

## Coding Standards and Best Practices

### C# Conventions
- Use **nullable reference types** (enabled project-wide)
- Use **implicit usings** (enabled project-wide) 
- Follow **async/await** patterns for asynchronous operations
- Use **record types** for immutable data models when appropriate
- Prefer **expression-bodied members** for simple properties and methods

### MVVM Patterns
- **ViewModels** should inherit from `ObservableObject` (CommunityToolkit.Mvvm)
- Use `[ObservableProperty]` attribute for bindable properties
- Use `[RelayCommand]` attribute for commands
- Keep ViewModels **platform-agnostic** - place them in `Awaitick.Core`
- Views should **not contain business logic** - use ViewModels and data binding
- Use `IMessenger` for communication between ViewModels when needed

### Dependency Injection
- Register services in the `App.xaml.cs` during application startup
- Use **interface-based services** for testability and platform abstraction
- Follow **single responsibility principle** for service design
- Services should be registered with appropriate lifetimes (Singleton, Transient, etc.)

### XAML Conventions
- Use **data binding** instead of code-behind when possible
- Follow **WinUI/UWP naming conventions** for controls and resources
- Use **x:Bind** for compile-time binding when targeting Windows
- Use **StaticResource** for styles and templates
- Keep XAML files **clean and readable** with proper indentation

### Cross-Platform Considerations
- **Platform-specific code** should be isolated in the main `Awaitick` project
- **Shared business logic** belongs in `Awaitick.Core`
- Use **conditional compilation** (`#if WINDOWS`, `#if ANDROID`, etc.) when needed
- Test on **multiple target platforms** when making UI changes
- Be aware of **platform-specific limitations** and capabilities

## File Naming and Organization

### Naming Conventions
- **ViewModels**: End with `ViewModel` (e.g., `MainViewModel.cs`)
- **Views**: Use descriptive names matching ViewModels (e.g., `MainPage.xaml`)
- **Services**: End with `Service` (e.g., `CountdownService.cs`)
- **Models**: Use noun names representing entities (e.g., `Event.cs`, `Countdown.cs`)
- **Interfaces**: Prefix with `I` (e.g., `ICountdownService.cs`)

### File Organization
- Group related files in **logical folders**
- Keep **platform-specific implementations** separate from shared code
- Use **partial classes** for platform-specific view customizations
- Place **converters** in `Converters/` folder
- Place **custom controls** in `Controls/` folder

## Common Patterns and Examples

### ViewModel Implementation
```csharp
[ObservableObject]
public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private string title = "Awaitick";

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        // Implementation
    }
}
```

### Service Registration
```csharp
services.AddSingleton<ICountdownService, CountdownService>();
services.AddTransient<MainViewModel>();
```

### XAML Data Binding
```xml
<TextBlock Text="{Binding Title}" />
<Button Command="{Binding LoadDataCommand}" Content="Load" />
```

## Build and Development

### Build Commands
- **Restore workloads**: `dotnet workload restore src/Awaitick.slnx`
- **Build solution**: `msbuild src/Awaitick/Awaitick.csproj /r`
- **Build specific platform**: Use platform-specific commands in CI workflows

### Development Setup
- Use **Visual Studio 2022** or **Visual Studio Code** with C# extension
- Install **Uno Platform** extension for better XAML support
- Install required **.NET workloads** for target platforms
- Use **Hot Reload** for faster development iterations

### Platform-Specific Notes
- **Android**: Requires Android SDK and emulator setup
- **iOS**: Requires Xcode on macOS for deployment
- **Windows**: Native WinUI support with full feature set
- **WebAssembly**: Runs in browser with some API limitations

## Testing Approach
Currently, the project doesn't have automated tests. When adding tests:
- Create **unit tests** for ViewModels and Services in `Awaitick.Core`
- Use **dependency injection** and **mocking** for isolated testing
- Consider **UI tests** using appropriate frameworks for target platforms
- Test **cross-platform compatibility** when possible

## Performance Considerations
- Use **async/await** for I/O operations and long-running tasks
- Implement **proper disposal patterns** for resources
- Use **ObservableCollection** for data binding to collections
- Consider **virtualization** for large lists of items
- Be mindful of **memory usage** on mobile platforms

## Common Issues and Solutions
- **Platform differences**: Use conditional compilation or platform-specific services
- **XAML binding errors**: Check property names and data context
- **Async deadlocks**: Always use ConfigureAwait(false) in library code
- **Memory leaks**: Properly dispose of event subscriptions and resources
- **Build errors**: Ensure all workloads and SDKs are installed and up to date

## Additional Resources
- [Uno Platform Documentation](https://platform.uno/)
- [WinUI 3 Controls](https://docs.microsoft.com/en-us/windows/winui/api/)
- [CommunityToolkit.Mvvm](https://docs.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/)