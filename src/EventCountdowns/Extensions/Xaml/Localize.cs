using EventCountdowns.Core.Infrastructure;
using Microsoft.UI.Xaml.Markup;

namespace EventCountdowns.Extensions.Xaml;

public class Localize : MarkupExtension
{
	private static Lazy<IStringLocalizer> _localization = new Lazy<IStringLocalizer>(
		() =>
		{
			return IoC.GetRequiredService<IStringLocalizer>();
		});

	public string Key { get; set; }

	protected override object ProvideValue() => _localization.Value.GetString(Key);
}
