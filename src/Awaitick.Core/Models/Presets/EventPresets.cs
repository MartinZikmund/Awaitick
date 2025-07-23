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
		// General / Secular Holidays
		new FixedDateEventPreset(EventCategory.GeneralHoliday, "NewYear", new DateTimeOffset(DateTimeOffset.Now.Year, 1, 1, 0, 0, 0, DateTimeOffset.Now.Offset)),
		new FixedDateEventPreset(EventCategory.GeneralHoliday, "NewYearsEve", new DateTimeOffset(DateTimeOffset.Now.Year, 12, 31, 0, 0, 0, DateTimeOffset.Now.Offset)),
		new FixedDateEventPreset(EventCategory.GeneralHoliday, "ValentinesDay", new DateTimeOffset(DateTimeOffset.Now.Year, 2, 14, 0, 0, 0, DateTimeOffset.Now.Offset)),
		new FixedDateEventPreset(EventCategory.GeneralHoliday, "InternationalWomensDay", new DateTimeOffset(DateTimeOffset.Now.Year, 3, 8, 0, 0, 0, DateTimeOffset.Now.Offset)),
		new FixedDateEventPreset(EventCategory.GeneralHoliday, "AprilFoolsDay", new DateTimeOffset(DateTimeOffset.Now.Year, 4, 1, 0, 0, 0, DateTimeOffset.Now.Offset)),
		new FixedDateEventPreset(EventCategory.GeneralHoliday, "EarthDay", new DateTimeOffset(DateTimeOffset.Now.Year, 4, 22, 0, 0, 0, DateTimeOffset.Now.Offset)),
		new FixedDateEventPreset(EventCategory.GeneralHoliday, "Halloween", new DateTimeOffset(DateTimeOffset.Now.Year, 10, 31, 0, 0, 0, DateTimeOffset.Now.Offset)),

		// Religious Holidays (Fixed-date only)
		new ChristmasEventPreset(),
		new FixedDateEventPreset(EventCategory.ChristianHoliday, "AllSaintsDay", new DateTimeOffset(DateTimeOffset.Now.Year, 11, 1, 0, 0, 0, DateTimeOffset.Now.Offset)),

		// National Holidays
		new FixedDateEventPreset(EventCategory.NationalHoliday, "IndependenceDayUSA", new DateTimeOffset(DateTimeOffset.Now.Year, 7, 4, 0, 0, 0, DateTimeOffset.Now.Offset)),
		new FixedDateEventPreset(EventCategory.NationalHoliday, "BastilleDay", new DateTimeOffset(DateTimeOffset.Now.Year, 7, 14, 0, 0, 0, DateTimeOffset.Now.Offset)),
		new FixedDateEventPreset(EventCategory.NationalHoliday, "CanadaDay", new DateTimeOffset(DateTimeOffset.Now.Year, 7, 1, 0, 0, 0, DateTimeOffset.Now.Offset)),
		new FixedDateEventPreset(EventCategory.NationalHoliday, "AustraliaDay", new DateTimeOffset(DateTimeOffset.Now.Year, 1, 26, 0, 0, 0, DateTimeOffset.Now.Offset)),
		new FixedDateEventPreset(EventCategory.NationalHoliday, "IndianIndependenceDay", new DateTimeOffset(DateTimeOffset.Now.Year, 8, 15, 0, 0, 0, DateTimeOffset.Now.Offset)),
		new FixedDateEventPreset(EventCategory.NationalHoliday, "BrazilIndependenceDay", new DateTimeOffset(DateTimeOffset.Now.Year, 9, 7, 0, 0, 0, DateTimeOffset.Now.Offset)),
		new FixedDateEventPreset(EventCategory.NationalHoliday, "SouthAfricaFreedomDay", new DateTimeOffset(DateTimeOffset.Now.Year, 4, 27, 0, 0, 0, DateTimeOffset.Now.Offset)),

		// Variable-date holidays
		new EasterEventPreset(),
	};
}
