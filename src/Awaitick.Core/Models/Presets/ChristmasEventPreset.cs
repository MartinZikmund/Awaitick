using System;

namespace Awaitick.Core.Models.Presets;

public class ChristmasEventPreset : EventPreset
{
    public ChristmasEventPreset() : base(EventCategory.ChristianHoliday, "Christmas", TextTheme.Light)
    {
    }

    protected override DateTimeOffset GetTargetDate()
    {
        // Christmas is on December 25th. If today is after, use next year.
        var now = DateTimeOffset.Now;
        var christmas = new DateTimeOffset(now.Year, 12, 25, 0, 0, 0, now.Offset);
        if (christmas < now)
        {
            christmas = new DateTimeOffset(now.Year + 1, 12, 25, 0, 0, 0, now.Offset);
        }
        return christmas;
    }
}
