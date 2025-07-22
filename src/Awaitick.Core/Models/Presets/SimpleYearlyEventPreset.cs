
namespace Awaitick.Core.Models.Presets;

internal record SimpleYearlyEventPreset(string DisplayNameKey, string CelebrationMessageKey, Uri? BackgroundImageUri, double BackgroundImageOpacity, string BackgroundColor, ElementTheme Theme) :
	EventPreset(DisplayNameKey, CelebrationMessageKey, BackgroundImageUri, BackgroundImageOpacity, BackgroundColor, Theme)
{
	protected override DateTimeOffset GetTargetDate()
	{
		return new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);
	}
}
