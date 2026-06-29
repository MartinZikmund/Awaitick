using System.Diagnostics;
using System.Globalization;
using Awaitick.Core.Services.Countdowns;
using Microsoft.UI.Xaml.Media;

namespace Awaitick.Core.Models;

public partial class CountdownViewModel : ObservableObject
{
	private EventCountdown _eventCountdown;
	private readonly ICountdownsManager _countdownsManager;

	private TimeSpan _cachedTimeLeft;
	private bool _cachedHasFinished;

	public CountdownViewModel(EventCountdown eventCountdown, ICountdownsManager? countdownsManager)
	{
		_eventCountdown = eventCountdown ?? throw new ArgumentNullException(nameof(eventCountdown));
		_countdownsManager = countdownsManager;

		var now = DateTimeOffset.Now;
		_cachedTimeLeft = _eventCountdown.TargetDateTime - now;
		_cachedHasFinished = _eventCountdown.TargetDateTime < now;
	}

	public EventCountdown Model => _eventCountdown;

	public string Id => _eventCountdown.Id;

	public string Name => _eventCountdown.Name;

	public Uri? BackgroundImageUri => _eventCountdown.BackgroundImageUri;

	public double BackgroundImageOpacity => _eventCountdown.BackgroundImageOpacity;

	public double BackgroundImageVerticalPosition => _eventCountdown.BackgroundImageVerticalPosition;

	public AlignmentY BackgroundImageVerticalAlignment => _eventCountdown.BackgroundImageVerticalPosition switch
	{
		<= 0.34 => AlignmentY.Top,
		>= 0.66 => AlignmentY.Bottom,
		_ => AlignmentY.Center,
	};

	public ElementTheme Theme => _eventCountdown.TextTheme switch
	{
		TextTheme.Light => ElementTheme.Dark,
		TextTheme.Dark => ElementTheme.Light,
		_ => ElementTheme.Default,
	};

	public Windows.UI.Color BackgroundColor => ColorHelper.ToColor(_eventCountdown.BackgroundColor);

	public bool HasFinished => _cachedHasFinished;

	public TimeSpan TimeLeft => _cachedTimeLeft;

	public int DaysLeft => _cachedTimeLeft.Days;

	public int HoursLeft => _cachedTimeLeft.Hours;

	public int MinutesLeft => _cachedTimeLeft.Minutes;

	public int SecondsLeft => _cachedTimeLeft.Seconds;

	public DateTimeOffset TargetDateTime => _eventCountdown.TargetDateTime;

	public string TargetDateString => TargetDateTime.ToLocalTime().ToString("f", CultureInfo.CurrentCulture);

	public string CelebrationMessage => _eventCountdown.CelebrationMessage;

	[RelayCommand]
	public void GoToDetail() => _countdownsManager?.GoToDetail(this);

	[RelayCommand]
	public Task ShareAsync() => _countdownsManager is not null ? _countdownsManager.ShareAsync(this) : Task.CompletedTask;

	[RelayCommand]
	public void GoToEdit() => _countdownsManager?.GoToEdit(this);

	[RelayCommand]
	public Task CloneAsync() => _countdownsManager is not null ? _countdownsManager.CloneAsync(this) : Task.CompletedTask;

	[RelayCommand]
	public Task<bool> DeleteAsync() => _countdownsManager is not null ? _countdownsManager.DeleteAsync(this) : Task.FromResult(false);

	public void UpdateModel(EventCountdown model)
	{
		_eventCountdown = model ?? throw new ArgumentNullException(nameof(model));
		RefreshFromModel();
	}

	public void RefreshFromModel()
	{
		OnPropertyChanged(nameof(Name));
		OnPropertyChanged(nameof(BackgroundImageUri));
		OnPropertyChanged(nameof(BackgroundImageOpacity));
		OnPropertyChanged(nameof(BackgroundImageVerticalPosition));
		OnPropertyChanged(nameof(BackgroundImageVerticalAlignment));
		OnPropertyChanged(nameof(Theme));
		OnPropertyChanged(nameof(BackgroundColor));
		OnPropertyChanged(nameof(TargetDateTime));
		OnPropertyChanged(nameof(TargetDateString));
		OnPropertyChanged(nameof(CelebrationMessage));
		UpdateBindings();
	}

	public void UpdateBindings()
	{
		var now = DateTimeOffset.Now;
		var timeLeft = _eventCountdown.TargetDateTime - now;
		var previousTimeLeft = _cachedTimeLeft;
		_cachedTimeLeft = timeLeft;

		if (previousTimeLeft.Seconds != timeLeft.Seconds)
		{
			OnPropertyChanged(nameof(SecondsLeft));
		}

		if (previousTimeLeft.Minutes != timeLeft.Minutes)
		{
			OnPropertyChanged(nameof(MinutesLeft));
		}

		if (previousTimeLeft.Hours != timeLeft.Hours)
		{
			OnPropertyChanged(nameof(HoursLeft));
		}

		if (previousTimeLeft.Days != timeLeft.Days)
		{
			OnPropertyChanged(nameof(DaysLeft));
		}

		if (previousTimeLeft != timeLeft)
		{
			OnPropertyChanged(nameof(TimeLeft));
		}

		var hasFinished = _eventCountdown.TargetDateTime < now;
		if (_cachedHasFinished != hasFinished)
		{
			_cachedHasFinished = hasFinished;
			OnPropertyChanged(nameof(HasFinished));
		}
	}
}
