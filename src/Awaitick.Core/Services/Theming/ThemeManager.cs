using Awaitick.Services.Navigation;
using Microsoft.UI;
using Windows.UI;
using Windows.UI.ViewManagement;

namespace Awaitick.Services.Theming;

public class ThemeManager : IThemeManager
{
	private readonly IWindowShellProvider _windowShellProvider;
	private readonly UISettings _uiSettings = new();

	public ThemeManager(IWindowShellProvider windowShellProvider)
	{
		_windowShellProvider = windowShellProvider;
		_uiSettings.ColorValuesChanged += OnColorValuesChanged;
	}

	public void SetTheme(ElementTheme theme)
	{
		GetRootElement().RequestedTheme = theme;
		UpdateTitleBarTheming();
	}

	public void SetTitleBarTheme(ElementTheme theme)
	{
		UpdateTitleBarTheming(theme);
	}

	public ElementTheme CurrentTheme => GetRootElement().RequestedTheme;

	public ApplicationTheme ActualTheme => GetApplicationTheme(CurrentTheme);

	private static ApplicationTheme GetApplicationTheme(ElementTheme theme) => theme switch
	{
		ElementTheme.Default => Application.Current.RequestedTheme,
		ElementTheme.Light => ApplicationTheme.Light,
		ElementTheme.Dark => ApplicationTheme.Dark,
		_ => throw new ArgumentOutOfRangeException(nameof(theme), theme, null)
	};

	private FrameworkElement GetRootElement()
	{
		var rootElement = _windowShellProvider.Shell as FrameworkElement;
		if (rootElement is null)
		{
			throw new InvalidOperationException("Root element of the window is not a FrameworkElement");
		}

		return rootElement;
	}

	private void UpdateTitleBarTheming(ElementTheme? themeOverride = null)
	{
		var effectiveTheme = themeOverride.HasValue
			? GetApplicationTheme(themeOverride.Value)
			: ActualTheme;
#pragma warning disable CS8618
#pragma warning disable Uno0001
		var titleBar = _windowShellProvider.Window.AppWindow.TitleBar;
		titleBar.BackgroundColor = Colors.Transparent;
		titleBar.ButtonBackgroundColor = Colors.Transparent;
		titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
		if (effectiveTheme == ApplicationTheme.Dark)
		{
			titleBar.ButtonForegroundColor = Colors.White;
			titleBar.ButtonInactiveForegroundColor = Colors.Gray;
			titleBar.ButtonHoverBackgroundColor = Color.FromArgb(100, 100, 100, 100);
			titleBar.ButtonHoverForegroundColor = Colors.White;
			titleBar.ButtonPressedBackgroundColor = Color.FromArgb(200, 100, 100, 100);
			titleBar.ButtonPressedForegroundColor = Colors.White;
		}
		else
		{
			titleBar.ButtonForegroundColor = Colors.Black;
			titleBar.ButtonInactiveForegroundColor = Colors.Gray;
			titleBar.ButtonHoverBackgroundColor = Color.FromArgb(100, 200, 200, 200);
			titleBar.ButtonHoverForegroundColor = Colors.Black;
			titleBar.ButtonPressedBackgroundColor = Color.FromArgb(200, 200, 200, 200);
			titleBar.ButtonPressedForegroundColor = Colors.Black;
		}
#pragma warning restore Uno0001
#pragma warning restore CS8618
	}

	private void OnColorValuesChanged(UISettings sender, object args) =>
		_windowShellProvider.DispatcherQueue.TryEnqueue(UpdateTitleBarTheming);
}
