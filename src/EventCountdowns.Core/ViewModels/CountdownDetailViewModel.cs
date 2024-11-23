using System.Globalization;
using EventCountdowns.Core.Models;
using EventCountdowns.Core.Services.Countdowns;
using EventCountdowns.Core.Services.Data;
using EventCountdowns.Core.Services.EventCountdownManager;
using EventCountdowns.Core.Services.ScheduledNotification;
using EventCountdowns.Core.Services.Tiles;
using EventCountdowns.Services.Navigation;
using EventCountdowns.ViewModels;

namespace EventCountdowns.Core.ViewModels;

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

	[ObservableProperty]
	private CountdownViewModel _event = null!;

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

	public override async void ViewNavigatedTo(object? parameter)
	{
		if (parameter is not NavigationModel navigationModel)
		{
			throw new ArgumentException("Parameter must be CountdownDetailViewModel.NavigationModel.", nameof(parameter));
		}

		var eventInfo = await _dataService.GetCountdownAsync(navigationModel.CountdownId);
		if (eventInfo is null)
		{
			throw new InvalidOperationException("This event does not exist");
		}

		Event = new CountdownViewModel(eventInfo, _countdownsManager);

		TargetDateString = Event.TargetDateTime.ToString("f", CultureInfo.CurrentCulture);
		IsTilePinned = _tileService.IsCountdownPinned(Event.Id);
		_scheduledNotificationService.SuppressCountdownNotification(Event.Model);
	}

	[RelayCommand]
	private async Task DeleteAsync()
	{
		var result = await _countdownsManager.DeleteAsync(Event);
		if (result)
		{
			_navigationService.GoBack();
		}
	}

	[RelayCommand]
	private void Edit() => _countdownsManager.GoToEdit(Event);

	[RelayCommand]
	private async Task ShareAsync() => await _countdownsManager.ShareAsync(Event);

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
		IsTilePinned = await _tileService.PinCountdownAsync(Event.Model);
		_tileService.UpdateCountdownTile(Event.Model);
		_tileService.ScheduleCountdownNotification(Event.Model);
	}

	[RelayCommand]
	private async Task UnPinAsync()
	{
		var unpinSuccessful = await _tileService.UnpinCountdownAsync(Event.Model);
		IsTilePinned = !unpinSuccessful;
	}

	public void UpdateCountdowns()
	{
		Event?.UpdateBindings();
	}
}
