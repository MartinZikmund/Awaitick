# Third-party notices

Awaitick is licensed under the [GNU AGPLv3](LICENSE). It also includes work by other people,
listed here with their licences.

The same information is shown inside the app under **Settings → About → Open-source
licenses**, embedded in the binary — it is never fetched over the network.

Generated from `dotnet list package --include-transitive` across the `net10.0-desktop`,
`net10.0-windows10.0.26100`, `net10.0-android` and `net10.0-ios` targets, with licences read
from each package's `.nuspec`. `net10.0-browserwasm` was not enumerated locally (the
`wasm-tools` workload was not installed); it draws on the same package set. **When you add
or remove a dependency, update this file and
`src/Awaitick.Core/Models/Licensing/ThirdPartyNotices.cs` together.**

---

## Bundled assets

### Fonts

**Inter** (`Inter-Light.ttf`, version 4.001) — Copyright 2016 The Inter Project Authors,
Rasmus Andersson. Licensed under the **SIL Open Font License 1.1**
(<https://scripts.sil.org/OFL>). "Inter" is a trademark of Rasmus Andersson; the OFL covers
the font software, not the name.

### Event background images

Nine of the bundled event backgrounds come from
[PublicDomainPictures.net](https://www.publicdomainpictures.net/) and are in the **public
domain**. Per-file source URLs are in
[`src/Awaitick/Assets/EventBackgrounds/Attributions.txt`](src/Awaitick/Assets/EventBackgrounds/Attributions.txt):

`Beach.jpg`, `Christmas.jpg`, `Concert.jpg`, `Easter.jpg`, `Halloween.jpg`, `Love.jpg`,
`Movies.jpg`, `NewYear.jpg`, `Plane.jpg`

The remaining sixteen backgrounds carry no attribution — see
[Needs a decision](#needs-a-decision).

---

## NuGet packages shipped in the app

### MIT

CommunityToolkit.Common,CommunityToolkit.Mvvm 
CommunityToolkit.WinUI.Controls.Primitives,CommunityToolkit.WinUI.Controls.SettingsControls 
CommunityToolkit.WinUI.Converters,CommunityToolkit.WinUI.Extensions 
CommunityToolkit.WinUI.Helpers,CommunityToolkit.WinUI.Triggers 
HarfBuzzSharp,HarfBuzzSharp.NativeAssets.Android 
HarfBuzzSharp.NativeAssets.Linux,HarfBuzzSharp.NativeAssets.Win32 
HarfBuzzSharp.NativeAssets.iOS,HarfBuzzSharp.NativeAssets.macOS 
Microsoft.Bcl.AsyncInterfaces,Microsoft.Extensions.Configuration 
Microsoft.Extensions.Configuration.Abstractions,Microsoft.Extensions.Configuration.Binder 
Microsoft.Extensions.Configuration.CommandLine,Microsoft.Extensions.Configuration.Environmen
tVariables 
Microsoft.Extensions.Configuration.FileExtensions,Microsoft.Extensions.Configuration.Json 
Microsoft.Extensions.Configuration.UserSecrets,Microsoft.Extensions.DependencyInjection 
Microsoft.Extensions.DependencyInjection.Abstractions,Microsoft.Extensions.Diagnostics 
Microsoft.Extensions.Diagnostics.Abstractions,Microsoft.Extensions.FileProviders.Abstraction
s Microsoft.Extensions.FileProviders.Physical,Microsoft.Extensions.FileSystemGlobbing 
Microsoft.Extensions.Hosting,Microsoft.Extensions.Hosting.Abstractions 
Microsoft.Extensions.Http,Microsoft.Extensions.Localization.Abstractions 
Microsoft.Extensions.Logging,Microsoft.Extensions.Logging.Abstractions 
Microsoft.Extensions.Logging.Configuration,Microsoft.Extensions.Logging.Console 
Microsoft.Extensions.Logging.Debug,Microsoft.Extensions.Logging.EventLog 
Microsoft.Extensions.Logging.EventSource,Microsoft.Extensions.Options 
Microsoft.Extensions.Options.ConfigurationExtensions,Microsoft.Extensions.Primitives 
Microsoft.NET.ILLink.Tasks,Microsoft.Windows.CsWin32 
Microsoft.Xaml.Behaviors.WinUI.Managed,Newtonsoft.Json Refit,SkiaSharp 
SkiaSharp.NativeAssets.Android,SkiaSharp.NativeAssets.Linux 
SkiaSharp.NativeAssets.WebAssembly,SkiaSharp.NativeAssets.Win32 
SkiaSharp.NativeAssets.WinUI,SkiaSharp.NativeAssets.iOS 
SkiaSharp.NativeAssets.macOS,SkiaSharp.Resources SkiaSharp.SceneGraph,SkiaSharp.Skottie 
SkiaSharp.Views,SkiaSharp.Views.Uno.WinUI SkiaSharp.Views.WinUI,System.Diagnostics.EventLog 
System.Json,System.Runtime.InteropServices.NFloat.Internal Tmds.DBus.Protocol

### Apache-2.0

SQLitePCLRaw.config.e_sqlite3,SQLitePCLRaw.core 
SQLitePCLRaw.provider.e_sqlite3,SQLitePCLRaw.provider.internal 
Uno.Core.Extensions.Collections,Uno.Core.Extensions.Disposables 
Uno.Core.Extensions.Equality,Uno.Core.Extensions.Logging 
Uno.Core.Extensions.Logging.Singleton,Uno.Extensions.Configuration 
Uno.Extensions.Core,Uno.Extensions.Core.WinUI 
Uno.Extensions.Hosting,Uno.Extensions.Hosting.WinUI 
Uno.Extensions.Http,Uno.Extensions.Localization 
Uno.Extensions.Localization.WinUI,Uno.Extensions.Logging.OSLog 
Uno.Extensions.Serialization,Uno.Extensions.Serialization.Http 
Uno.Extensions.Serialization.Refit,Uno.Extensions.Storage 
Uno.Extensions.Storage.WinUI,Uno.Fonts.Fluent Uno.Fonts.OpenSans,Uno.Foundation 
Uno.Foundation.Logging,Uno.Themes.WinUI Uno.Toolkit,Uno.Toolkit.WinUI 
Uno.UI.Adapter.Microsoft.Extensions.Logging,Uno.WinRT 
Uno.WinUI,Uno.WinUI.DevServer.Messaging Uno.WinUI.Graphics2DSK,Uno.WinUI.Lottie 
Uno.WinUI.Runtime.Skia,Uno.WinUI.Runtime.Skia.Android 
Uno.WinUI.Runtime.Skia.AppleUIKit,Uno.WinUI.Runtime.Skia.Linux.FrameBuffer 
Uno.WinUI.Runtime.Skia.MacOS,Uno.WinUI.Runtime.Skia.Win32 
Uno.WinUI.Runtime.Skia.Wpf,Uno.WinUI.Runtime.Skia.X11

### MIT AND Apache-2.0

Android support-library bindings: the C# binding code is MIT, the bound AndroidX library is Apache-2.0.

Xamarin.AndroidX.Activity,Xamarin.AndroidX.Annotation 
Xamarin.AndroidX.Annotation.Experimental,Xamarin.AndroidX.Annotation.Jvm 
Xamarin.AndroidX.AppCompat,Xamarin.AndroidX.AppCompat.AppCompatResources 
Xamarin.AndroidX.Arch.Core.Common,Xamarin.AndroidX.Arch.Core.Runtime 
Xamarin.AndroidX.AsyncLayoutInflater,Xamarin.AndroidX.Browser 
Xamarin.AndroidX.CardView,Xamarin.AndroidX.Collection 
Xamarin.AndroidX.Collection.Jvm,Xamarin.AndroidX.Concurrent.Futures 
Xamarin.AndroidX.ConstraintLayout,Xamarin.AndroidX.ConstraintLayout.Core 
Xamarin.AndroidX.CoordinatorLayout,Xamarin.AndroidX.Core 
Xamarin.AndroidX.Core.Core.Ktx,Xamarin.AndroidX.Core.SplashScreen 
Xamarin.AndroidX.Core.ViewTree,Xamarin.AndroidX.CursorAdapter 
Xamarin.AndroidX.CustomView,Xamarin.AndroidX.CustomView.PoolingContainer 
Xamarin.AndroidX.DocumentFile,Xamarin.AndroidX.DrawerLayout 
Xamarin.AndroidX.DynamicAnimation,Xamarin.AndroidX.Emoji2 
Xamarin.AndroidX.Emoji2.ViewsHelper,Xamarin.AndroidX.Fragment 
Xamarin.AndroidX.Interpolator,Xamarin.AndroidX.Legacy.Support.Core.UI 
Xamarin.AndroidX.Legacy.Support.Core.Utils,Xamarin.AndroidX.Legacy.Support.V4 
Xamarin.AndroidX.Lifecycle.Common,Xamarin.AndroidX.Lifecycle.Common.Jvm 
Xamarin.AndroidX.Lifecycle.LiveData.Core,Xamarin.AndroidX.Lifecycle.Process 
Xamarin.AndroidX.Lifecycle.Runtime,Xamarin.AndroidX.Lifecycle.Runtime.Android 
Xamarin.AndroidX.Lifecycle.ViewModel,Xamarin.AndroidX.Lifecycle.ViewModel.Android 
Xamarin.AndroidX.Lifecycle.ViewModelSavedState,Xamarin.AndroidX.Lifecycle.ViewModelSavedStat
e.Android Xamarin.AndroidX.Loader,Xamarin.AndroidX.LocalBroadcastManager 
Xamarin.AndroidX.Media,Xamarin.AndroidX.Print 
Xamarin.AndroidX.ProfileInstaller.ProfileInstaller,Xamarin.AndroidX.RecyclerView 
Xamarin.AndroidX.ResourceInspection.Annotation,Xamarin.AndroidX.SavedState 
Xamarin.AndroidX.SavedState.SavedState.Android,Xamarin.AndroidX.SlidingPaneLayout 
Xamarin.AndroidX.Startup.StartupRuntime,Xamarin.AndroidX.SwipeRefreshLayout 
Xamarin.AndroidX.Tracing.Tracing,Xamarin.AndroidX.Tracing.Tracing.Android 
Xamarin.AndroidX.Transition,Xamarin.AndroidX.VectorDrawable 
Xamarin.AndroidX.VectorDrawable.Animated,Xamarin.AndroidX.VersionedParcelable 
Xamarin.AndroidX.ViewPager,Xamarin.AndroidX.ViewPager2 
Xamarin.AndroidX.Window,Xamarin.AndroidX.Window.Extensions.Core.Core 
Xamarin.Google.Android.Material,Xamarin.Google.ErrorProne.Annotations 
Xamarin.Google.Guava.ListenableFuture,Xamarin.JSpecify 
Xamarin.Jetbrains.Annotations,Xamarin.Kotlin.StdLib 
Xamarin.KotlinX.AtomicFU,Xamarin.KotlinX.AtomicFU.Jvm 
Xamarin.KotlinX.Coroutines.Android,Xamarin.KotlinX.Coroutines.Core 
Xamarin.KotlinX.Coroutines.Core.Jvm,Xamarin.KotlinX.Serialization.Core 
Xamarin.KotlinX.Serialization.Core.Jvm

### Individually licensed

| Package | Licence | Notes |
|---|---|---|
| `MZikmund.Toolkit.WinUI` | MIT | Copyright (c) 2023 Martin Zikmund |
| `sqlite-net-e` | MIT | Copyright (c) Krueger Systems, Inc. |
| `SourceGear.sqlite3` | Public domain | SQLite itself is public domain — <https://sqlite.org/copyright.html> |
| `LibVLCSharp` | LGPL-2.1-or-later | Desktop/Android media support. "or later" permits LGPL-3.0, which is AGPLv3-compatible. |
| `Microsoft.WindowsAppSDK` | Microsoft Software License Terms | Windows target only. The Windows App SDK runtime — see [Needs a decision](#needs-a-decision). |
| `Microsoft.Web.WebView2` | Microsoft redistribution licence (BSD-3-Clause-style) | Windows target only. |
| `Uno.Diagnostics.Eventing`, `Uno.Fonts.Roboto`, `Uno.Wasm.WebSockets`, `Uno.icu-ios`, `Uno.icu-macos`, `Uno.icu-win`, `Uno.Microsoft.Xaml.Behaviors.WinUI.Managed`, `Uno.Microsoft.Xaml.Behaviors.Interactivity.WinUI` | Not declared in package | Uno Platform ships its components under Apache-2.0, but these `.nuspec` files carry no `<license>` element. See [Needs a decision](#needs-a-decision). |
| `CommonServiceLocator` | **MS-PL** | Pulled in transitively by `Uno.Core.Extensions.Logging.Singleton`. See [Needs a decision](#needs-a-decision). |

---

## Build and development tooling — not redistributed

These packages are used to build and debug Awaitick and are **not** part of any shipped
build, so their licences do not travel with the app. Each is marked either
`PrivateAssets="all"` or `Exclude="all" IncludeAssets="None"` for release builds (verified in
`Uno.Sdk` 6.6.0-dev.216 `targets/Uno.Implicit.Packages.ProjectSystem.targets`, and in
`src/Directory.Build.props` for Nerdbank.GitVersioning):

| Package | Licence | Excluded by |
|---|---|---|
| `Uno.Sdk.Extras` | Uno Platform EULA (proprietary) | `PrivateAssets="all"` |
| `Uno.Resizetizer` | Apache-2.0 | `PrivateAssets="all"` |
| `Uno.Settings.DevServer` | Not declared | `PrivateAssets="all"` |
| `Uno.WinUI.DevServer` | Apache-2.0 | `Exclude="all"` when `Optimize=true` |
| `Uno.UI.HotDesign` | Not declared | `Exclude="all"` when `Optimize=true` |
| `Uno.UI.App.Mcp` | Not declared | `Exclude="all"` when `Optimize=true` |
| `Nerdbank.GitVersioning` | MIT | `PrivateAssets="All"` |
| `Microsoft.Windows.SDK.BuildTools`, `Microsoft.Windows.SDK.Win32Docs` | Windows SDK licence | Build-time tooling |
| `Microsoft.Windows.SDK.Win32Metadata`, `Microsoft.Windows.WDK.Win32Metadata` | Windows SDK licence | Reference metadata |
| `Tmds.DBus.Generator` | Not declared (upstream Tmds.DBus is MIT) | Source generator |

---

## Needs a decision

Four items could not be cleared automatically. **Nothing has been removed or changed** —
these are for Martin to decide.

### 1. `CommonServiceLocator` is MS-PL, and MS-PL is GPL-incompatible

The Free Software Foundation classifies the Microsoft Public License as a free software
licence that is **incompatible with the GNU GPL** (and therefore with the AGPL). This package
**is** shipped: it arrives transitively through `Uno.Core.Extensions.Logging.Singleton`, which
comes from Uno Platform itself.

This is not something the app can simply drop — it is upstream. Options: confirm the actual
licence at <https://github.com/unitycontainer/commonservicelocator> (the `.nuspec` only
carries the deprecated `licenseUrl` field, which may be stale), or raise it with Uno Platform.

### 2. Sixteen background images have no attribution

These ship in the app with no recorded source or licence:

`AllSaintsDay.jpg`, `AprilFoolsDay.jpg`, `AustraliaDay.jpg`, `BastilleDay.jpg`,
`BrazilIndependenceDay.jpg`, `CanadaDay.jpg`, `EarthDay.jpg`, `Easter2.jpg`, `Halloween2.jpg`,
`IndependenceDayUSA.jpg`, `IndianIndependenceDay.jpg`, `InternationalWomensDay.jpg`,
`NewYear2.jpg`, `NewYearsEve.jpg`, `SouthAfricaFreedomDay.jpg`, `ValentinesDay.jpg`

If they are your own work, say so here and the question closes. If they came from somewhere
else, they need a source and a licence — publishing the repository makes this visible in a way
a closed-source app never did.

### 3. Eight Uno packages declare no licence

Listed in the table above. Uno Platform is Apache-2.0 across the board and these are almost
certainly fine, but the packages themselves carry no `<license>` element, so this is an
assumption rather than a verified fact. Worth reporting upstream so the metadata gets fixed.

### 4. `Microsoft.WindowsAppSDK` ships under a proprietary Microsoft EULA

Every AGPL application on Windows faces this. The usual reading is that the Windows App SDK
is a "System Library" under AGPLv3 section 1 — a major component of the operating system it
runs on — which the licence explicitly excludes from the Corresponding Source requirement.
Flagged for completeness rather than as a live problem.
