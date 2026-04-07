using Windows.Foundation.Metadata;
using Awaitick.Services.Navigation;
using Awaitick.Core.Infrastructure;
using Awaitick.Core.Messages;
using Awaitick.ViewModels;
using Awaitick.Core.Services.Settings;
using Awaitick.Services.Theming;
using Awaitick.Views;
using CommunityToolkit.Mvvm.Messaging;

namespace Awaitick;

public sealed partial class WindowShell : Page, IWindowShell
{
	private readonly IServiceScope _windowScope;
	private readonly Window _associatedWindow;

	public WindowShell(IServiceProvider serviceProvider, Window associatedWindow)
	{
		InitializeComponent();

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
			}
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

	private void OnOverlayVisibilityChanged(object recipient, OverlayVisibilityChangedMessage message)
	{
		if (!HasCustomTitleBar)
		{
			return;
		}

		TitleBar.Opacity = message.IsVisible ? 1 : 0;
		TitleBar.IsHitTestVisible = message.IsVisible;
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

		if (ApiInformation.IsPropertyPresent("Microsoft.UI.Xaml.Window", "SystemBackdrop"))
		{
			_associatedWindow.SystemBackdrop = new MicaBackdrop();
			Background = null;
		}
	}
}
