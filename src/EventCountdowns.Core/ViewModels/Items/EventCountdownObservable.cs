using System.Globalization;
using EventCountdowns.Core.Services;
using EventCountdowns.Core.Services.ConfirmationDialog;
using EventCountdowns.Core.Services.EventCountdownManager;
using EventCountdowns.Core.ViewModels;
using EventCountdowns.Services.Navigation;

namespace EventCountdowns.Core.Models;

public partial class EventCountdownViewModel : ObservableObject
{
	private readonly EventCountdown _eventCountdown;
	private readonly IEventCountdownManager _eventCountdownManager;
	private readonly IEventSharingService _sharingService;
	private readonly IStringLocalizer _localizer;
	private readonly INavigationService _navigationService;
	private readonly IConfirmationDialogService _confirmationDialogService;

	public EventCountdownViewModel(
		EventCountdown eventCountdown,
		IEventCountdownManager eventCountdownManager,
		IEventSharingService sharingService,
		IStringLocalizer localizer,
		INavigationService navigationService,
		IConfirmationDialogService confirmationDialogService)
	{
		_eventCountdown = eventCountdown ?? throw new ArgumentNullException(nameof(eventCountdown));
		_eventCountdownManager = eventCountdownManager;
		_sharingService = sharingService ?? throw new ArgumentNullException(nameof(sharingService));
		_localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
		_navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
		_confirmationDialogService = confirmationDialogService ?? throw new ArgumentNullException(nameof(confirmationDialogService));
	}

	public EventCountdown Model => _eventCountdown;

	public string Id => _eventCountdown.Id;

	public string Name => _eventCountdown.Name;

	public Uri? BackgroundImage => _eventCountdown.BackgroundImageUri;

	public bool Finished => _eventCountdown.TargetDateTime < DateTimeOffset.Now;

	public TimeSpan TimeLeft => _eventCountdown.TargetDateTime - DateTimeOffset.Now;

	public int DaysLeft => TimeLeft.Days;

	public int HoursLeft => TimeLeft.Hours;

	public int MinutesLeft => TimeLeft.Minutes;

	public int SecondsLeft => TimeLeft.Seconds;

	public DateTimeOffset TargetDateTime => _eventCountdown.TargetDateTime;

	public string CelebrationMessage => _eventCountdown.CelebrationMessage;

	[RelayCommand]
	public void Share() => _sharingService.ShareAsync(this);

	[RelayCommand]
	public void Edit() => _navigationService.Navigate<CountdownEditorViewModel>(Id);

	[RelayCommand]
	private async Task DeletePrompt()
	{
		//show delete dialog
		await _confirmationDialogService.ShowAsync(_localizer.GetString("ConfirmDelete"),
			string.Format(CultureInfo.CurrentCulture, _localizer.GetString("AreYouSureDeleteTextFormat"), Name), DeleteConfirmed,
			() => { });
	}

	private async void DeleteConfirmed()
	{
		await _eventCountdownManager.DeleteCountdownAsync(Model);
		_navigationService.GoBack();
	}

	public void UpdateBindings()
	{
		OnPropertyChanged(nameof(DaysLeft));
		OnPropertyChanged(nameof(HoursLeft));
		OnPropertyChanged(nameof(MinutesLeft));
		OnPropertyChanged(nameof(SecondsLeft));
		OnPropertyChanged(nameof(TimeLeft));
		OnPropertyChanged(nameof(Finished));
	}
}
