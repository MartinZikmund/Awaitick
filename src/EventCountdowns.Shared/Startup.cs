using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventCountdowns.Core.Services.Dialogs;
using EventCountdowns.Core.Services.Navigation;
using EventCountdowns.Core.ViewModels;
using EventCountdowns.Views;
using Microsoft.Extensions.DependencyInjection;

namespace EventCountdowns
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton<INavigationService, NavigationService>();
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
