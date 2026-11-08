# Worktree-scoped app identity

You're working on two branches in two git worktrees and want to run both apps at once to compare them. Without help you can't: both builds declare the same package identity, so installing the second replaces the first, and they share one app-data folder.

A Dev build made from a **linked git worktree** therefore gets its own identity automatically:

```
D:\Personal\Awaitick                                  -> dev.mzikmund.awaitick.dev
D:\Personal\Awaitick\.claude\worktrees\template-sync  -> dev.mzikmund.awaitick.dev.wttemplate289155
```

Both install and run, and neither can see the other's data.

## What you see

| Where | Main checkout | Worktree |
|---|---|---|
| Start menu, Settings → Apps | `Awaitick Dev` | `Awaitick Dev (template-sync)` |
| In-app badge | `DEV` | `DEV · template-sync` |
| Android launcher | `Awaitick Dev` | `Awaitick Dev [TS]` |
| iOS home screen | `Awaitick Dev` | `Awaitick Dev (template-sync)` |

## How it works

[`src/WorktreeIdentity.props`](../src/WorktreeIdentity.props) detects the worktree with filesystem reads only, because `ApplicationId` is chosen during MSBuild evaluation, where `Exec` can't run.

In a linked worktree `.git` is a file pointing under `<common>/.git/worktrees/<name>`. A submodule's `.git` is also a file, but it points under `modules/`, so it's ignored.

The identifier is that git admin-directory name, not the folder name or the branch:

- Git guarantees it's unique.
- It survives a detached HEAD.
- It doesn't change when you switch branches, which would otherwise orphan the install you were using.

### Derived values

| Value | Rule | Used by |
|---|---|---|
| Id segment | `wt` + 8 padded alphanumerics + 6 hex, always 16 characters | `ApplicationId` |
| Long tag | Name without a `worktree-` or `wt-` prefix, capped at 20 | Windows name, iOS name, badge |
| Short tag | Initials of 2+ segments, otherwise the first 4 characters | Android label |
| Dev port | `5001 + (hash % 999)` | WebAssembly dev server |

## Turning it off

| You want | Do this |
|---|---|
| The normal Dev identity from inside a worktree | `-p:EnableWorktreeIdentity=false` |
| A specific name | `-p:AppWorktreeName=whatever` |
| A release build | Nothing, because `AppChannel=Prod` is never suffixed |

`AppWorktreeName` can't switch the feature on where it's off. The channel, CI and kill-switch checks all sit in one gate after detection.

## Guarantees

[`scripts/verify-worktree-identity.ps1`](../scripts/verify-worktree-identity.ps1) asserts these, and CI runs it from a normal clone. Run it by hand from a worktree after touching either MSBuild file:

```powershell
pwsh scripts/verify-worktree-identity.ps1
```

| # | Guarantee |
|---|---|
| I1 | `AppChannel=Prod` is never suffixed, including the Windows Store identity |
| I2 | The main checkout gets the plain Dev identity |
| I3 | CI builds are never suffixed |
| I5 | `ApplicationId` stays within 50 characters, letter-first, `[a-z0-9.]` only (MSIX `Identity/@Name`) |
| I6 | `ApplicationTitle` stays within 40 characters (MSIX `uap:DefaultTile/@ShortName`) |

## Per platform

| Head | Isolated by | Notes |
|---|---|---|
| Windows | Package family name | Fully isolated. Keep `Identity/@Name` blank in `Package.appxmanifest` or the suffix is silently cancelled |
| Desktop | App-data folder, derived from `ApplicationId` | |
| Android | `package=` | The label comes from `Strings.xml`, so the build rewrites its staged copy |
| iOS | `CFBundleIdentifier` | **Simulator only.** A per-worktree id matches no provisioning profile. Not verified, because it needs a Mac |
| WebAssembly | Browser origin only | Building prints the port to use: `dotnet run -f net10.0-browserwasm --urls http://localhost:{port}` |

## Limitations

- **`git worktree move` changes the identity**, orphaning the old install and its data. Uninstall first.
- **Icons are the same across worktrees.** Only the channel changes the icon.
- **`Get-Process Awaitick` matches every worktree.** Stop apps by PID.
- **A local `-p:AppChannel=Prod` build from a worktree overwrites your real Prod install.**
- **Stray installs accumulate.** Find them with `Get-AppxPackage dev.mzikmund.awaitick.dev.wt*`.

## If you add features later

Machine-wide registrations aren't covered by `ApplicationId`. `Package.appxmanifest` has no `<Extensions>` today. The moment one appears, such as a protocol handler, a file association or a toast COM activator, worktrees will fight over it and it'll need separate handling.

See also [versioning.md](./versioning.md) for the channel model this builds on.
