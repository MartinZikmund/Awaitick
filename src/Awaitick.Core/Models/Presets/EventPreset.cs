using Awaitick.Services.Localization;
using Microsoft.UI;
using Windows.UI;

namespace Awaitick.Core.Models.Presets;

public abstract class EventPreset(
	string EventPresetKey,
	double BackgroundImageOpacity = 0.8,
	Color? BackgroundColor = null,
	ElementTheme Theme = ElementTheme.Default)
{
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
		var displayNameKey = $"EventCountdown.{EventPresetKey}.Name";
		var celebrationMessageKey = $"EventCountdown.{EventPresetKey}.CelebrationMessage";
		var backgroundImageUri = new Uri($"ms-appx:///Assets/Events/{EventPresetKey}.jpg", UriKind.Absolute);

		var displayName = Localizer.Instance.GetString(displayNameKey);
		var celebrationMessage = Localizer.Instance.GetString(celebrationMessageKey);

		return new EventCountdown
		{
			Name = displayName,
			CelebrationMessage = celebrationMessage,
			BackgroundImageUri = backgroundImageUri,
			BackgroundImageOpacity = BackgroundImageOpacity,
			BackgroundColor = BackgroundColor is not null ? ColorHelper.ToHex(BackgroundColor.Value) : ColorHelper.ToHex(Colors.Transparent),
			Theme = Theme,
			TargetDateTime = GetTargetDate(),
		};
	}
}
