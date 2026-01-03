using System.Globalization;
using Awaitick.Core.Models;
using Awaitick.Core.Services.Countdowns;
using Awaitick.Core.Services.Data;
using Awaitick.Core.Services.EventCountdownManager;
using Awaitick.Core.Services.ScheduledNotification;
using Awaitick.Core.Services.Tiles;
using Awaitick.Services.Navigation;
using Awaitick.ViewModels;

namespace Awaitick.Core.ViewModels;

public partial class CountdownDetailViewModel : PageViewModel
{
	public class NavigationModel
	{
		public NavigationModel()
		{
		}

		public NavigationModel(string countdownId)
		{
			CountdownId = countdownId;
		}

		public string CountdownId { get; set; } = string.Empty;
	}

	private readonly ICountdownsDataService _eventCountdownManager;
	private readonly INavigationService _navigationService;
	private readonly IDataService _dataService;
	private readonly ITileService _tileService;
	private readonly IScheduledNotificationService _scheduledNotificationService;
	private readonly ICountdownsManager _countdownsManager;
	private readonly IStringLocalizer _localizationService;

	private bool _isTilePinned;
	private string _targetDateString = "";

	public CountdownDetailViewModel(
		ICountdownsDataService eventCountdownManager,
		ICountdownsManager countdownsManager,
		INavigationService navigationService,
		IDataService dataService,
		ITileService tileService,
		IScheduledNotificationService scheduledNotificationService,
		IStringLocalizer localizationService) :
		base(navigationService)
	{
		_eventCountdownManager = eventCountdownManager;
		_navigationService = navigationService;
		_dataService = dataService;
		_tileService = tileService;
		_scheduledNotificationService = scheduledNotificationService;
		_countdownsManager = countdownsManager;
		_localizationService = localizationService;
	}

	[ObservableProperty]
	public partial CountdownViewModel EventCountdown { get; private set; }

	public override async void ViewNavigatedTo(object? parameter)
	{
		try
		{
			if (parameter is not NavigationModel navigationModel)
			{
				System.Diagnostics.Debug.WriteLine($"[CountdownDetailViewModel] Invalid navigation parameter: expected NavigationModel, got {parameter?.GetType().Name ?? "null"}");
				_navigationService.GoBack();
				return;
			}

			var eventInfo = await _dataService.GetCountdownAsync(navigationModel.CountdownId);
			if (eventInfo is null)
			{
				System.Diagnostics.Debug.WriteLine($"[CountdownDetailViewModel] Countdown not found: {navigationModel.CountdownId}");
				_navigationService.GoBack();
				return;
			}

			EventCountdown = new CountdownViewModel(eventInfo, _countdownsManager);

			IsTilePinned = _tileService.IsCountdownPinned(EventCountdown.Id);
			_scheduledNotificationService.SuppressCountdownNotification(EventCountdown.Model);
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"[CountdownDetailViewModel] Error in ViewNavigatedTo: {ex}");
			_navigationService.GoBack();
		}
	}

	[RelayCommand]
	private async Task DeleteAsync()
	{
		var result = await _countdownsManager.DeleteAsync(EventCountdown);
		if (result)
		{
			_navigationService.GoBack();
		}
	}

	[RelayCommand]
	private void Edit() => _countdownsManager.GoToEdit(EventCountdown);

	[RelayCommand]
	private async Task ShareAsync() => await _countdownsManager.ShareAsync(EventCountdown);

	public string TargetDateString
	{
		get => _targetDateString;
		set => SetProperty(ref _targetDateString, value);
	}

	public bool IsTilePinned
	{
		get => _isTilePinned;
		set => SetProperty(ref _isTilePinned, value);
	}

	[RelayCommand]
	private async Task PinAsync()
	{
		IsTilePinned = await _tileService.PinCountdownAsync(EventCountdown.Model);
		_tileService.UpdateCountdownTile(EventCountdown.Model);
		_tileService.ScheduleCountdownNotification(EventCountdown.Model);
	}

	[RelayCommand]
	private async Task UnPinAsync()
	{
		var unpinSuccessful = await _tileService.UnpinCountdownAsync(EventCountdown.Model);
		IsTilePinned = !unpinSuccessful;
	}

	public void UpdateCountdowns() => EventCountdown?.UpdateBindings();
}
