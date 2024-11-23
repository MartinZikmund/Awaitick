namespace EventCountdowns.Core.Models;

public partial class CountdownViewModel : ObservableObject
{
	private readonly EventCountdown _eventCountdown;

	public CountdownViewModel(EventCountdown eventCountdown)
	{
		_eventCountdown = eventCountdown ?? throw new ArgumentNullException(nameof(eventCountdown));
	}

	public EventCountdown Model => _eventCountdown;

	public string Id => _eventCountdown.Id;

	public string Name => _eventCountdown.Name;

	public Uri? BackgroundImage => _eventCountdown.BackgroundImageUri;

	public bool HasFinished => _eventCountdown.TargetDateTime < DateTimeOffset.Now;

	public TimeSpan TimeLeft => _eventCountdown.TargetDateTime - DateTimeOffset.Now;

	public int DaysLeft => TimeLeft.Days;

	public int HoursLeft => TimeLeft.Hours;

	public int MinutesLeft => TimeLeft.Minutes;

	public int SecondsLeft => TimeLeft.Seconds;

	public DateTimeOffset TargetDateTime => _eventCountdown.TargetDateTime;

	public string CelebrationMessage => _eventCountdown.CelebrationMessage;

	public void UpdateBindings()
	{
		OnPropertyChanged(nameof(DaysLeft));
		OnPropertyChanged(nameof(HoursLeft));
		OnPropertyChanged(nameof(MinutesLeft));
		OnPropertyChanged(nameof(SecondsLeft));
		OnPropertyChanged(nameof(TimeLeft));
		OnPropertyChanged(nameof(HasFinished));
	}
}
