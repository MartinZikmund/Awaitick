# AGENTS.md

Guidance for AI coding agents working in this repository.

Awaitick is a cross-platform event countdown app built with Uno Platform on .NET 10. It targets Android, iOS, Windows (WinAppSDK), Desktop (Skia) and WebAssembly from one project.

## Conventions

Detailed conventions live in **`.claude/rules/`**. Read them before writing code:

- `architecture.md`: Core and head split, MVVM, DI, navigation, messaging, localization, adding a page
- `code-style.md`: language, naming, Central Package Management
- `testing.md`: MSTest on Microsoft.Testing.Platform, run command, fakes
- `git.md`: commits, branches, versioning
- `docs.md`: feature docs go in `docs/<topic>.md`

## Build and test

```bash
dotnet tool restore                                    # once after cloning (XAML Styler)

dotnet build src/Awaitick/Awaitick.csproj -f net10.0-desktop
dotnet build src/Awaitick/Awaitick.csproj -f net10.0-android

dotnet test tests/Awaitick.Core.Tests/Awaitick.Core.Tests.csproj
```

**The Windows head needs MSBuild, not `dotnet build`.** `Awaitick.Core` contains XAML, so `dotnet build` fails with `UNOB0008`:

```powershell
$msbuild = & "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -latest -prerelease -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\MSBuild.exe'
& $msbuild src/Awaitick/Awaitick.csproj /r /p:TargetFramework=net10.0-windows10.0.26100
```

## Identity and releases

Builds default to the **Dev** channel, and a build from a linked git worktree also gets its own identity. So **don't hardcode the package id or window title**. Read `ApplicationId` from MSBuild.

Keep identity out of tracked files: `Package.appxmanifest` must not set `Identity/@Name` or display names, and `Info.plist` must not set `CFBundleIdentifier` or the bundle names. See [docs/versioning.md](docs/versioning.md) and [docs/worktree-identity.md](docs/worktree-identity.md).

## Running the app

- **Windows head:** use the **`/run-winui-app`** skill to build, launch packaged, automate and clean up. Read it first.
- **Desktop, WebAssembly and mobile heads:** use the **`uno-app` MCP** (`uno_app_select_solution`, then `uno_discover_tools` and `uno_execute_tool`). It doesn't drive the WinAppSDK head.

See a UI change render before reporting it done.

## Skills and resources

- **Windows and WinUI:** the `winui:*` skills. Uno's API mirrors WinUI, so that guidance applies almost verbatim.
- **UI and design:** `/winui-design` is the primary resource for layout, styling, theming and Fluent design.
- **.NET:** the `dotnet-*` and `dotnet-test:*` skills.
- **API docs:** the Microsoft Learn MCP for .NET and Windows, the Uno docs MCP for Uno specifics.
- **Web output:** the Playwright MCP or the Chrome integration.

## Tech choices to avoid

This app is plain WinUI XAML with CommunityToolkit.Mvvm by design. **Don't introduce Uno.Extensions Navigation, C# Markup or MVUX.** Prefer a WinUI or CommunityToolkit option over adding new dependencies on Uno.Extensions, Uno Toolkit or Uno Themes. The features already in `UnoFeatures` are fine to keep.

## Working style

- **TDD.** Write a failing test first, watch it fail, then implement. Push logic into `Awaitick.Core` so it's testable without a UI head.
- **Use modern C#** where it makes the code clearer.
- **Keep comments lean.** Comment the why in a line or two.
- **XAML formatting is enforced by CI.** Run `dotnet xstyler -c Settings.XamlStyler -f <file>` on XAML you change, or comment `/apply-xaml-style` on the PR.
- **Report Uno divergences.** WinUI is the reference behaviour. If something is buggy or behaves differently on an Uno target, point it out and offer to file an issue at [`unoplatform/uno`](https://github.com/unoplatform/uno) with a minimal repro. Confirm with the user first.
