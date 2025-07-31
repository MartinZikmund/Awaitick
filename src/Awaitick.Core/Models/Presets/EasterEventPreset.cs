namespace Awaitick.Core.Models.Presets;

public class EasterEventPreset : EventPreset
{
	public EasterEventPreset() : base(EventCategory.ChristianHoliday, "Easter")
	{
	}

	protected override DateTimeOffset GetTargetDate()
	{
		try
		{
			var easterDate = EasterSunday(DateTimeOffset.Now.Year);
			if (easterDate < DateTimeOffset.Now)
			{
				easterDate = EasterSunday(DateTimeOffset.Now.Year + 1);
			}

			return easterDate;
		}
		catch
		{
			// We cannot calculate Easter date, fallback to a fixed date
			return GetDateInFuture(new DateTimeOffset(2025, 4, 20, 12, 0, 0, DateTimeOffset.Now.Offset));
		}
	}

	private void EasterSunday(int year, out int month, out int day)
	{
		int g = year % 19;
		int c = year / 100;
		int h = (c - (int)(c / 4) - (int)((8 * c + 13) / 25)
											+ 19 * g + 15) % 30;
		int i = h - (int)(h / 28) * (1 - (int)(h / 28) *
					(int)(29 / (h + 1)) * (int)((21 - g) / 11));

		day = i - ((year + (int)(year / 4) +
					  i + 2 - c + (int)(c / 4)) % 7) + 28;
		month = 3;

		if (day > 31)
		{
			month++;
			day -= 31;
		}
	}

	private DateTimeOffset EasterSunday(int year)
	{
		int month = 0;
		int day = 0;
		EasterSunday(year, out month, out day);

		var date = new DateTimeOffset(year, month, day, 0, 0, 0, DateTimeOffset.Now.Offset);
		return date;
	}
}
