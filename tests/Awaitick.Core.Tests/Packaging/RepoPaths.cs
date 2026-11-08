namespace Awaitick.Core.Tests.Packaging;

internal static class RepoPaths
{
	public static string Root { get; } = FindRoot();

	public static string AppProject => Path.Combine(Root, "src", "Awaitick");

	private static string FindRoot()
	{
		DirectoryInfo? directory = new(AppContext.BaseDirectory);
		while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "Awaitick.slnx")))
		{
			directory = directory.Parent;
		}

		return directory?.FullName ?? throw new DirectoryNotFoundException("Repository root (Awaitick.slnx) not found.");
	}
}
