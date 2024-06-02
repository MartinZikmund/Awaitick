using System.Globalization;
using EventCountdowns.Core.Models;
using EventCountdowns.Core.Services;
using EventCountdowns.Core.Services.ConfirmationDialog;
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

	private readonly IEventCountdownManager _eventCountdownManager;
	private readonly INavigationService _navigationService;
	private readonly IDataService _dataService;
	private readonly ITileService _tileService;
	private readonly IScheduledNotificationService _scheduledNotificationService;
	private readonly IEventSharingService _sharingService;
	private readonly IConfirmationDialogService _confirmationDialogService;
	private readonly IStringLocalizer _localizationService;

	[ObservableProperty]
	private EventCountdownObservable _event = null!;

	private bool _isTilePinned;
	private string _targetDateString = "";

	public CountdownDetailViewModel(
		IEventCountdownManager eventCountdownManager,
		INavigationService navigationService,
		IDataService dataService,
		ITileService tileService,
		IScheduledNotificationService scheduledNotificationService,
		IEventSharingService sharingService,
		IConfirmationDialogService confirmationDialogService,
		IStringLocalizer localizationService) :
		base(navigationService)
	{
		_eventCountdownManager = eventCountdownManager;
		_navigationService = navigationService;
		_dataService = dataService;
		_tileService = tileService;
		_scheduledNotificationService = scheduledNotificationService;
		_sharingService = sharingService;
		_confirmationDialogService = confirmationDialogService;
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

		Event = new EventCountdownObservable(eventInfo, _sharingService);

		TargetDateString = Event.TargetDateTime.ToString("f", CultureInfo.CurrentCulture);
		IsTilePinned = _tileService.IsCountdownPinned(Event.Id);
		_scheduledNotificationService.SuppressCountdownNotification(Event.Model);
	}

	[RelayCommand]
	private async Task DeletePrompt()
	{
		//show delete dialog
		await _confirmationDialogService.ShowAsync(_localizationService.GetString("ConfirmDelete"),
			string.Format(CultureInfo.CurrentCulture, _localizationService.GetString("AreYouSureDeleteTextFormat"), Event.Name), DeleteConfirmed,
			() => { });
	}

	private async void DeleteConfirmed()
	{
		await _eventCountdownManager.DeleteCountdownAsync(Event.Model);
		_navigationService.GoBack();
	}

	[RelayCommand]
	private void Edit()
	{
		_navigationService.Navigate<CountdownEditorViewModel>(CountdownEditorViewModel.NavigationModel.CreateEdit(Event.Id));
	}

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
