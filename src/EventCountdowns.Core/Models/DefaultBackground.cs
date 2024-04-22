namespace EventCountdowns.Core.Models;

public class DefaultBackground
{
	public DefaultBackground(string name)
	{
		BackgroundUri = new Uri($"ms-appx:///EventCountdowns/Assets/SampleBackgrounds/{name}.png", UriKind.Absolute);
		ThumbnailUri = new Uri($"ms-appx:///EventCountdowns/Assets/SampleBackgrounds/Thumbnails/{name}.png", UriKind.Absolute);
	}

	public Uri BackgroundUri { get; }

	public Uri ThumbnailUri { get; }
}
