using CommunityToolkit.Mvvm.DependencyInjection;
using EventCountdowns.Core.Configuration;
using EventCountdowns.Core.Infrastructure;
using EventCountdowns.Core.ViewModels;
using EventCountdowns.Services.Navigation;
using EventCountdowns.ViewModels;
using MZikmund.Services.Loading;
using MZikmund.Services.Dialogs;
using EventCountdowns.Core.Services.Data;
using EventCountdowns.Core.Services.Tiles;
using EventCountdowns.Core.Services;
using EventCountdowns.Core.Services.InAppPurchases;
using EventCountdowns.Core.Services.Mail;
using EventCountdowns.Core.Services.ScheduledNotification;
using EventCountdowns.Core.Services.StoreLauncher;
using EventCountdowns.Core.Services.Settings;
using EventCountdowns.Core.Services.EventCountdownManager;
using EventCountdowns.Core.Services.BackgroundPicker;
using EventCountdowns.Core.DefaultData;
using CommunityToolkit.Mvvm.Messaging;
using Uno.Resizetizer;
using MZikmund.Toolkit.WinUI.Infrastructure;
using EventCountdowns.Core.Services.Countdowns;
using MZikmund.Toolkit.WinUI.Services;

namespace EventCountdowns;

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

		await Host.Services.GetRequiredService<IDataService>().InitializeAsync();

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
			// When the navigation stack isn't restored navigate to the first page,
			// configuring the new page by passing required information as a navigation
			// parameter
			windowShell.ServiceProvider.GetRequiredService<INavigationService>().Navigate<MainViewModel>(args.Arguments);
		}

		// Ensure the current window is active
		MainWindow.Activate();
	}

	private void ConfigureServices(IServiceCollection services)
	{
		services.AddScoped<WindowShellViewModel>();
		services.AddScoped<MainViewModel>();
		services.AddTransient<CountdownEditorViewModel>();
		services.AddTransient<CountdownDetailViewModel>();

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
		services.AddScoped<IBackgroundPickerService, BackgroundPickerService>();
		services.AddSingleton<IDefaultBackgrounds, DefaultBackgrounds>();
		services.AddSingleton<IDataService, FileDataService>();
		services.AddSingleton<IFileService, FileService>();
		services.AddSingleton<ITileService, TileService>();
		services.AddSingleton<IInAppPurchaseService, InAppPurchaseService>();
		services.AddSingleton<IMailService, MailService>();
		services.AddSingleton<IScheduledNotificationService, ScheduledNotificationService>();
		services.AddSingleton<IStoreLauncherService, StoreLauncherService>();
		services.AddSingleton<ISettingsService, SettingsService>();
		services.AddSingleton<IAppPreferences, AppPreferences>();
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
