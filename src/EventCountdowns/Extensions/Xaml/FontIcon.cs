using Microsoft.UI.Xaml.Markup;

namespace EventCountdowns.Extensions.Xaml;

[MarkupExtensionReturnType(ReturnType = typeof(FontIcon))]
public class FontIconExtension : MarkupExtension
{
	public string Glyph { get; set; } = "";

	protected override object ProvideValue()
	{
		var fontIcon = new FontIcon() { Glyph = Glyph };
		return fontIcon;
	}
}
