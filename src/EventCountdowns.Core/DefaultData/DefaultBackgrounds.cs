using System;
using System.Collections.Generic;
using System.Linq;
using EventCountdowns.Core.Models;

namespace EventCountdowns.Core.DefaultData;

public class DefaultBackgrounds : IDefaultBackgrounds
{
	private readonly Dictionary<string, DefaultBackground> _defaultBackgrounds = new Dictionary<string, DefaultBackground>()
	{
		{ "blank", new DefaultBackground("", null, "ms-appx:///Assets/SampleBackgrounds/Thumbnails/BlankBackground.jpg") },
		{ "christmas", new DefaultBackground("", "ms-appx:///Assets/SampleBackgrounds/Christmas.jpg", "ms-appx:///Assets/SampleBackgrounds/Thumbnails/Christmas.jpg") },
		{ "easter", new DefaultBackground("", "ms-appx:///Assets/SampleBackgrounds/Easter.jpg", "ms-appx:///Assets/SampleBackgrounds/Thumbnails/Easter.jpg") },
		{ "halloween", new DefaultBackground("", "ms-appx:///Assets/SampleBackgrounds/Halloween.jpg", "ms-appx:///Assets/SampleBackgrounds/Thumbnails/Halloween.jpg") },
		{ "beach", new DefaultBackground("", "ms-appx:///Assets/SampleBackgrounds/Beach.jpg", "ms-appx:///Assets/SampleBackgrounds/Thumbnails/Beach.jpg") },
		{ "concert", new DefaultBackground("", "ms-appx:///Assets/SampleBackgrounds/Concert.jpg", "ms-appx:///Assets/SampleBackgrounds/Thumbnails/Concert.jpg") },
		{ "love", new DefaultBackground("", "ms-appx:///Assets/SampleBackgrounds/Love.jpg", "ms-appx:///Assets/SampleBackgrounds/Thumbnails/Love.jpg") },
		{ "movies", new DefaultBackground("", "ms-appx:///Assets/SampleBackgrounds/Movies.jpg", "ms-appx:///Assets/SampleBackgrounds/Thumbnails/Movies.jpg") },
		{ "newyear", new DefaultBackground("", "ms-appx:///Assets/SampleBackgrounds/NewYear.jpg", "ms-appx:///Assets/SampleBackgrounds/Thumbnails/NewYear.jpg") },
		{ "plane", new DefaultBackground("", "ms-appx:///Assets/SampleBackgrounds/Plane.jpg", "ms-appx:///Assets/SampleBackgrounds/Thumbnails/Plane.jpg") },
	};
	public DefaultBackground[] GetDefaultBackgrounds()
	{
		return _defaultBackgrounds.Values.ToArray();
	}

	public DefaultBackground GetSampleEventBackground(SampleEventTypes sampleEventKind)
	{
		switch (sampleEventKind)
		{
			case SampleEventTypes.Christmas:
				return _defaultBackgrounds["christmas"];
			case SampleEventTypes.Easter:
				return _defaultBackgrounds["easter"];
			case SampleEventTypes.Halloween:
				return _defaultBackgrounds["halloween"];
			case SampleEventTypes.NewYear:
				return _defaultBackgrounds["newyear"];
			default:
				throw new ArgumentOutOfRangeException(nameof(sampleEventKind), sampleEventKind, null);
		}
	}
}
