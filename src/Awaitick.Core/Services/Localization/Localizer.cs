using CommunityToolkit.Mvvm.DependencyInjection;

namespace Awaitick.Services.Localization;

public class Localizer
{
	private static readonly Lazy<IStringLocalizer> _stringLocalizer = new(Ioc.Default.GetRequiredService<IStringLocalizer>);

	private Localizer()
	{
	}

	public static Localizer Instance { get; } = new Localizer();

	public string GetString(string key)
	{
		var result = _stringLocalizer.Value.GetString(key);
		if (result.ResourceNotFound)
		{
			this.Log().LogError($"Key '{key}' not found in localization resources.");
			return $"??{key}??";
		}
		else
		{
			return result.Value;
		}
	}

	public string this[string key] => GetString(key);
}
