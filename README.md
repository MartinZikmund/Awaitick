# Awaitick

**Countdown to the events that matter.** Awaitick is a cross-platform countdown app built
with [Uno Platform](https://platform.uno/) — one C# codebase running on Windows, Android,
iOS, the desktop and the web.

[![License: AGPL v3](https://img.shields.io/badge/License-AGPL%20v3-blue.svg)](LICENSE)
[![CI](https://github.com/MartinZikmund/Awaitick/actions/workflows/ci.yml/badge.svg)](https://github.com/MartinZikmund/Awaitick/actions/workflows/ci.yml)
[![PRs welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg)](CONTRIBUTING.md)

## Platforms

| Target | TFM |
|---|---|
| Windows (WinAppSDK) | `net10.0-windows10.0.26100` |
| Android | `net10.0-android` |
| iOS | `net10.0-ios` |
| Desktop (Skia) | `net10.0-desktop` |
| Web (WebAssembly) | `net10.0-browserwasm` |

## Building

Requires the .NET 10 SDK.

```bash
dotnet workload restore Awaitick.slnx
dotnet workload install wasm-tools      # only needed for the WebAssembly target

# Build every target
msbuild src/Awaitick/Awaitick.csproj /r
```

To work on a single platform without installing every workload, narrow the target framework
for both restore and build:

```bash
dotnet build src/Awaitick/Awaitick.csproj -p:TargetFrameworks=net10.0-desktop
```

## Project layout

| Path | What it is |
|---|---|
| `src/Awaitick` | The app head — views, platform services, app composition |
| `src/Awaitick.Core` | Shared library — view models, services, models |

## Contributing

Contributions are welcome. Read [CONTRIBUTING.md](CONTRIBUTING.md) first — it covers the
build, the branch and commit conventions, and the one-time
[CLA](CLA.md) signature a bot will ask for on your first pull request.

Everyone taking part is expected to follow the
[Code of Conduct](CODE_OF_CONDUCT.md). To report a security issue, see
[SECURITY.md](SECURITY.md) — please do not open a public issue for those.

## License

Awaitick is free software, licensed under the
**[GNU Affero General Public License v3.0 or later](LICENSE)**. You may use, study, modify
and share it; if you share a modified version, or run one as a network service, its users
get the same rights — including the source.

Three things worth knowing:

- **App Store exception.** Apple's terms conflict with AGPLv3 sections 6 and 10, so
  [`COPYING.iOS`](COPYING.iOS) grants an additional permission under section 7 allowing
  distribution through the Apple App Store and Mac App Store — for anyone, not just the
  official builds.
- **The brand is not covered.** The Awaitick name, logo and icon are reserved. Forks must
  rebrand — see [TRADEMARKS.md](TRADEMARKS.md).
- **Contributions need a CLA.** One comment, once. See [CLA.md](CLA.md).

[LICENSING.md](LICENSING.md) explains all of this in plain English, and
[THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md) lists the third-party code and assets
Awaitick ships.

Copyright (c) 2020-present Martin Zikmund.
