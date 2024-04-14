namespace EventCountdowns.Core.Models;

public class DefaultBackground
{
	public DefaultBackground(string key, string backgroundPath, string thumbnailPath)
	{
		Key = key;
		BackgroundPath = backgroundPath;
		ThumbnailPath = thumbnailPath;
	}

	public string Key { get; set; }

	public string BackgroundPath { get; set; }

	public string ThumbnailPath { get; set; }
}
