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

	private static string AppRoot => Path.Combine(FindRepoRoot(), "src", "Awaitick");

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

	private static string FindRepoRoot()
	{
		DirectoryInfo? directory = new(AppContext.BaseDirectory);
		while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "Awaitick.slnx")))
		{
			directory = directory.Parent;
		}

		return directory?.FullName ?? throw new DirectoryNotFoundException("Repository root (Awaitick.slnx) not found.");
	}
}
