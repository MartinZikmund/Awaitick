using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using EventCountdowns.Core.Models;
using EventCountdowns.Core.Services.Data;
using EventCountdowns.Core.Services.InAppPurchases;
using EventCountdowns.Core.Services.Mail;
using EventCountdowns.Core.Services.ScheduledNotification;
using EventCountdowns.Core.Services.Settings;
using EventCountdowns.Core.Services.StoreLauncher;
using EventCountdowns.Core.Services.Tiles;

namespace EventCountdowns.Core.ViewModels
{
    public class MainViewModel : ViewModel
    {
        private readonly IDataService _dataService;
        private readonly ITileService? _tileService;
        private readonly IInAppPurchaseService _inAppPurchaseService;
        private readonly IMailService _mailService;
        private readonly IScheduledNotificationService _scheduledNotificationService;
        private readonly IStoreLauncherService _storeLauncherService;
        private readonly IAppSettings _appSettings;

        public MainViewModel(
            IDataService dataService, 
            ITileService? tileService, 
            IInAppPurchaseService inAppPurchaseService, 
            IMailService mailService, 
            IScheduledNotificationService scheduledNotificationService, 
            IStoreLauncherService storeLauncherService, 
            IAppSettings appSettings)
        {
            _dataService = dataService;
            _tileService = tileService;
            _inAppPurchaseService = inAppPurchaseService;
            _mailService = mailService;
            _scheduledNotificationService = scheduledNotificationService;
            _storeLauncherService = storeLauncherService;
            _appSettings = appSettings;
        }

        public override async Task LoadAsync()
        {
            IsLoading = true;
            //load countdowns
            var countdowns = await _dataService.GetCountdownsAsync();
            EventCountdowns.Clear();
            foreach (var countdown in countdowns)
            {
                EventCountdowns.Add(new EventCountdownObservable(countdown));
            }
            IsLoading = false;
            _scheduledNotificationService.UnSuppressAllCountdownNotifications();
            if (_appSettings.LaunchCount % 4 == 0 && !_inAppPurchaseService.HasUserAnyProduct())
            {
                ShowCoffee = true;
            }
        }

        public ObservableCollection<EventCountdownObservable> EventCountdowns { get; } = new ObservableCollection<EventCountdownObservable>();

        private bool _isLoading = false;

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private bool _showCoffee = false;

        public bool ShowCoffee
        {
            get => _showCoffee;
            set => SetProperty(ref _showCoffee, value);
        }

        public ICommand AddCommand => GetOrCreateCommand(Add);

        private void Add()
        {
            Navigation.Navigate<CountdownEditorViewModel>();
        }

        public ICommand ShowCountdownCommand => GetOrCreateCommand<EventCountdownObservable>(ShowCountdown);

        private void ShowCountdown(EventCountdownObservable? eventCountdown)
        {
            if (eventCountdown != null)
            {
                Navigation.Navigate<CountdownDetailViewModel>(new CountdownDetailViewModel.NavigationModel(eventCountdown.Id));
            }
        }

        public ICommand AboutAppCommand => GetOrCreateCommand(AboutApp);

        private void AboutApp()
        {
            Navigation.Navigate<AboutViewModel>();
        }

        public ICommand BuyMeCoffeeCommand => GetOrCreateCommand(BuyMeCoffee);

        private void BuyMeCoffee()
        {
            Navigation.Navigate<BuyMeCoffeeViewModel>();
        }

        public ICommand RateAppCommand => GetOrCreateCommand(RateApp);

        private async void RateApp()
        {
            _appSettings.OfferUserRating = false;
            await _storeLauncherService.RateAppAsync();
            //TODO:Track rating
        }

        public ICommand SendFeedbackCommand => GetOrCreateCommand(SendFeedback);

        private async void SendFeedback()
        {
            await _mailService.ComposeMailAsync("Feedback", "eventcountdownsapp@sphereline.com");
        }

        public void UpdateCountdowns()
        {
            foreach (var countdown in EventCountdowns)
            {
                countdown?.UpdateBindings();
            }
        }
    }
}
