using EventCountdowns.Services.Localization;
using Microsoft.UI.Xaml.Markup;

namespace EventCountdowns.Extensions.Xaml;

public class Localize : MarkupExtension
{
	public string Key { get; set; } = "";

	protected override object ProvideValue() => Localizer.Instance.GetString(Key);
}
