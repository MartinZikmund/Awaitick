using System.Globalization;
using EventCountdowns.Core.Services;

namespace EventCountdowns.Core.Models;

public partial class EventCountdownObservable : ObservableObject
{
	private readonly EventCountdown _eventCountdown;
	private readonly IEventSharingService _sharingService;

	public EventCountdownObservable(EventCountdown eventCountdown, IEventSharingService sharingService)
	{
		_eventCountdown = eventCountdown ?? throw new ArgumentNullException(nameof(eventCountdown));
		_sharingService = sharingService ?? throw new ArgumentNullException(nameof(sharingService));
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
