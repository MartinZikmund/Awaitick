using Awaitick.Core.Infrastructure;
using Microsoft.UI.Xaml.Media;

namespace Awaitick.Extensions;

public static class UIElementExtensions
{
	public static IServiceProvider? GetServiceProvider(this UIElement element)
	{
		if (element.XamlRoot?.Content is not { } root)
		{
			return null;
		}

		return FindWindowShell(root)?.ServiceProvider;
	}

	private static IWindowShell? FindWindowShell(DependencyObject element)
	{
		if (element is IWindowShell windowShell)
		{
			return windowShell;
		}

		var childCount = VisualTreeHelper.GetChildrenCount(element);
		for (var i = 0; i < childCount; i++)
		{
			if (FindWindowShell(VisualTreeHelper.GetChild(element, i)) is { } found)
			{
				return found;
			}
		}

		return null;
	}
}
