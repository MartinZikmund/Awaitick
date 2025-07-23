using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awaitick.Core.Models.Presets;

public class EventPresets
{
	public static readonly List<EventPreset> Presets = new()
	{
		new EasterEventPreset(),
		new ChristmasEventPreset(),
		new FixedDateEventPreset(EventCategory.GeneralHoliday, "NewYear", new DateTimeOffset(DateTime.Now.Year, 1, 1, 0, 0, 0, DateTimeOffset.Now.Offset)),

	};
}
