using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace EventCountdowns.Core.Models;

public class EventCountdownObservable : ObservableObject
{
	private readonly EventCountdown _eventCountdown;

	public EventCountdownObservable(EventCountdown eventCountdown)
	{
		_eventCountdown = eventCountdown;
	}

	public EventCountdown Model => _eventCountdown;

	public string Id => _eventCountdown.Id;

	public string Name => _eventCountdown.Name;

	public string BackgroundImagePath => _eventCountdown.BackgroundImagePath;

	public bool Finished => _eventCountdown.TargetDateTime < DateTimeOffset.Now;

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
		OnPropertyChanged(nameof(Finished));
	}
}
