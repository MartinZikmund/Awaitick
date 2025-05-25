using System.Diagnostics;
using System.Globalization;
using Awaitick.Core.Services.Countdowns;

namespace Awaitick.Core.Models;

public partial class CountdownViewModel : ObservableObject
{
	private readonly EventCountdown _eventCountdown;
	private readonly ICountdownsManager _countdownsManager;

	public CountdownViewModel(EventCountdown eventCountdown, ICountdownsManager? countdownsManager)
	{
		_eventCountdown = eventCountdown ?? throw new ArgumentNullException(nameof(eventCountdown));
		_countdownsManager = countdownsManager;
	}

	public EventCountdown Model => _eventCountdown;

	public string Id => _eventCountdown.Id;

	public string Name => _eventCountdown.Name;

	public Uri? BackgroundImageUri => _eventCountdown.BackgroundImageUri;

	public double BackgroundImageOpacity => _eventCountdown.BackgroundImageOpacity;

	public ElementTheme Theme => _eventCountdown.Theme;

	public Windows.UI.Color BackgroundColor => ColorHelper.ToColor(_eventCountdown.BackgroundColor);

	public bool HasFinished => _eventCountdown.TargetDateTime < DateTimeOffset.Now;

	public TimeSpan TimeLeft => _eventCountdown.TargetDateTime - DateTimeOffset.Now;

	public int DaysLeft => TimeLeft.Days;

	public int HoursLeft => TimeLeft.Hours;

	public int MinutesLeft => TimeLeft.Minutes;

	public int SecondsLeft => TimeLeft.Seconds;

	public DateTimeOffset TargetDateTime => _eventCountdown.TargetDateTime;
	
	public string TargetDateString => TargetDateTime.ToString("f", CultureInfo.CurrentCulture);

	public string CelebrationMessage => _eventCountdown.CelebrationMessage;

	[RelayCommand]
	public void GoToDetail() => _countdownsManager?.GoToDetail(this);

	[RelayCommand]
	public Task ShareAsync() => _countdownsManager is not null ? _countdownsManager.ShareAsync(this) : Task.CompletedTask;

	[RelayCommand]
	public void GoToEdit() => _countdownsManager?.GoToEdit(this);

	[RelayCommand]
	public Task<bool> DeleteAsync() => _countdownsManager is not null ? _countdownsManager.DeleteAsync(this) : Task.FromResult(false);

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
