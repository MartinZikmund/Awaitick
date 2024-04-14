using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventCountdowns.Core.Models;
using EventCountdowns.Core.Services;


/* Unmerged change from project 'EventCountdowns (net8.0)'
Before:
namespace EventCountdowns.Core.DefaultData
{
	public class SampleEvents : ISampleEvents
After:
namespace EventCountdowns.Core.DefaultData;

	public class SampleEvents : ISampleEvents
*/
namespace EventCountdowns.Core.DefaultData;

public class SampleEvents : ISampleEvents
{
	private readonly IDefaultBackgrounds _defaultBackgrounds;
	private readonly ILocalizationService _localizationService;

	public SampleEvents(IDefaultBackgrounds defaultBackgrounds, ILocalizationService localizationService)
	{
		_defaultBackgrounds = defaultBackgrounds;
		_localizationService = localizationService;
	}

	public EventCountdown[] GetSampleEvents()
	{
		List<EventCountdown> events = new List<EventCountdown>
		{
			CreateChristmasCountdown(),
			CreateHalloweenCountdown(),
			CreateNewYearCountdown()
		};
		var easterCountdown = CreateEasterCountdown();
		if (easterCountdown != null)
		{
			events.Add(easterCountdown);
		}
		return events.ToArray();
	}

	private EventCountdown CreateNewYearCountdown()
	{
		var newYearDate = new DateTimeOffset(DateTimeOffset.Now.Year, 1, 1, 00, 00, 00,
		   DateTimeOffset.Now.Offset);
		if (newYearDate < DateTimeOffset.Now)
		{
			newYearDate = new DateTimeOffset(DateTimeOffset.Now.Year + 1, 1, 1, 00, 00, 00, DateTimeOffset.Now.Offset);
		}
		return new EventCountdown()
		{
			Name = _localizationService.NewYear,
			BackgroundImagePath = _defaultBackgrounds.GetSampleEventBackground(SampleEventTypes.NewYear).BackgroundPath,
			CelebrationMessage = _localizationService.HappyNewYear,
			Id = Guid.NewGuid().ToString(),
			TargetDateTime = newYearDate
		};
	}

	private EventCountdown CreateChristmasCountdown()
	{
		var christmasDate = new DateTimeOffset(DateTimeOffset.Now.Year, 12, 25, 00, 00, 00,
DateTimeOffset.Now.Offset);
		if (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.StartsWith("cs"))
		{
			christmasDate = christmasDate.AddDays(-1);
		}
		if (christmasDate < DateTimeOffset.Now)
		{
			christmasDate = new DateTimeOffset(DateTimeOffset.Now.Year + 1, 12, 25, 00, 00, 00, DateTimeOffset.Now.Offset);
		}

		return new EventCountdown()
		{
			Name = _localizationService.Christmas,
			BackgroundImagePath = _defaultBackgrounds.GetSampleEventBackground(SampleEventTypes.Christmas).BackgroundPath,
			Id = Guid.NewGuid().ToString(),
			CelebrationMessage = _localizationService.MerryChristmas,
			TargetDateTime = christmasDate
		};
	}

	private EventCountdown CreateHalloweenCountdown()
	{
		var halloweenDate = new DateTimeOffset(DateTimeOffset.Now.Year, 10, 31, 00, 00, 00,
			DateTimeOffset.Now.Offset);
		if (halloweenDate < DateTimeOffset.Now)
		{
			halloweenDate = new DateTimeOffset(DateTimeOffset.Now.Year + 1, 10, 31, 00, 00, 00, DateTimeOffset.Now.Offset);
		}
		return new EventCountdown()
		{
			Name = _localizationService.Halloween,
			BackgroundImagePath = _defaultBackgrounds.GetSampleEventBackground(SampleEventTypes.Halloween).BackgroundPath,
			CelebrationMessage = _localizationService.ScaryHalloween,
			Id = Guid.NewGuid().ToString(),
			TargetDateTime = halloweenDate
		};
	}

	private EventCountdown CreateEasterCountdown()
	{
		try
		{
			var easterDate = EasterSunday(DateTimeOffset.Now.Year);
			if (easterDate < DateTimeOffset.Now)
			{
				easterDate = EasterSunday(DateTimeOffset.Now.Year + 1);
			}

			return new EventCountdown()
			{
				Name = _localizationService.Easter,
				BackgroundImagePath =
					_defaultBackgrounds.GetSampleEventBackground(SampleEventTypes.Easter).BackgroundPath,
				CelebrationMessage = _localizationService.HappyEaster,
				Id = Guid.NewGuid().ToString(),
				TargetDateTime = easterDate
			};
		}
		catch
		{
			return null;
		}
	}

	private void EasterSunday(int year, out int month, out int day)
	{
		int g = year % 19;
		int c = year / 100;
		int h = h = (c - (int)(c / 4) - (int)((8 * c + 13) / 25)
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
