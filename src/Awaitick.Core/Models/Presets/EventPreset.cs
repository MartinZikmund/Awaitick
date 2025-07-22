namespace Awaitick.Core.Models.Presets;

public abstract record EventPreset(
	string DisplayNameKey,
	string CelebrationMessageKey,
	Uri? BackgroundImageUri,
	double BackgroundImageOpacity,
	string BackgroundColor,
	ElementTheme Theme)
{
	protected abstract DateTimeOffset GetTargetDate();

	public EventCountdown Create()
	{
		return new EventCountdown
		{
			Name = DisplayNameKey,
			CelebrationMessage = CelebrationMessageKey,
			BackgroundImageUri = BackgroundImageUri,
			BackgroundImageOpacity = BackgroundImageOpacity,
			BackgroundColor = BackgroundColor,
			Theme = Theme,
			TargetDateTime = GetTargetDate(),
		};
	}
}
