using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Messaging;
using Awaitick.Core.Messages;
using Awaitick.Core.Models;
using Awaitick.Core.Services.Countdowns;
using Awaitick.Core.Services.Data;
using Awaitick.Core.Services.Mail;
using Awaitick.Core.Services.ScheduledNotification;
using Awaitick.Core.Services.Settings;
using Awaitick.Core.Services.StoreLauncher;
using Awaitick.Core.Services.Tiles;
using Awaitick.Services.Navigation;
using Awaitick.Services.Store;
using Awaitick.ViewModels;

namespace Awaitick.Core.ViewModels;

public partial class MainViewModel : PageViewModel
{
	private readonly IDataService _dataService;
	private readonly ITileService? _tileService;
	private readonly IMailService _mailService;
	private readonly IStoreService _storeService;
	private readonly ICountdownsManager _countdownsManager;
	private readonly IScheduledNotificationService _scheduledNotificationService;
	private readonly IStoreLauncherService _storeLauncherService;
	private readonly IAppPreferences _appSettings;
	private readonly INavigationService _navigationService;
	private readonly IMessenger _messenger;

	private bool _isFirstNavigation = true;

	public MainViewModel(
		IDataService dataService,
		ITileService? tileService,
		IMailService mailService,
		IStoreService storeService,
		ICountdownsManager countdownsManager,
		IScheduledNotificationService scheduledNotificationService,
		IStoreLauncherService storeLauncherService,
		INavigationService navigationService,
		IMessenger messenger,
		IAppPreferences appSettings) :
		base(navigationService)
	{
		_dataService = dataService;
		_tileService = tileService;
		_mailService = mailService;
		_storeService = storeService;
		_countdownsManager = countdownsManager;
		_scheduledNotificationService = scheduledNotificationService;
		_storeLauncherService = storeLauncherService;
		_navigationService = navigationService;
		_messenger = messenger;
		_appSettings = appSettings;
		_messenger.Register<CountdownDeletedMessage>(this, CountdownDeletedHandler);
	}

	private static void CountdownDeletedHandler(object recipient, CountdownDeletedMessage message)
	{
		var viewModel = recipient as MainViewModel;
		if (viewModel != null)
		{
			var countdown = viewModel.Awaitick.FirstOrDefault(c => c.Id == message.Id);
			if (countdown != null)
			{
				viewModel.Awaitick.Remove(countdown);
				viewModel.OnPropertyChanged(nameof(HasAnyEvents));
			}
		}
	}

	protected override async void ViewNavigatedTo(object? parameter)
	{
		IsLoading = true;

		HasProLicense = await _storeService.HasProAsync();

		//load countdowns
		var countdowns = await _dataService.GetCountdownsAsync();
		var newCountdowns = new ObservableCollection<CountdownViewModel>();
		foreach (var countdown in countdowns)
		{
			newCountdowns.Add(new CountdownViewModel(countdown, _countdownsManager));
		}
		Awaitick = newCountdowns;
		OnPropertyChanged(nameof(HasAnyEvents));

		IsLoading = false;
		_scheduledNotificationService.UnSuppressAllCountdownNotifications();

		if (Awaitick.Count == 0 && _isFirstNavigation)
		{
			Add();
		}

		_isFirstNavigation = false;
	}

	[ObservableProperty]
	public partial ObservableCollection<CountdownViewModel> Awaitick { get; private set; } = new();

	public bool HasAnyEvents => Awaitick.Count > 0;

	[ObservableProperty]
	public partial bool HasProLicense { get; set; } = true;

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
	public void GoToGetPro() => _navigationService.Navigate<GetProViewModel>();

	[RelayCommand]
	private void RootTap()
	{
		if (Awaitick.Count == 0)
		{
			Add();
		}
	}

	[RelayCommand]
	private void Add() => _navigationService.Navigate<CountdownEditorViewModel>(new CountdownEditorViewModel.NavigationModel() { Mode = CountdownEditorViewModel.EditorMode.Add });

	[RelayCommand]
	private void ShowCountdown(CountdownViewModel? eventCountdown)
	{
		if (eventCountdown != null)
		{
			_navigationService.Navigate<CountdownDetailViewModel>(new CountdownDetailViewModel.NavigationModel(eventCountdown.Id));
		}
	}

	[RelayCommand]
	private void BuyMeCoffee() => _navigationService.Navigate<BuyMeCoffeeViewModel>();

	[RelayCommand]
	private async Task RateAppAsync()
	{
		_appSettings.OfferUserRating = false;
		await _storeLauncherService.RateAppAsync();
		//TODO:Track rating
	}

	[RelayCommand]
	private async Task SendFeedbackAsync() => await _mailService.ComposeMailAsync("Feedback", "awaitickapp@sphereline.com");

	[RelayCommand]
	private void GoToSettings() => _navigationService.Navigate<SettingsViewModel>();

	public void UpdateCountdowns()
	{
		foreach (var countdown in Awaitick)
		{
			countdown?.UpdateBindings();
		}
	}
}
