using Awaitick.Core.Infrastructure;
using CommunityToolkit.WinUI;

namespace Awaitick.Extensions;

public static class UIElementExtensions
{
	public static IServiceProvider? GetServiceProvider(this UIElement element)
	{
		if (element.XamlRoot?.Content?.FindDescendantOrSelf<UIElement>(element => element is IWindowShell) is not IWindowShell windowShell)
		{
			return null;
		}

		return windowShell.ServiceProvider;
	}
}
