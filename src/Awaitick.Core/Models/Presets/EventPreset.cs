using Awaitick.Services.Localization;
using Microsoft.UI;
using Windows.UI;

namespace Awaitick.Core.Models.Presets;

public abstract class EventPreset
{
	public EventPreset(
		EventCategory category,
		int groupNumber,
		string eventPresetKey,
		TextTheme textTheme,
		double backgroundImageOpacity = 0.8,
		Color? backgroundColor = null)
	{
		Key = eventPresetKey;
		Category = category;
		GroupNumber = groupNumber;
		BackgroundImageOpacity = backgroundImageOpacity;
		BackgroundColor = backgroundColor;
		TextTheme = textTheme;
	}

	public string Key { get; }

	public int GroupNumber { get; }

	public string Name => Localizer.Instance.GetString($"EventPreset_{Key}_Name");

	public Uri BackgroundImageUri => new Uri($"ms-appx:///Assets/EventBackgrounds/{Key}.jpg", UriKind.Absolute);

	public EventCategory Category { get; }

	public double BackgroundImageOpacity { get; }

	public Color? BackgroundColor { get; }

	public TextTheme TextTheme { get; }

	protected abstract DateTimeOffset GetTargetDate();

	protected DateTimeOffset GetDateInFuture(DateTimeOffset date)
	{
		while (date < DateTimeOffset.Now)
		{
			date = date.AddYears(1);
		}

		return date;
	}

	public EventCountdown Create()
	{
		var celebrationMessageKey = $"EventCountdown_{Key}_CelebrationMessage";

		var celebrationMessage = Localizer.Instance.GetString(celebrationMessageKey);

		return new EventCountdown
		{
			Name = Name,
			CelebrationMessage = celebrationMessage,
			BackgroundImageUri = BackgroundImageUri,
			BackgroundImageOpacity = BackgroundImageOpacity,
			BackgroundColor = BackgroundColor is not null ? ColorHelper.ToHex(BackgroundColor.Value) : ColorHelper.ToHex(Colors.Transparent),
			TextTheme = TextTheme,
			TargetDateTime = GetTargetDate(),
		};
	}
}
