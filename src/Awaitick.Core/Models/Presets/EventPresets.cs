using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace Awaitick.Core.Models.Presets;

public class EventPresets
{
	public static readonly EventPreset[] Presets =
	[
		// General / Secular Holidays
		new FixedDateEventPreset(EventCategory.GeneralHoliday, "NewYearsEve", new DateTimeOffset(DateTimeOffset.Now.Year, 12, 31, 0, 0, 0, DateTimeOffset.Now.Offset), TextTheme.Light, backgroundColor: Color.FromArgb(255, 45, 26, 0)),
		new FixedDateEventPreset(EventCategory.GeneralHoliday, "NewYear", new DateTimeOffset(DateTimeOffset.Now.Year, 1, 1, 0, 0, 0, DateTimeOffset.Now.Offset), TextTheme.Light, backgroundColor: Color.FromArgb(255, 180, 200, 220)),
		new FixedDateEventPreset(EventCategory.GeneralHoliday, "ValentinesDay", new DateTimeOffset(DateTimeOffset.Now.Year, 2, 14, 0, 0, 0, DateTimeOffset.Now.Offset), TextTheme.Light, backgroundColor: Color.FromArgb(255, 198, 40, 40)),
		new FixedDateEventPreset(EventCategory.GeneralHoliday, "InternationalWomensDay", new DateTimeOffset(DateTimeOffset.Now.Year, 3, 8, 0, 0, 0, DateTimeOffset.Now.Offset), TextTheme.Light, backgroundColor: Color.FromArgb(255, 198, 40, 40)),
		new FixedDateEventPreset(EventCategory.GeneralHoliday, "AprilFoolsDay", new DateTimeOffset(DateTimeOffset.Now.Year, 4, 1, 0, 0, 0, DateTimeOffset.Now.Offset), TextTheme.Light, backgroundColor: Color.FromArgb(255, 255, 202, 40)),
		new FixedDateEventPreset(EventCategory.GeneralHoliday, "EarthDay", new DateTimeOffset(DateTimeOffset.Now.Year, 4, 22, 0, 0, 0, DateTimeOffset.Now.Offset), TextTheme.Light, backgroundColor: Color.FromArgb(255, 61, 122, 30)),
		new FixedDateEventPreset(EventCategory.GeneralHoliday, "Halloween", new DateTimeOffset(DateTimeOffset.Now.Year, 10, 31, 0, 0, 0, DateTimeOffset.Now.Offset), TextTheme.Light, backgroundColor: Color.FromArgb(255, 13, 27, 30)),

		// Religious Holidays (Fixed-date only)
		new ChristmasEventPreset(),
		new FixedDateEventPreset(EventCategory.ChristianHoliday, "AllSaintsDay", new DateTimeOffset(DateTimeOffset.Now.Year, 11, 1, 0, 0, 0, DateTimeOffset.Now.Offset), TextTheme.Light, backgroundColor: Color.FromArgb(255, 18, 26, 36)),

		// National Holidays
		new FixedDateEventPreset(EventCategory.NationalHoliday, "IndependenceDayUSA", new DateTimeOffset(DateTimeOffset.Now.Year, 7, 4, 0, 0, 0, DateTimeOffset.Now.Offset), TextTheme.Light, backgroundColor: Color.FromArgb(255, 12, 25, 41)),
		new FixedDateEventPreset(EventCategory.NationalHoliday, "BastilleDay", new DateTimeOffset(DateTimeOffset.Now.Year, 7, 14, 0, 0, 0, DateTimeOffset.Now.Offset), TextTheme.Light, backgroundColor: Color.FromArgb(255, 5, 5, 30)),
		new FixedDateEventPreset(EventCategory.NationalHoliday, "CanadaDay", new DateTimeOffset(DateTimeOffset.Now.Year, 7, 1, 0, 0, 0, DateTimeOffset.Now.Offset), TextTheme.Light, backgroundColor: Color.FromArgb(255, 26, 5, 5)),
		new FixedDateEventPreset(EventCategory.NationalHoliday, "AustraliaDay", new DateTimeOffset(DateTimeOffset.Now.Year, 1, 26, 0, 0, 0, DateTimeOffset.Now.Offset), TextTheme.Light, backgroundColor: Color.FromArgb(255, 26, 45, 69)),
		new FixedDateEventPreset(EventCategory.NationalHoliday, "IndianIndependenceDay", new DateTimeOffset(DateTimeOffset.Now.Year, 8, 15, 0, 0, 0, DateTimeOffset.Now.Offset), TextTheme.Light, backgroundColor: Color.FromArgb(255, 200, 100, 0)),
		new FixedDateEventPreset(EventCategory.NationalHoliday, "BrazilIndependenceDay", new DateTimeOffset(DateTimeOffset.Now.Year, 9, 7, 0, 0, 0, DateTimeOffset.Now.Offset), TextTheme.Light, backgroundColor: Color.FromArgb(255, 27, 96, 48)),
		new FixedDateEventPreset(EventCategory.NationalHoliday, "SouthAfricaFreedomDay", new DateTimeOffset(DateTimeOffset.Now.Year, 4, 27, 0, 0, 0, DateTimeOffset.Now.Offset), TextTheme.Light, backgroundColor: Color.FromArgb(255, 46, 125, 50)),

		// Variable-date holidays
		new EasterEventPreset(),
	];
}
