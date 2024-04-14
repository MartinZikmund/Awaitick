using CommunityToolkit.Mvvm.DependencyInjection;
using EventCountdowns.Core.Configuration;
using EventCountdowns.Core.Infrastructure;
using EventCountdowns.Core.ViewModels;
using EventCountdowns.Services.Navigation;
using EventCountdowns.ViewModels;
using MZikmund.Services.Loading;
using MZikmund.Services.Dialogs;

namespace EventCountdowns;

public class CountdownsApp : Application, IApplication
{
	public Window? MainWindow { get; private set; }

	protected IHost? Host { get; private set; }

	protected override void OnLaunched(LaunchActivatedEventArgs args)
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
		MainWindow.EnableHotReload();
#endif

		Host = builder.Build();
		Ioc.Default.ConfigureServices(Host.Services);

		// Do not repeat app initialization when the Window already has content,
		// just ensure that the window is active
		if (MainWindow.Content is not WindowShell windowShell)
		{
			// Create a Frame to act as the navigation context and navigate to the first page
			windowShell = new WindowShell(Host.Services, MainWindow);

			// Place the frame in the current Window
			MainWindow.Content = windowShell;
		}

		if (windowShell.Content == null)
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

		services.AddSingleton<IApplication>(this);
		services.AddScoped<IDialogCoordinator, DialogCoordinator>();
		services.AddScoped<IFrameProvider, FrameProvider>();
		services.AddScoped<INavigationService, NavigationService>();
		services.AddScoped<ILoadingIndicator, LoadingIndicator>();
		services.AddScoped<IDialogService, DialogService>();
		services.AddScoped<IWindowShellProvider, WindowShellProvider>();
	}
}
