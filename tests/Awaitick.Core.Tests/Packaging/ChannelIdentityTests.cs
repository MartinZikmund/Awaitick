using System.Xml.Linq;

namespace Awaitick.Core.Tests.Packaging;

/// <summary>
/// The build fills identity per AppChannel, but only where the tracked files leave it blank. A value
/// hard-coded here, as Visual Studio's "Associate App with the Store" writes, silently wins.
/// </summary>
[TestClass]
public class ChannelIdentityTests
{
	private static readonly XNamespace _manifest = "http://schemas.microsoft.com/appx/manifest/foundation/windows10";
	private static readonly XNamespace _uap = "http://schemas.microsoft.com/appx/manifest/uap/windows10";

	[TestMethod]
	public void WindowsManifest_DoesNotHardcodeThePackageName() =>
		Assert.IsNull(LoadManifestRoot().Element(_manifest + "Identity")!.Attribute("Name"));

	[TestMethod]
	public void WindowsManifest_KeepsTheStorePublisher() =>
		Assert.AreEqual(
			"CN=5EDFBB6C-AD5D-4B76-B479-1C5F44017714",
			LoadManifestRoot().Element(_manifest + "Identity")!.Attribute("Publisher")!.Value);

	[TestMethod]
	public void WindowsManifest_DoesNotHardcodeTheDisplayName()
	{
		var root = LoadManifestRoot();

		Assert.IsNull(root.Element(_manifest + "Properties")!.Element(_manifest + "DisplayName"));
		Assert.IsNull(root.Descendants(_uap + "VisualElements").Single().Attribute("DisplayName"));
	}

	[TestMethod]
	public void IosInfoPlist_DoesNotHardcodeIdentityOrName()
	{
		var keys = XDocument.Load(Path.Combine(RepoPaths.AppProject, "Platforms", "iOS", "Info.plist"))
			.Descendants("key")
			.Select(key => key.Value)
			.ToArray();

		CollectionAssert.DoesNotContain(keys, "CFBundleIdentifier");
		CollectionAssert.DoesNotContain(keys, "CFBundleName");
		CollectionAssert.DoesNotContain(keys, "CFBundleDisplayName");
	}

	private static XElement LoadManifestRoot() =>
		XDocument.Load(Path.Combine(RepoPaths.AppProject, "Package.appxmanifest")).Root!;
}
