
using Windows.UI;

namespace Awaitick.Core.Models.Presets;

public class FixedDateEventPreset : EventPreset
{
	private DateTimeOffset _targetDate;

	public FixedDateEventPreset(EventCategory category, string eventPresetKey, DateTimeOffset targetDate, TextTheme textTheme, double backgroundImageOpacity = 0.8, Color? backgroundColor = null)
		: base(category, eventPresetKey, textTheme, backgroundImageOpacity, backgroundColor)
	{
		_targetDate = targetDate;
	}

	protected override DateTimeOffset GetTargetDate() => GetDateInFuture(_targetDate);
}
