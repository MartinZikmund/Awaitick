
using Windows.UI;

namespace Awaitick.Core.Models.Presets;

public class FixedDateEventPreset : EventPreset
{
	private DateTimeOffset _targetDate;

	public FixedDateEventPreset(EventCategory category, string eventPresetKey, DateTimeOffset targetDate, double backgroundImageOpacity = 0.8, Color? backgroundColor = null, ElementTheme theme = ElementTheme.Default)
		: base(category, eventPresetKey, backgroundImageOpacity, backgroundColor, theme)
	{		
		_targetDate = targetDate;
	}

	protected override DateTimeOffset GetTargetDate() => GetDateInFuture(_targetDate);
}
