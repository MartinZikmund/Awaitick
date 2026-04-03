namespace Awaitick.Core.Models;

public class DefaultBackground
{
	public DefaultBackground(string name)
	{
		var extension = System.IO.Path.GetExtension(name);
		if (string.IsNullOrEmpty(extension))
		{
			extension = ".jpg";
		}
		else
		{
			name = System.IO.Path.GetFileNameWithoutExtension(name);
		}

		BackgroundUri = new Uri($"ms-appx:///Assets/EventBackgrounds/{name}{extension}", UriKind.Absolute);
		ThumbnailUri = new Uri($"ms-appx:///Assets/EventBackgrounds/Thumbnails/{name}Thumbnail{extension}", UriKind.Absolute);
	}

	public Uri BackgroundUri { get; }

	public Uri ThumbnailUri { get; }
}
