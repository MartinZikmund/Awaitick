using System.Collections.ObjectModel;
using EventCountdowns.Core.Models;
using EventCountdowns.Core.Services;
using EventCountdowns.Core.Services.Data;
using EventCountdowns.Core.Services.InAppPurchases;
using EventCountdowns.Core.Services.Mail;
using EventCountdowns.Core.Services.ScheduledNotification;
using EventCountdowns.Core.Services.Settings;
using EventCountdowns.Core.Services.StoreLauncher;
using EventCountdowns.Core.Services.Tiles;
using EventCountdowns.Services.Navigation;
using EventCountdowns.ViewModels;

namespace EventCountdowns.Core.ViewModels;

public partial class MainViewModel : PageViewModel
{
	private readonly IDataService _dataService;
	private readonly ITileService? _tileService;
	private readonly IInAppPurchaseService _inAppPurchaseService;
	private readonly IMailService _mailService;
	private readonly IScheduledNotificationService _scheduledNotificationService;
	private readonly IStoreLauncherService _storeLauncherService;
	private readonly IAppSettings _appSettings;
	private readonly INavigationService _navigationService;

	public MainViewModel(
		IDataService dataService,
		ITileService? tileService,
		IInAppPurchaseService inAppPurchaseService,
		IMailService mailService,
		IScheduledNotificationService scheduledNotificationService,
		IStoreLauncherService storeLauncherService,
		INavigationService navigationService,
		IAppSettings appSettings) :
		base(navigationService)
	{
		_dataService = dataService;
		_tileService = tileService;
		_inAppPurchaseService = inAppPurchaseService;
		_mailService = mailService;
		_scheduledNotificationService = scheduledNotificationService;
		_storeLauncherService = storeLauncherService;
		_navigationService = navigationService;
		_appSettings = appSettings;
	}

	public override async void ViewNavigatedTo(object? parameter)
	{
		IsLoading = true;
		//load countdowns
		var countdowns = await _dataService.GetCountdownsAsync();
		EventCountdowns.Clear();
		foreach (var countdown in countdowns)
		{
			EventCountdowns.Add(new CountdownViewModel(countdown));
		}
		OnPropertyChanged(nameof(HasAnyEvents));

		IsLoading = false;
		_scheduledNotificationService.UnSuppressAllCountdownNotifications();
		if (_appSettings.LaunchCount % 4 == 0 && !_inAppPurchaseService.HasUserAnyProduct())
		{
			ShowCoffee = true;
		}
	}

	public ObservableCollection<CountdownViewModel> EventCountdowns { get; } = new ObservableCollection<CountdownViewModel>();

	public bool HasAnyEvents => EventCountdowns.Count > 0;

	private bool _isLoading;

	public bool IsLoading
	{
		get => _isLoading;
		set => SetProperty(ref _isLoading, value);
	}

	private bool _showCoffee;

	public bool ShowCoffee
	{
		get => _showCoffee;
		set => SetProperty(ref _showCoffee, value);
	}

	[RelayCommand]
	private void RootTap()
	{
		if (EventCountdowns.Count == 0)
		{
			Add();
		}
	}

	[RelayCommand]
	private void Add()
	{
		_navigationService.Navigate<CountdownEditorViewModel>(new CountdownEditorViewModel.NavigationModel() { Mode = CountdownEditorViewModel.EditorMode.Add });
	}

	[RelayCommand]
	private void ShowCountdown(CountdownViewModel? eventCountdown)
	{
		if (eventCountdown != null)
		{
			_navigationService.Navigate<CountdownDetailViewModel>(new CountdownDetailViewModel.NavigationModel(eventCountdown.Id));
		}
	}

	[RelayCommand]
	private void AboutApp()
	{
		_navigationService.Navigate<AboutViewModel>();
	}

	[RelayCommand]
	private void BuyMeCoffee()
	{
		_navigationService.Navigate<BuyMeCoffeeViewModel>();
	}

	[RelayCommand]
	private async Task RateAppAsync()
	{
		_appSettings.OfferUserRating = false;
		await _storeLauncherService.RateAppAsync();
		//TODO:Track rating
	}

	[RelayCommand]
	private async Task SendFeedbackAsync()
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
