using Awaitick.Services.Localization;
using Microsoft.UI;
using Windows.UI;

namespace Awaitick.Core.Models.Presets;

public abstract class EventPreset
{
	private string _eventPresetKey;
	
	public EventPreset(
		EventCategory category,
		string eventPresetKey,
		double backgroundImageOpacity = 0.8,
		Color? backgroundColor = null,
		ElementTheme theme = ElementTheme.Default)
	{
		_eventPresetKey = eventPresetKey;
		Category = category;
		BackgroundImageOpacity = backgroundImageOpacity;
		BackgroundColor = backgroundColor;
		Theme = theme;
	}

	public string Name => Localizer.Instance.GetString($"EventPreset.{_eventPresetKey}.Name");

	public Uri BackgroundImageUri => new Uri($"ms-appx:///Assets/EventPresets/{_eventPresetKey}.jpg", UriKind.Absolute);

	public EventCategory Category { get; }

	public double BackgroundImageOpacity { get; }

	public Color? BackgroundColor { get; }

	public ElementTheme Theme { get; }

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
		var celebrationMessageKey = $"EventCountdown.{_eventPresetKey}.CelebrationMessage";

		var celebrationMessage = Localizer.Instance.GetString(celebrationMessageKey);

		return new EventCountdown
		{
			Name = Name,
			CelebrationMessage = celebrationMessage,
			BackgroundImageUri = BackgroundImageUri,
			BackgroundImageOpacity = BackgroundImageOpacity,
			BackgroundColor = BackgroundColor is not null ? ColorHelper.ToHex(BackgroundColor.Value) : ColorHelper.ToHex(Colors.Transparent),
			Theme = Theme,
			TargetDateTime = GetTargetDate(),
		};
	}
}
