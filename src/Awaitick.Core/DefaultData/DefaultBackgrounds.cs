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

		var folder = await StorageFolder.GetFolderFromPathAsync(
			System.IO.Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "Assets", "EventBackgrounds"));

		var files = await folder.GetFilesAsync();

		_cachedBackgrounds = files
			.Where(f => f.FileType is ".jpg" or ".png")
			.Select(f => new DefaultBackground(System.IO.Path.GetFileNameWithoutExtension(f.Name)))
			.ToArray();

		return _cachedBackgrounds;
	}
}
