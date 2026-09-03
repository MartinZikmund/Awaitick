# Contributing to Awaitick

Thanks for wanting to help. This page covers everything you need to get a change merged.

## Before you start

- **Small fix?** Just open a pull request.
- **Bigger change?** Open an issue first so we can agree on the approach before you spend
  time on it.
- Looking for somewhere to start? Try the
  [`good first issue`](https://github.com/MartinZikmund/Awaitick/labels/good%20first%20issue)
  label.

## The CLA

Contributions require a one-time [Contributor Licence Agreement](CLA.md). A bot comments on
your first pull request with instructions — signing takes one comment and it remembers you
afterwards.

**You keep the copyright in your work.** The CLA grants the licence breadth needed to keep
publishing Awaitick on app stores and to relicense if that ever becomes necessary. Please
read it rather than signing blind; [LICENSING.md](LICENSING.md) explains the reasoning.

## Building

You need the **.NET 10 SDK**.

```bash
dotnet workload restore Awaitick.slnx
dotnet workload install wasm-tools      # only for the WebAssembly target

msbuild src/Awaitick/Awaitick.csproj /r
```

Building every target needs the Android, iOS and WebAssembly workloads. To work on one
platform without installing all of them, narrow the target framework — this applies to
**restore as well as build**, which plain `-f` does not:

```bash
dotnet build src/Awaitick/Awaitick.csproj -p:TargetFrameworks=net10.0-desktop
```

| Target | TFM |
|---|---|
| Windows | `net10.0-windows10.0.26100` |
| Android | `net10.0-android` |
| iOS | `net10.0-ios` |
| Desktop | `net10.0-desktop` |
| WebAssembly | `net10.0-browserwasm` |

## Tests

There is **no automated test project yet**. CI runs a Debug build of every target as a smoke
test, so at minimum make sure your change builds.

Adding tests is welcome and useful. Put logic in **`Awaitick.Core`** where it can be covered
without a UI head, and target that project from any new test project.

## Where code goes

| Path | What belongs there |
|---|---|
| `src/Awaitick.Core` | View models, services, models — anything testable and head-independent |
| `src/Awaitick` | Views, platform-specific services, app composition (`App.xaml.cs`) |

Prefer `Awaitick.Core`. A view model belongs in the head only if it depends on a type defined
there.

A few conventions worth knowing:

- MVVM with **CommunityToolkit.Mvvm** — `[ObservableProperty]` partial properties and
  `[RelayCommand]`.
- Views come in pairs: a non-generic base closing the generic (`XViewBase : PageBase<XViewModel>`)
  and the page itself (`XView : XViewBase`). Views are discovered by naming convention, so a
  page ending in `View` needs no navigation-table entry — only a
  `services.AddTransient<XViewModel>()` line in `App.xaml.cs`.
- **Never hardcode user-facing text.** Add the key to **both**
  `src/Awaitick/Strings/en-US/Resources.resw` and `src/Awaitick/Strings/cs-CZ/Resources.resw`,
  then use `{ex:Localize Key=MyKey}` in XAML or `IStringLocalizer` in code.

## Adding or removing a dependency

Versions live only in `src/Directory.Packages.props` (Central Package Management) — add a
`<PackageVersion>` there and reference it with no `Version=` attribute.

Then update **both** [`THIRD-PARTY-NOTICES.md`](THIRD-PARTY-NOTICES.md) and
`src/Awaitick.Core/Models/Licensing/ThirdPartyNotices.cs`, which back the in-app
**Settings → About → Open-source licenses** screen. They are maintained by hand and drift
silently if only one is updated.

Anything under a licence incompatible with the AGPL — proprietary, "non-commercial", SSPL,
BUSL — cannot be added. Say so in the pull request if you are unsure.

## Branches and commits

Branch off `main`:

- `feature/<short-name>` for new work
- `fix/<short-name>` for bug fixes
- `chore/`, `docs/`, `ci/` for everything else

Commits follow [Conventional Commits](https://www.conventionalcommits.org/): `type: subject`
with type one of `feat | fix | chore | docs | refactor | test | ci | build`. A scope is
optional — use one when it sharpens the message (`feat(settings): ...`). Imperative mood, no
trailing period.

**Do not hand-edit version numbers.** Versions come from Nerdbank.GitVersioning via
`version.json` and git height; CI fails if `Package.appxmanifest` stops saying
`Version="0.0.0.0"`.

## Pull requests

- Keep them focused. One concern per PR reviews far faster than five.
- Say what changed and why. Screenshots help for anything visual.
- CI must be green, and the CLA bot must be satisfied.

## Code of conduct

Taking part means following our [Code of Conduct](CODE_OF_CONDUCT.md).

Found a security problem? **Do not open a public issue** — see [SECURITY.md](SECURITY.md).
