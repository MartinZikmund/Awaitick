using EventCountdowns.Core.Models;

namespace EventCountdowns.Core.DefaultData;

public class DefaultBackgrounds : IDefaultBackgrounds
{
	private readonly Dictionary<string, DefaultBackground> _defaultBackgrounds = new Dictionary<string, DefaultBackground>()
	{
		{ "blank", new DefaultBackground("BlankBackground") },
		{ "christmas", new DefaultBackground("Christmas") },
		{ "easter", new DefaultBackground("Easter") },
		{ "halloween", new DefaultBackground("Halloween") },
		{ "beach", new DefaultBackground("Beach") },
		{ "concert", new DefaultBackground("Concert") },
		{ "love", new DefaultBackground("Love") },
		{ "movies", new DefaultBackground("Movies") },
		{ "newyear", new DefaultBackground("NewYear") },
		{ "plane", new DefaultBackground("Plane")},
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
