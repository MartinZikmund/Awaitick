using CommunityToolkit.Mvvm.DependencyInjection;
using Awaitick.Core.Configuration;
using Awaitick.Core.Infrastructure;
using Awaitick.Core.ViewModels;
using Awaitick.Services.Navigation;
using Awaitick.ViewModels;
using MZikmund.Services.Loading;
using MZikmund.Services.Dialogs;
using Awaitick.Core.Services.Data;
using Awaitick.Core.Services.Tiles;
using Awaitick.Core.Services;
using Awaitick.Core.Services.InAppPurchases;
using Awaitick.Core.Services.Mail;
using Awaitick.Core.Services.ScheduledNotification;
using Awaitick.Core.Services.StoreLauncher;
using Awaitick.Core.Services.Settings;
using Awaitick.Core.Services.EventCountdownManager;
using Awaitick.Core.DefaultData;
using CommunityToolkit.Mvvm.Messaging;
using Uno.Resizetizer;
using MZikmund.Toolkit.WinUI.Infrastructure;
using Awaitick.Core.Services.Countdowns;
using MZikmund.Toolkit.WinUI.Services;
using Awaitick.Services.Theming;
using Awaitick.Services.Store;
using Microsoft.Extensions.DependencyInjection;
using Awaitick.Services;
using Windows.Devices.WiFiDirect.Services;
using Awaitick.Core.Models;
using System.Text.Json;
using Awaitick.Core.Services.DeepLink;

namespace Awaitick;

public partial class CountdownsApp : Application, IApplication
{
	/// <summary>
	/// Initializes the singleton application object. This is the first line of authored code
	/// executed, and as such is the logical equivalent of main() or WinMain().
	/// </summary>
	public CountdownsApp()
	{
		this.InitializeComponent();
	}

#if WINDOWS
	/// <summary>
	/// Handles toast notification activation arguments.
	/// </summary>
	private void HandleToastActivation(string launchArgs)
	{
		if (string.IsNullOrEmpty(launchArgs))
		{
			return;
		}

		// Parse the launch arguments (format: "action=viewCountdown&countdownId=guid")
		var queryParams = launchArgs.Split('&')
			.Select(p => p.Split('='))
			.Where(p => p.Length == 2)
			.ToDictionary(p => p[0], p => p[1]);

		if (queryParams.TryGetValue(NotificationConstants.CountdownIdKey, out var countdownId))
		{
			var deepLinkService = Host?.Services.GetService<IDeepLinkService>();
			deepLinkService?.SetPendingNavigation(countdownId);

			// If app is already running, trigger navigation on UI thread
			MainWindow?.DispatcherQueue.TryEnqueue(() =>
			{
				// Navigation will be handled by MainViewModel when it checks for pending navigation
				var messenger = Host?.Services.GetService<IMessenger>();
				messenger?.Send(new Core.Messages.DeepLinkReceivedMessage());
			});
		}
	}
#endif

	public Window? MainWindow { get; private set; }

	protected IHost? Host { get; private set; }

	protected override async void OnLaunched(LaunchActivatedEventArgs args)
	{
		var builder = this.CreateBuilder(args)
			.Configure(host => host
#if DEBUG
				// Switch to Development environment when running in DEBUG
				.UseEnvironment(Environments.Development)
#endif
				.UseConfiguration(configure: configBuilder =>
					configBuilder
						.EmbeddedSource<CountdownsApp>()
						.Section<AppConfig>()
				)
				.UseSerialization(services =>
				{
					services.AddSingleton(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
					services
						.AddJsonTypeInfo(EventCountdownSerializerContext.Default.EventCountdown);
				})
				// Enable localization (see appsettings.json for supported languages)
				.UseLocalization()
				.ConfigureServices((context, services) => ConfigureServices(services))
			);
		MainWindow = builder.Window;

#if DEBUG
		MainWindow.UseStudio();
#endif

		MainWindow.SetWindowIcon();

		Host = builder.Build();
		Ioc.Default.ConfigureServices(Host.Services);
		IoC.SetProvider(Host.Services);

#if __IOS__
		if (!string.IsNullOrEmpty(Awaitick.iOS.NotificationDelegate.PendingCountdownId))
		{
			var deepLinkService = Host.Services.GetService<IDeepLinkService>();
			deepLinkService?.SetPendingNavigation(Awaitick.iOS.NotificationDelegate.PendingCountdownId);
			Awaitick.iOS.NotificationDelegate.PendingCountdownId = null;
		}
#elif __ANDROID__
		if (!string.IsNullOrEmpty(Awaitick.Droid.MainActivity.PendingCountdownId))
		{
			var deepLinkService = Host.Services.GetService<IDeepLinkService>();
			deepLinkService?.SetPendingNavigation(Awaitick.Droid.MainActivity.PendingCountdownId);
			Awaitick.Droid.MainActivity.PendingCountdownId = null;
		}
#endif

		var dataService = Host.Services.GetRequiredService<IDataService>();
		await dataService.InitializeAsync();

		// Reschedule all notifications on startup to ensure they persist after device restart or app updates
		var appPreferencesStartup = Host.Services.GetRequiredService<IAppPreferences>();
		if (appPreferencesStartup.NotificationsEnabled)
		{
			var notificationService = Host.Services.GetRequiredService<IScheduledNotificationService>();
			var countdowns = await dataService.GetCountdownsAsync();
			var futureCountdowns = countdowns.Where(c => c.TargetDateTime > DateTimeOffset.Now);
			await notificationService.RescheduleAllNotificationsAsync(futureCountdowns);
		}

#if WINDOWS
		// Handle toast notification activation (launch arguments contain the toast launch string)
		if (!string.IsNullOrEmpty(args.Arguments))
		{
			HandleToastActivation(args.Arguments);
		}
#endif

		var appPreferences = Host.Services.GetRequiredService<IAppPreferences>();

		// Do not repeat app initialization when the Window already has content,
		// just ensure that the window is active
		if (MainWindow.Content is not WindowShell windowShell)
		{
			// Create a Frame to act as the navigation context and navigate to the first page
			windowShell = new WindowShell(Host.Services, MainWindow);

			// Place the frame in the current Window
			MainWindow.Content = windowShell;
		}

		if (windowShell.RootFrame.Content is null)
		{
			if (appPreferences.FirstStart)
			{
				windowShell.ServiceProvider.GetRequiredService<INavigationService>().Navigate<OnboardingViewModel>(args.Arguments);
			}
			else
			{
				windowShell.ServiceProvider.GetRequiredService<INavigationService>().Navigate<MainViewModel>(args.Arguments);
			}
		}

		// Ensure the current window is active
		MainWindow.Activate();
	}

	private void ConfigureServices(IServiceCollection services)
	{
		services.AddScoped<WindowShellViewModel>();
		services.AddScoped<SettingsViewModel>();
		services.AddScoped<MainViewModel>();
		services.AddTransient<OnboardingViewModel>();
		services.AddScoped<GetProViewModel>();
		services.AddTransient<CountdownEditorViewModel>();
		services.AddTransient<CountdownDetailViewModel>();
		services.AddTransient<NewCountdownViewModel>();

		services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);

		services.AddSingleton<IApplication>(this);
		services.AddSingleton<WindowShellProvider>();
		services.AddScoped<IXamlRootProvider>(services => services.GetRequiredService<WindowShellProvider>());
		services.AddScoped<IWindowShellProvider>(services => services.GetRequiredService<WindowShellProvider>());
		services.AddScoped<IFrameProvider, FrameProvider>();
		services.AddScoped<INavigationService, NavigationService>();
		services.AddScoped<ILoadingIndicator, LoadingIndicator>();
		services.AddScoped<IDialogCoordinator, DialogCoordinator>();
		services.AddScoped<IDialogService, DialogService>();

		services.AddScoped<ISystemSharingService, SystemSharingService>();
		services.AddScoped<ICountdownsManager, CountdownsManager>();
		services.AddSingleton<ICountdownsDataService, CountdownsDataService>();
		services.AddScoped<IImagePickerService, ImagePickerService>();
		services.AddSingleton<IDefaultBackgrounds, DefaultBackgrounds>();
		services.AddSingleton<IFileService, FileService>();
		services.AddSingleton<ITileService, TileService>();
		services.AddSingleton<IMailService, MailService>();
		services.AddSingleton<IDeepLinkService, DeepLinkService>();
#if WINDOWS || __ANDROID__ || __IOS__
		services.AddSingleton<IScheduledNotificationService, Awaitick.Services.ScheduledNotification.ScheduledNotificationService>();
#else
		services.AddSingleton<IScheduledNotificationService, Awaitick.Core.Services.ScheduledNotification.ScheduledNotificationService>();
#endif
		services.AddScoped<INotificationPermissionService, NotificationPermissionService>();
		services.AddSingleton<IStoreLauncherService, StoreLauncherService>();
		services.AddSingleton<IPreferences, Preferences>();
		services.AddSingleton<IAppPreferences, AppPreferences>();
		services.AddScoped<IThemeManager, ThemeManager>();
#if HAS_UNO
		services.AddScoped<IStoreService, ProStoreService>();
#elif DEBUG
		services.AddScoped<IStoreService, FakeStoreService>();
#else
		services.AddScoped<IStoreService, StoreService>();
#endif
		services.AddSingleton<IDataService, FileDataService>();
	}

	/// <summary>
	/// Configures global Uno Platform logging
	/// </summary>
	public static void InitializeLogging()
	{
#if DEBUG
		// Logging is disabled by default for release builds, as it incurs a significant
		// initialization cost from Microsoft.Extensions.Logging setup. If startup performance
		// is a concern for your application, keep this disabled. If you're running on the web or
		// desktop targets, you can use URL or command line parameters to enable it.
		//
		// For more performance documentation: https://platform.uno/docs/articles/Uno-UI-Performance.html

		var factory = LoggerFactory.Create(builder =>
		{
#if __WASM__
            builder.AddProvider(new global::Uno.Extensions.Logging.WebAssembly.WebAssemblyConsoleLoggerProvider());
#elif __IOS__ || __MACCATALYST__
            builder.AddProvider(new global::Uno.Extensions.Logging.OSLogLoggerProvider());
#else
			builder.AddConsole();
#endif

			// Exclude logs below this level
			builder.SetMinimumLevel(LogLevel.Information);

			// Default filters for Uno Platform namespaces
			builder.AddFilter("Uno", LogLevel.Warning);
			builder.AddFilter("Windows", LogLevel.Warning);
			builder.AddFilter("Microsoft", LogLevel.Warning);

			// Generic Xaml events
			// builder.AddFilter("Microsoft.UI.Xaml", LogLevel.Debug );
			// builder.AddFilter("Microsoft.UI.Xaml.VisualStateGroup", LogLevel.Debug );
			// builder.AddFilter("Microsoft.UI.Xaml.StateTriggerBase", LogLevel.Debug );
			// builder.AddFilter("Microsoft.UI.Xaml.UIElement", LogLevel.Debug );
			// builder.AddFilter("Microsoft.UI.Xaml.FrameworkElement", LogLevel.Trace );

			// Layouter specific messages
			// builder.AddFilter("Microsoft.UI.Xaml.Controls", LogLevel.Debug );
			// builder.AddFilter("Microsoft.UI.Xaml.Controls.Layouter", LogLevel.Debug );
			// builder.AddFilter("Microsoft.UI.Xaml.Controls.Panel", LogLevel.Debug );

			// builder.AddFilter("Windows.Storage", LogLevel.Debug );

			// Binding related messages
			// builder.AddFilter("Microsoft.UI.Xaml.Data", LogLevel.Debug );
			// builder.AddFilter("Microsoft.UI.Xaml.Data", LogLevel.Debug );

			// Binder memory references tracking
			// builder.AddFilter("Uno.UI.DataBinding.BinderReferenceHolder", LogLevel.Debug );

			// DevServer and HotReload related
			// builder.AddFilter("Uno.UI.RemoteControl", LogLevel.Information);

			// Debug JS interop
			// builder.AddFilter("Uno.Foundation.WebAssemblyRuntime", LogLevel.Debug );
		});

		global::Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory = factory;

#if HAS_UNO
        global::Uno.UI.Adapter.Microsoft.Extensions.Logging.LoggingAdapter.Initialize();
#endif
#endif
	}
}
