using System.Text.Json;

namespace Awaitick.Core.Tests.Packaging;

[TestClass]
public class LocalizationConfigurationTests
{
	[TestMethod]
	public void Cultures_MatchTheLocalizedStringFolders()
	{
		var cultures = ReadCultures().Order().ToArray();
		var folders = Directory.GetDirectories(Path.Combine(AppRoot, "Strings"))
			.Select(folder => Path.GetFileName(folder)!)
			.Order()
			.ToArray();

		CollectionAssert.AreEqual(folders, cultures);
	}

	[TestMethod]
	public void Cultures_StartWithEnglishAsTheFallback() =>
		Assert.AreEqual("en-US", ReadCultures().First());

	private static string AppRoot => RepoPaths.AppProject;

	private static IEnumerable<string> ReadCultures()
	{
		using JsonDocument appSettings = JsonDocument.Parse(File.ReadAllText(Path.Combine(AppRoot, "appsettings.json")));

		return appSettings.RootElement
			.GetProperty("LocalizationConfiguration")
			.GetProperty("Cultures")
			.EnumerateArray()
			.Select(culture => culture.GetString()!)
			.ToArray();
	}
}
