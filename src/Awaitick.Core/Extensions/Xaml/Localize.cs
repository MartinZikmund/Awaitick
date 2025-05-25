using Awaitick.Services.Localization;
using Microsoft.UI.Xaml.Markup;

namespace Awaitick.Extensions.Xaml;

[MarkupExtensionReturnType(ReturnType = typeof(string))]
public class LocalizeExtension : MarkupExtension
{
	public string Key { get; set; } = "";

	protected override object ProvideValue() => Localizer.Instance.GetString(Key);
}
