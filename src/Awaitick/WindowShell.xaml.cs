using Awaitick.Core.Infrastructure;
using Awaitick.Core.Messages;
using Awaitick.Core.Services.Settings;
using Awaitick.Core.ViewModels;
using Awaitick.Services.Navigation;
using Awaitick.Services.Theming;
using Awaitick.ViewModels;
using Awaitick.Views;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Composition.SystemBackdrops;
using Windows.Foundation.Metadata;

namespace Awaitick;

public sealed partial class WindowShell : Page, IWindowShell
{
	private readonly IServiceScope _windowScope;
	private readonly Window _associatedWindow;
	private bool _titleBarVisible = true;

	public WindowShell(IServiceProvider serviceProvider, Window associatedWindow)
	{
		InitializeComponent();
		Loaded += WindowShell_Loaded;
		_windowScope = serviceProvider.CreateScope();

		var windowShellProvider = (WindowShellProvider)ServiceProvider.GetRequiredService<IWindowShellProvider>();
		windowShellProvider.SetShell(this, associatedWindow);
		ServiceProvider.GetRequiredService<INavigationService>().RegisterViewsFromAssembly(typeof(CountdownsApp).Assembly);

		var settings = ServiceProvider.GetRequiredService<IAppPreferences>();
		var themeService = ServiceProvider.GetRequiredService<IThemeManager>();
		themeService.SetTheme(settings.Theme);

		ViewModel = ServiceProvider.GetRequiredService<WindowShellViewModel>();

		_associatedWindow = associatedWindow;
		CustomizeWindow();

		InnerFrame.Navigated += OnFrameNavigated;

		var messenger = ServiceProvider.GetRequiredService<IMessenger>();
		messenger.Register<FullScreenChangedMessage>(this, OnFullScreenChanged);
		messenger.Register<OverlayVisibilityChangedMessage>(this, OnOverlayVisibilityChanged);
		messenger.Register<TitleBarThemeOverrideMessage>(this, OnTitleBarThemeOverride);
	}

	private void WindowShell_Loaded(object sender, RoutedEventArgs e)
	{
		if (RootFrame.Content is null)
		{
			var appPreferences = ServiceProvider.GetRequiredService<IAppPreferences>();
			var navigationService = ServiceProvider.GetRequiredService<INavigationService>();
			if (appPreferences.FirstStart)
			{
				navigationService.Navigate<OnboardingViewModel>((Application.Current as CountdownsApp)?.LaunchArgs);
			}
			else
			{
				navigationService.Navigate<MainViewModel>((Application.Current as CountdownsApp)?.LaunchArgs);
			}
		}
	}

	public IServiceProvider ServiceProvider => _windowScope.ServiceProvider;

	public WindowShellViewModel ViewModel { get; }

	public Frame RootFrame => InnerFrame;

	public bool HasCustomTitleBar { get; private set; }

	private void OnFrameNavigated(object sender, NavigationEventArgs e)
	{
		var blendsInTitleBar = InnerFrame.Content is IBlendsInTitleBar page && page.BlendsInTitleBar;

		if (blendsInTitleBar)
		{
			TitleBar.Background = new SolidColorBrush(Microsoft.UI.Colors.Transparent);
		}
		else
		{
			TitleBar.ClearValue(Panel.BackgroundProperty);

			if (HasCustomTitleBar)
			{
				TitleBar.Opacity = 1;
				TitleBar.IsHitTestVisible = true;
				_titleBarVisible = true;
			}
		}

		// Reset title bar button theming to app theme when navigating to a
		// non-blending page.  Pages that blend into the title bar (e.g.,
		// CountdownDetailView) set their own override via
		// TitleBarThemeOverrideMessage once their data loads — resetting
		// here would race with that (OnNavigatedTo fires before Navigated).
		if (!blendsInTitleBar)
		{
			ServiceProvider.GetRequiredService<IThemeManager>().SetTitleBarThemeOverride(null);
			TitleBar.RequestedTheme = ElementTheme.Default;
		}
	}

	private void OnFullScreenChanged(object recipient, FullScreenChangedMessage message)
	{
#if !HAS_UNO
		var presenter = message.IsFullScreen
			? Microsoft.UI.Windowing.AppWindowPresenterKind.FullScreen
			: Microsoft.UI.Windowing.AppWindowPresenterKind.Default;
		_associatedWindow.AppWindow.SetPresenter(presenter);
#endif
	}

	private void OnTitleBarThemeOverride(object recipient, TitleBarThemeOverrideMessage message)
	{
		ServiceProvider.GetRequiredService<IThemeManager>().SetTitleBarThemeOverride(message.ThemeOverride);
		TitleBar.RequestedTheme = message.ThemeOverride switch
		{
			ApplicationTheme.Dark => ElementTheme.Dark,
			ApplicationTheme.Light => ElementTheme.Light,
			_ => ElementTheme.Default,
		};
	}

	private void OnOverlayVisibilityChanged(object recipient, OverlayVisibilityChangedMessage message)
	{
		if (!HasCustomTitleBar)
		{
			return;
		}

		if (message.IsVisible)
		{
			TitleBar.Opacity = 1;
			TitleBar.IsHitTestVisible = true;
			_titleBarVisible = true;
		}
		else
		{
			TitleBar.Opacity = 0;
			TitleBar.IsHitTestVisible = false;
			_titleBarVisible = false;
		}
	}

	private void CustomizeWindow()
	{
		if (ApiInformation.IsPropertyPresent("Microsoft.UI.Xaml.Window", "ExtendsContentIntoTitleBar"))
		{
#if !HAS_UNO
			_associatedWindow.ExtendsContentIntoTitleBar = true;
			_associatedWindow.AppWindow.TitleBar.PreferredHeightOption = Microsoft.UI.Windowing.TitleBarHeightOption.Tall;
			_associatedWindow.SetTitleBar(DraggableTitleBar);
			HasCustomTitleBar = true;
#endif
		}

		if (MicaController.IsSupported())
		{
			_associatedWindow.SystemBackdrop = new MicaBackdrop();
			Background = null;
		}
	}
}
