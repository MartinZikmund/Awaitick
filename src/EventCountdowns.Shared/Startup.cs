using System;
using EventCountdowns.Core.Services;
using EventCountdowns.Core.Services.Navigation;
using EventCountdowns.Core.Services.Settings;
using EventCountdowns.Core.ViewModels;
using EventCountdowns.Core.Services.Tiles;
using EventCountdowns.Services.Navigation;
using EventCountdowns.Services.Theming;
using EventCountdowns.Views;
using Microsoft.Extensions.DependencyInjection;
using EventCountdowns.Core.Services.Data;

namespace EventCountdowns
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton<IInAppPurchaseService, InAppPurchaseService>()
                .AddSingleton<IMailService, MailService>()
                .AddSingleton<IScheduledNotificationService, ScheduledNotificationService>()
                .AddSingleton<IStoreLauncherService, StoreLauncherService>()
                .AddSingleton<ITileService, TileService>()
                .AddSingleton<IFileService, FileService>()
                .AddSingleton<IDataService, FileDataService>()
                .AddSingleton<ILocalizationService, LocalizationService>()
                .AddSingleton<IThemeManager, ThemeManager>()
                .AddSingleton<ISettingsService, SettingsService>()
                .AddSingleton<IAppSettings, AppSettings>()
                .AddSingleton<IFrameAccessor, FrameAccessor>()
                .AddSingleton<INavigationService, NavigationService>();

            RegisterViewModels(services);
        }

        private static void RegisterViewModels(IServiceCollection services)
        {
            services
                .AddSingleton<AppShellViewModel>()
                .AddSingleton<MainViewModel>()
                .AddSingleton<BuyMeCoffeeViewModel>()
                .AddSingleton<CountdownDetailViewModel>()
                .AddSingleton<CountdownEditorViewModel>();
        }

        public static void Configure(IServiceProvider serviceProvider)
        {
            var navigationService = serviceProvider.GetRequiredService<INavigationService>();
            navigationService
                .RegisterForNavigation<MainViewModel, MainView>()
                .RegisterForNavigation<CountdownDetailViewModel, CountdownDetailView>()
                .RegisterForNavigation<CountdownEditorViewModel, CountdownEditorView>()
                .RegisterForNavigation<BuyMeCoffeeViewModel, BuyMeCoffeeView>()
                .RegisterForNavigation<AboutViewModel, AboutView>();
        }
    }
}
