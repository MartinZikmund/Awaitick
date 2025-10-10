using Awaitick.Core.Models;

namespace Awaitick.Core.DefaultData;

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
}
