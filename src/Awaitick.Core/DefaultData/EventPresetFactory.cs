using System.Globalization;
using System.Text;
using Awaitick.Core.Models;

namespace Awaitick.Core.DefaultData;

public class EventPresetFactory : IEventPresetFactory
{
	private readonly IDefaultBackgrounds _defaultBackgrounds;
	private readonly IStringLocalizer _localizationService;

	public EventPresetFactory(IDefaultBackgrounds defaultBackgrounds, IStringLocalizer localizationService)
	{
		_defaultBackgrounds = defaultBackgrounds;
		_localizationService = localizationService;
	}

	public EventCountdown Create(EventPreset preset) =>
		preset switch
		{
			EventPreset.Christmas => CreateChristmasCountdown(),
			EventPreset.Easter => CreateEasterCountdown(),
			EventPreset.Halloween => CreateHalloweenCountdown(),
			EventPreset.NewYear => CreateNewYearCountdown(),
			_ => throw new InvalidOperationException("Unknown preset"),
		};

	private EventCountdown CreateNewYearCountdown()
	{
		var newYearDate = new DateTimeOffset(DateTimeOffset.Now.Year, 1, 1, 00, 00, 00, DateTimeOffset.Now.Offset);
		if (newYearDate < DateTimeOffset.Now)
		{
			newYearDate = new DateTimeOffset(DateTimeOffset.Now.Year + 1, 1, 1, 00, 00, 00, DateTimeOffset.Now.Offset);
		}
		return new EventCountdown()
		{
			Name = _localizationService.GetString("NewYear"),
			BackgroundImageUri = _defaultBackgrounds.GetSampleEventBackground(EventPreset.NewYear).BackgroundUri,
			CelebrationMessage = _localizationService.GetString("HappyNewYear"),
			Id = Guid.NewGuid().ToString(),
			TargetDateTime = newYearDate
		};
	}

	private EventCountdown CreateChristmasCountdown()
	{
		var christmasDate = new DateTimeOffset(DateTimeOffset.Now.Year, 12, 25, 00, 00, 00, DateTimeOffset.Now.Offset);
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
			Name = _localizationService.GetString("Christmas"),
			BackgroundImageUri = _defaultBackgrounds.GetSampleEventBackground(EventPreset.Christmas).BackgroundUri,
			Id = Guid.NewGuid().ToString(),
			CelebrationMessage = _localizationService.GetString("MerryChristmas"),
			TargetDateTime = christmasDate
		};
	}

	private EventCountdown CreateHalloweenCountdown()
	{
		var halloweenDate = new DateTimeOffset(DateTimeOffset.Now.Year, 10, 31, 00, 00, 00, DateTimeOffset.Now.Offset);
		if (halloweenDate < DateTimeOffset.Now)
		{
			halloweenDate = new DateTimeOffset(DateTimeOffset.Now.Year + 1, 10, 31, 00, 00, 00, DateTimeOffset.Now.Offset);
		}
		return new EventCountdown()
		{
			Name = _localizationService.GetString("Halloween"),
			BackgroundImageUri = _defaultBackgrounds.GetSampleEventBackground(EventPreset.Halloween).BackgroundUri,
			CelebrationMessage = _localizationService.GetString("ScaryHalloween"),
			Id = Guid.NewGuid().ToString(),
			TargetDateTime = halloweenDate
		};
	}

	private EventCountdown CreateEasterCountdown()
	{
		var easterDate = EasterSunday(DateTimeOffset.Now.Year);
		if (easterDate < DateTimeOffset.Now)
		{
			easterDate = EasterSunday(DateTimeOffset.Now.Year + 1);
		}

		return new()
		{
			Name = _localizationService.GetString("Easter"),
			BackgroundImageUri =
				_defaultBackgrounds.GetSampleEventBackground(EventPreset.Easter).BackgroundUri,
			CelebrationMessage = _localizationService.GetString("HappyEaster"),
			Id = Guid.NewGuid().ToString(),
			TargetDateTime = easterDate
		};
	}

	private (int month, int day) CalculateEasterSunday(int year)
	{
		int g = year % 19;
		int c = year / 100;
		int h = h = (c - (int)(c / 4) - (int)((8 * c + 13) / 25)
											+ 19 * g + 15) % 30;
		int i = h - (int)(h / 28) * (1 - (int)(h / 28) *
					(int)(29 / (h + 1)) * (int)((21 - g) / 11));

		var day = i - ((year + (int)(year / 4) + i + 2 - c + (int)(c / 4)) % 7) + 28;
		var month = 3;

		if (day > 31)
		{
			month++;
			day -= 31;
		}

		return (month, day);
	}

	private DateTimeOffset EasterSunday(int year)
	{
		var (month, day) = CalculateEasterSunday(year);

		var date = new DateTimeOffset(year, month, day, 0, 0, 0, DateTimeOffset.Now.Offset);
		return date;
	}
}
