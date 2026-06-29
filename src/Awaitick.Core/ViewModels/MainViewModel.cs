using System.Collections.ObjectModel;
using Awaitick.Core.Messages;
using Awaitick.Core.Models;
using Awaitick.Core.Services.Countdowns;
using Awaitick.Core.Services.Data;
using Awaitick.Core.Services.DeepLink;
using Awaitick.Core.Services.Mail;
using Awaitick.Core.Services.ScheduledNotification;
using Awaitick.Core.Services.Settings;
using Awaitick.Core.Services.StoreLauncher;
using Awaitick.Core.Services.Tiles;
using Awaitick.Services.Navigation;
using Awaitick.Services.Store;
using Awaitick.ViewModels;
using CommunityToolkit.Mvvm.Messaging;

namespace Awaitick.Core.ViewModels;

public partial class MainViewModel : PageViewModel
{
	private const int MaxFreeCountdowns = 3;

	private readonly IDataService _dataService;
	private readonly ITileService? _tileService;
	private readonly IMailService _mailService;
	private readonly IStoreService _storeService;
	private readonly ICountdownsManager _countdownsManager;
	private readonly IScheduledNotificationService _scheduledNotificationService;
	private readonly IStoreLauncherService _storeLauncherService;
	private readonly IDeepLinkService _deepLinkService;
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
		IDeepLinkService deepLinkService,
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
		_deepLinkService = deepLinkService;
		_navigationService = navigationService;
		_messenger = messenger;
		_appSettings = appSettings;
		_messenger.Register<CountdownDeletedMessage>(this, CountdownDeletedHandler);
		_messenger.Register<CountdownUpdatedMessage>(this, CountdownUpdatedHandler);
		_messenger.Register<CountdownAddedMessage>(this, CountdownAddedHandler);
		_messenger.Register<DeepLinkReceivedMessage>(this, DeepLinkReceivedHandler);
	}

	private static async void CountdownAddedHandler(object recipient, CountdownAddedMessage message)
	{
		if (recipient is not MainViewModel viewModel)
		{
			return;
		}

		if (viewModel.Awaitick.Any(c => c.Id == message.Id))
		{
			return;
		}

		var model = await viewModel._dataService.GetCountdownAsync(message.Id);
		if (model is null)
		{
			return;
		}

		CountdownViewModel newCountdown = new(model, viewModel._countdownsManager);

		// Insert preserving ascending ordering by TargetDateTime.
		var index = 0;
		while (index < viewModel.Awaitick.Count && viewModel.Awaitick[index].TargetDateTime <= newCountdown.TargetDateTime)
		{
			index++;
		}

		viewModel.Awaitick.Insert(index, newCountdown);
		viewModel.OnPropertyChanged(nameof(HasAnyEvents));
	}

	private static void CountdownUpdatedHandler(object recipient, CountdownUpdatedMessage message)
	{
		if (recipient is MainViewModel viewModel)
		{
			_ = viewModel.HandleCountdownUpdatedAsync(message.Id);
		}
	}

	private async Task HandleCountdownUpdatedAsync(string id)
	{
		var countdown = Awaitick.FirstOrDefault(c => c.Id == id);
		if (countdown is null)
		{
			return;
		}

		// Re-read the freshly persisted model; the data service returns new
		// EventCountdown instances, so the tile VM still holds the stale one.
		var freshModel = await _dataService.GetCountdownAsync(id);
		if (freshModel is null)
		{
			return;
		}

		countdown.UpdateModel(freshModel);
	}

	private static void DeepLinkReceivedHandler(object recipient, DeepLinkReceivedMessage message)
	{
		var viewModel = recipient as MainViewModel;
		if (viewModel != null)
		{
			var pendingCountdownId = viewModel._deepLinkService.ConsumePendingNavigation();
			if (!string.IsNullOrEmpty(pendingCountdownId))
			{
				viewModel._navigationService.Navigate<CountdownDetailViewModel>(
					new CountdownDetailViewModel.NavigationModel(pendingCountdownId));
			}
		}
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

	public override async void ViewNavigatedTo(object? parameter)
	{
		IsLoading = true;

		//load countdowns
		var countdowns = await _dataService.GetCountdownsAsync();
		var newIds = countdowns.Select(c => c.Id).ToList();
		var existingIds = Awaitick.Select(c => c.Id).ToList();

		if (!existingIds.SequenceEqual(newIds))
		{
			// Collection changed (items added, removed, or reordered) — rebuild,
			// but reuse existing CountdownViewModel instances so ItemsView keeps
			// their containers and loaded BitmapImages.
			var existingById = Awaitick.ToDictionary(c => c.Id);
			var newCountdowns = new ObservableCollection<CountdownViewModel>();
			foreach (var countdown in countdowns)
			{
				if (existingById.TryGetValue(countdown.Id, out var existing))
				{
					newCountdowns.Add(existing);
				}
				else
				{
					newCountdowns.Add(new CountdownViewModel(countdown, _countdownsManager));
				}
			}
			Awaitick = newCountdowns;
			OnPropertyChanged(nameof(HasAnyEvents));
		}

		HasProLicense = await _storeService.HasProAsync();

		IsLoading = false;
		_scheduledNotificationService.UnSuppressAllCountdownNotifications();

		// Check for pending deep link navigation (e.g., from notification tap)
		var pendingCountdownId = _deepLinkService.ConsumePendingNavigation();
		if (!string.IsNullOrEmpty(pendingCountdownId))
		{
			_isFirstNavigation = false;
			// Navigate to the countdown detail view
			_navigationService.Navigate<CountdownDetailViewModel>(
				new CountdownDetailViewModel.NavigationModel(pendingCountdownId));
			return; // Skip auto-add for first navigation if deep linking
		}

		if (Awaitick.Count == 0 && _isFirstNavigation)
		{
			NavigateToNewEditorDirect();
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
	private void Add()
	{
		if (!HasProLicense && Awaitick.Count >= MaxFreeCountdowns)
		{
			_navigationService.Navigate<GetProViewModel>();
			return;
		}

		_navigationService.Navigate<NewCountdownViewModel>();
	}

	private void NavigateToNewEditorDirect()
	{
		_navigationService.Navigate<CountdownEditorViewModel>(
			CountdownEditorViewModel.NavigationModel.CreateAdd());
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
