using System.Globalization;
using EventCountdowns.Core.Models;
using EventCountdowns.Core.Services.ConfirmationDialog;
using EventCountdowns.Core.Services.Data;
using EventCountdowns.Core.Services.EventCountdownManager;
using EventCountdowns.Core.Services.ScheduledNotification;
using EventCountdowns.Core.Services.Share;
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
	private readonly ISystemSharingService _sharingService;
	private readonly IConfirmationDialogService _confirmationDialogService;
	private readonly IStringLocalizer _localizationService;

	private EventCountdownObservable? _eventCountdown;
	private bool _isTilePinned;
	private string _targetDateString = "";

	public CountdownDetailViewModel(
		IEventCountdownManager eventCountdownManager, INavigationService navigationService, IDataService dataService, ITileService tileService, IScheduledNotificationService scheduledNotificationService, ISystemSharingService sharingService, IConfirmationDialogService confirmationDialogService, IStringLocalizer localizationService)
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

		EventCountdown = new EventCountdownObservable(await _dataService.GetCountdownAsync(navigationModel.CountdownId));
		if (EventCountdown != null)
		{
			TargetDateString = EventCountdown.TargetDateTime.ToString("f", CultureInfo.CurrentCulture);
			IsTilePinned = _tileService.IsCountdownPinned(EventCountdown.Id);
			_scheduledNotificationService.SuppressCountdownNotification(EventCountdown.Model);
		}
	}

	public EventCountdownObservable EventCountdown
	{
		get => _eventCountdown;
		set => SetProperty(ref _eventCountdown, value);
	}

	[RelayCommand]
	private async void DeletePrompt()
	{
		//show delete dialog
		await
			_confirmationDialogService.ShowAsync(_localizationService.GetString("ConfirmDelete"),
				string.Format(CultureInfo.CurrentCulture, _localizationService.GetString("AreYouSureDeleteTextFormat"), EventCountdown.Name), DeleteConfirmed,
				() => { });
	}

	private async void DeleteConfirmed()
	{
		await _eventCountdownManager.DeleteCountdownAsync(EventCountdown.Model);
		_navigationService.GoBack();
	}

	[RelayCommand]
	private void Edit()
	{
		_navigationService.Navigate<CountdownEditorViewModel>(CountdownEditorViewModel.NavigationModel.CreateEdit(EventCountdown.Id));
	}

	[RelayCommand]
	private void Share()
	{
		string sharedText = "";
		if (EventCountdown.Finished)
		{
			sharedText = string.Format(CultureInfo.CurrentCulture, _localizationService.GetString("SharingFinishedEventFormatString"),
				EventCountdown.CelebrationMessage, _localizationService.GetString("AppSocialHandle"));
		}
		else
		{
			sharedText = string.Format(CultureInfo.CurrentCulture, _localizationService.GetString("SharingFormatString"),
				EventCountdown.Name,
				EventCountdown.DaysLeft,
				EventCountdown.HoursLeft,
				EventCountdown.MinutesLeft,
				EventCountdown.TargetDateTime.ToString("g", CultureInfo.CurrentCulture),
				_localizationService.GetString("AppSocialHandle"));
		}
		_sharingService.ShareTextAsync(sharedText);
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

	public void UpdateCountdowns()
	{
		EventCountdown?.UpdateBindings();
	}
}
