using EventCountdowns.Core.Models;

namespace EventCountdowns.Core.DefaultData;

public class DefaultBackgrounds : IDefaultBackgrounds
{
	private readonly Dictionary<string, DefaultBackground> _defaultBackgrounds = new()
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

	public DefaultBackground[] GetDefaultBackgrounds() => _defaultBackgrounds.Values.ToArray();

	public DefaultBackground GetSampleEventBackground(EventPreset sampleEventKind) => sampleEventKind switch
	{
		EventPreset.Christmas => _defaultBackgrounds["christmas"],
		EventPreset.Easter => _defaultBackgrounds["easter"],
		EventPreset.Halloween => _defaultBackgrounds["halloween"],
		EventPreset.NewYear => _defaultBackgrounds["newyear"],
		_ => throw new ArgumentOutOfRangeException(nameof(sampleEventKind), sampleEventKind, null),
	};
}
