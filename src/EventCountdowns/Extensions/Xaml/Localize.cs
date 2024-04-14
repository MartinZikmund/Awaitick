using System;
using EventCountdowns.Core.Infrastructure;
using EventCountdowns.Core.Services;
using Windows.ApplicationModel;
using Microsoft.UI.Xaml.Markup;


/* Unmerged change from project 'EventCountdowns (net8.0)'
Before:
namespace EventCountdowns.Extensions.Xaml
{
	public class Localize : MarkupExtension
After:
namespace EventCountdowns.Extensions.Xaml;

	public class Localize : MarkupExtension
*/
namespace EventCountdowns.Extensions.Xaml;

public class Localize : MarkupExtension
{
	private static Lazy<ILocalizationService> _localization = new Lazy<ILocalizationService>(
		() =>
		{
			if (DesignMode.DesignMode2Enabled)
			{
				return new LocalizationService();
			}

			return IoC.GetRequiredService<ILocalizationService>();
		});

	public string Key { get; set; }

	protected override object ProvideValue() => _localization.Value.GetString(Key);

/* Unmerged change from project 'EventCountdowns (net8.0)'
Removed:
}
*/
}
