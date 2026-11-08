# Community Toolkit controls (source copy)

`SettingsCard`, `SettingsExpander` and `ConstrainedBox`, plus the state triggers the settings templates use, copied from
[CommunityToolkit/Windows](https://github.com/CommunityToolkit/Windows) at tag `v8.2.251219`, because no published
build targets Uno 7 yet. MIT licensed, see [License.md](License.md).

The folder layout and namespaces match the package, so nothing outside this folder knows it is a copy except the path
in `App.xaml`. The only edits to the upstream files:

- `using` lines at the top of the C# files, which upstream's build tooling supplies as global usings
- the `ms-appx` paths in `SettingsControls/Themes/Generic.xaml` and `SettingsExpander.xaml`

The non-control helpers `Awaitick.Core` needs are copied separately, in `src/Awaitick.Core/ThirdParty/CommunityToolkit`.

## Going back to the packages

Once CommunityToolkit builds target Uno 7:

1. Delete this folder and `src/Awaitick.Core/ThirdParty/CommunityToolkit`.
2. In `src/Directory.Packages.props`, replace the comment pointing here with `PackageVersion` items for
   `CommunityToolkit.WinUI.Controls.SettingsControls`, `CommunityToolkit.WinUI.Controls.Primitives` and
   `CommunityToolkit.WinUI.Helpers`.
3. In `Awaitick.csproj`, add `PackageReference` items for the two `Controls` packages.
4. In `Awaitick.Core.csproj`, add a `PackageReference` for `CommunityToolkit.WinUI.Helpers`.
5. In `App.xaml`, remove the merged dictionary that points at this folder.
