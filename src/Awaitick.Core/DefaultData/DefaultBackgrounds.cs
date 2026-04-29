using Awaitick.Core.Models;
using Windows.Storage;

namespace Awaitick.Core.DefaultData;

public class DefaultBackgrounds : IDefaultBackgrounds
{
	private DefaultBackground[]? _cachedBackgrounds;

	public async Task<DefaultBackground[]> GetDefaultBackgroundsAsync()
	{
		if (_cachedBackgrounds is not null)
		{
			return _cachedBackgrounds;
		}

		try
		{
			var installedLocation = Windows.ApplicationModel.Package.Current.InstalledLocation;
			var assetsFolder = await installedLocation.GetFolderAsync("Assets");
			var folder = await assetsFolder.GetFolderAsync("EventBackgrounds");
			var files = await folder.GetFilesAsync();

			_cachedBackgrounds = files
				.Where(f => f.FileType is ".jpg" or ".png")
				.Select(f => new DefaultBackground(f.Name))
				.ToArray();
		}
		catch
		{
			_cachedBackgrounds = [];
		}

		return _cachedBackgrounds;
	}
}
