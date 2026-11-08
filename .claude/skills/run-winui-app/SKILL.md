---
name: run-winui-app
description: >-
  Build, launch and UI-automate the Windows (WinAppSDK) head of Awaitick from the command line, with
  no Visual Studio UI. Use whenever the user wants to run, launch or screenshot the Windows app, see
  a change working on Windows, verify a fix there, or drive the UI (click buttons, flip toggles,
  inspect elements). Trigger even for "run the app on Windows" or "click Settings in the app".
---

# Run and automate the Windows app

Builds the WinAppSDK head, launches it **fully packaged with package identity**, and drives it with the Windows App Development CLI (`winapp`). Run everything from PowerShell, from the checkout you mean, because identity and paths vary per worktree.

Requires `winapp` (`winget install Microsoft.WinAppCli`).

## 1. Build

`dotnet build` fails for this head with `UNOB0008`, because `Awaitick.Core` contains XAML. Use MSBuild:

```powershell
$msbuild = & "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -latest -prerelease -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\MSBuild.exe'
& $msbuild src/Awaitick/Awaitick.csproj /r /v:quiet /p:TargetFramework=net10.0-windows10.0.26100 /p:Configuration=Debug
```

If it hangs with no output, set `$env:MSBUILDDISABLENODEREUSE = '1'` and retry.

## 2. Launch

```powershell
$out = Join-Path (Get-Location) "src\Awaitick\bin\Debug\net10.0-windows10.0.26100"
$app = winapp run $out --exe Awaitick.exe --detach --json | ConvertFrom-Json
$AppPid = $app.ProcessId
```

Don't background the app with `Start-Job`, because jobs don't survive across tool calls.

## 3. Automate

Always target the **PID**. Titles and identities vary per worktree.

```powershell
winapp ui inspect -a $AppPid -i              # interactive elements and their slugs
winapp ui search "Keep" -a $AppPid           # find by text
winapp ui invoke "btn-settings-a989" -a $AppPid
winapp ui screenshot -a $AppPid --output .screenshots\app.png
```

Work in a loop: inspect, act, verify. Prefer slugs over text, and re-inspect after navigation, because slugs carry a hash that goes stale. `.screenshots/` is git-ignored. Read the PNG back to actually look at it.

Things that work differently from what you'd guess:

- A `ToggleSwitch` shows up as a `Button` with `[on]` or `[off]`. `invoke` flips it, so re-run `search` to read the new state.
- `inspect` depth is limited, so add `--depth 14` if an element seems missing.

## Getting past first launch

A fresh identity starts on onboarding, and each channel and worktree has its own data. Invoke these in order, re-inspecting for slugs:

1. **Get started**
2. **Continue**
3. **Skip** (opens the new countdown editor)
4. **Cancel** (lands on the main page, where **Settings** is in the bottom bar)

## 4. Clean up

```powershell
Stop-Process -Id $AppPid -Force -ErrorAction SilentlyContinue
winapp unregister --manifest "$out\AppxManifest.xml"
```

Stop the PID, not the name: `Get-Process Awaitick` matches every worktree. Unregistering deletes that identity's app data.

## See also

[docs/worktree-identity.md](../../../docs/worktree-identity.md) explains why the id, name and badge vary.
