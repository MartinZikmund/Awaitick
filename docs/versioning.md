# Versioning and release channels

`main` is where the next release is brewing, and `release/v{minor}` branches are where releases ship. Every build carries an `AppChannel`, baked into its package identity, so Dev and Prod can sit side by side on every device.

## Branches

| Branch | `version.json` | NBGV version | Channel |
|---|---|---|---|
| `main` | next planned minor | `0.2.{height}-dev` | Dev |
| `release/v0.1` | `0.1` | `0.1.{height}` | Prod |
| feature branches | inherited from `main` | prerelease | Dev |

`publicReleaseRefSpec` only matches `release/v{minor}`, so `main` can never produce a stable version by accident.

## What the channel changes

`AppChannel` is an MSBuild property that defaults to `Dev`. Packaging workflows set it from the branch.

| | Dev | Prod |
|---|---|---|
| Android and iOS id | `dev.mzikmund.awaitick.dev` | `dev.mzikmund.awaitick` |
| Windows package name | `dev.mzikmund.awaitick.dev` | `5143MartinZikmund.Awaitick` |
| Display name | `Awaitick Dev` | `Awaitick` |
| Android and iOS icon | Amber | Blue |
| In-app badge | `DEV` | None |
| Compile constant | `APP_CHANNEL_DEV` | None |

The Windows icon doesn't change, because the manifest points at hand-made PNGs in `Platforms/Windows`.

## Identity lives in the csproj, not the tracked files

The build only fills blanks, so a value hard-coded in a tracked file silently wins. `ChannelIdentityTests` guards this.

- **`Package.appxmanifest`** must keep `Identity/@Name` and the display names blank. If Visual Studio's *Associate App with the Store* writes them back, undo it.
- **The MSIX `Publisher` is the exception and stays in the manifest.** It has to, because the generator turns `ApplicationPublisher` into `O={value}`, which would mangle the Store's `CN=…` publisher.
- **`Info.plist`** must not set `CFBundleIdentifier`, `CFBundleName` or `CFBundleDisplayName`.

## Workflows

| Workflow | Runs on | Result |
|---|---|---|
| `ci.yml`, `validate-manifest-version.yml` | push to `main`, every PR | Checks only |
| `package-windows.yml` | push to `release/v*`, manual | Prod `.msixupload` to upload to Partner Center yourself |
| `package-android.yml` | push to `release/v*`, manual | Prod AAB uploaded to the Play **internal** track as a **draft** |
| `package-ios.yml` | push to `release/v*`, manual | Prod `.ipa` uploaded to TestFlight |
| `static-web-apps-deploy.yml` | push to `release/v*`, PRs, manual on `release/v*` | Production site from releases, previews from PRs |
| `tag-release.yml` | after all three packaging workflows pass on `release/v*` | Pushes tag `v{version}` |
| `prepare-release.yml` | manual | Cuts the release branch, starts its builds, opens the `main` bump PR |

A manual packaging run from any other branch builds the Dev channel and publishes nothing.

**`main` no longer deploys the website.** The production site only changes when a release goes out.

## Cutting a release

Run **Actions → Prepare Release**. It:

1. Creates `release/v{current}`.
2. Starts the packaging and website workflows on it. It has to start them explicitly, because a push made with `GITHUB_TOKEN` doesn't trigger workflows.
3. Opens a PR moving `main` to the next minor version. That PR doesn't get CI on its own for the same reason, so close and reopen it to run the checks.

To patch a release, commit the fix to `release/v{minor}`. NBGV bumps the patch and the same workflows run. If `main` needs the fix too, cherry-pick it.

For a major version bump, edit `version.json` on `main` by hand.

## Mobile version numbers

Nerdbank.GitVersioning sets these on Android and iOS by itself, so don't add `ApplicationVersion` or `ApplicationDisplayVersion` to the csproj. On Android the version code is `minor * 65536 + height`, plus a much larger major term, so it keeps rising across releases.

## Secrets

| Secret | Used by |
|---|---|
| `ANDROID_KEYSTORE_BASE64`, `ANDROID_KEY_ALIAS`, `ANDROID_KEY_PASSWORD`, `ANDROID_STORE_PASSWORD` | Android signing |
| `GOOGLE_PLAY_SERVICE_ACCOUNT_JSON` | Play upload |
| `APPLE_DISTRIBUTION_P12_BASE64`, `APPLE_P12_PASSWORD`, `APPLE_PROVISIONING_PROFILE_BASE64`, `APPLE_PROVISIONING_PROFILE_UUID`, `APPLE_CODESIGN_KEY`, `APPLE_TEAM_ID` | iOS signing |
| `APPSTORE_ISSUER_ID`, `APPSTORE_API_KEY_ID`, `APPSTORE_API_PRIVATE_KEY` | TestFlight upload |
| `AZURESTATICWEBAPPSDEPLOYMENTTOKEN` | Website |
