using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using EventCountdowns.Core.Models;
using EventCountdowns.Core.Services;
using EventCountdowns.Core.Services.Data;
using EventCountdowns.Core.Services.InAppPurchases;
using EventCountdowns.Core.Services.Mail;
using EventCountdowns.Core.Services.ScheduledNotification;
using EventCountdowns.Core.Services.Settings;
using EventCountdowns.Core.Services.StoreLauncher;
using EventCountdowns.Core.Services.TelemetryService;
using EventCountdowns.Core.Services.Tile;

namespace EventCountdowns.Core.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly IDataService _dataService;
        private readonly ITileService _tileService;
        private readonly IInAppPurchaseService _inAppPurchaseService;
        private readonly IMailService _mailService;
        private readonly IScheduledNotificationService _scheduledNotificationService;
        private readonly IStoreLauncherService _storeLauncherService;
        private readonly IAppSettings _appSettings;

        public MainViewModel(IDataService dataService, ITileService tileService, IInAppPurchaseService inAppPurchaseService, IMailService mailService, IScheduledNotificationService scheduledNotificationService, IStoreLauncherService storeLauncherService, IAppSettings appSettings)
        {
            _dataService = dataService;
            _tileService = tileService;
            _inAppPurchaseService = inAppPurchaseService;
            _mailService = mailService;
            _scheduledNotificationService = scheduledNotificationService;
            _storeLauncherService = storeLauncherService;
            _appSettings = appSettings;
        }

        public async void Init()
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
            get { return _isLoading; }
            set { SetProperty(ref _isLoading, value); }
        }

        private bool _showCoffee = false;

        public bool ShowCoffee
        {
            get
            {
                return _showCoffee;
            }
            set { SetProperty(ref _showCoffee, value); }
        }

        private ICommand _addCommand = null;

        public ICommand AddCommand => _addCommand ?? (_addCommand = new MvxCommand(Add));

        private void Add()
        {
            ShowViewModel<CountdownEditorViewModel>();
        }

        private ICommand _showCountdownCommand = null;

        public ICommand ShowCountdownCommand
            => _showCountdownCommand ?? (_showCountdownCommand = new MvxCommand<EventCountdownObservable>(ShowCountdown));

        private void ShowCountdown(EventCountdownObservable eventCountdown)
        {
            if (eventCountdown != null)
            {
                ShowViewModel<CountdownDetailViewModel>(new CountdownDetailViewModel.NavigationModel(eventCountdown.Id));
            }
        }

        private ICommand _aboutAppCommand = null;
        public ICommand AboutAppCommand => _aboutAppCommand ?? (_aboutAppCommand = new MvxCommand(AboutApp));

        private void AboutApp()
        {
            ShowViewModel<AboutViewModel>();
        }

        private ICommand _buyMeCoffeeCommand = null;

        public ICommand BuyMeCoffeeCommand
            => _buyMeCoffeeCommand ?? (_buyMeCoffeeCommand = new MvxCommand(BuyMeCoffee));

        private void BuyMeCoffee()
        {
            ShowViewModel<BuyMeCoffeeViewModel>();
        }

        private ICommand _rateAppCommand = null;

        public ICommand RateAppCommand => _rateAppCommand ?? (_rateAppCommand
            = new MvxCommand(RateApp));

        private async void RateApp()
        {
            _appSettings.OfferUserRating = false;
            await _storeLauncherService.RateAppAsync();
            Mvx.Resolve<ITelemetryService>().TrackEvent("UserRatedApp");
        }

        private ICommand _sendFeedbackCommand = null;

        public ICommand SendFeedbackCommand
            => _sendFeedbackCommand ?? (_sendFeedbackCommand = new MvxCommand(SendFeedback));

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
