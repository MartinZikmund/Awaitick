using System.Globalization;
using EventCountdowns.Core.Models;

namespace EventCountdowns.Core.Services;

public class EventSharingService : IEventSharingService
{
	private readonly ISystemSharingService _systemSharingService;
	private readonly IStringLocalizer _localizationService;

	public EventSharingService(ISystemSharingService systemSharingService, IStringLocalizer localizationService)
	{
		_systemSharingService = systemSharingService ?? throw new ArgumentNullException(nameof(systemSharingService));
		_localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
	}

	public async Task ShareAsync(EventCountdownObservable eventInfo)
	{
		string sharedText = "";
		if (eventInfo.Finished)
		{
			sharedText = string.Format(
				CultureInfo.CurrentCulture,
				_localizationService.GetString("SharingFinishedEventFormatString"),
				eventInfo.CelebrationMessage,
				_localizationService.GetString("AppName"));
		}
		else
		{
			sharedText = string.Format(
				CultureInfo.CurrentCulture,
				_localizationService.GetString("SharingFormatString"),
				eventInfo.Name,
				eventInfo.DaysLeft,
				eventInfo.HoursLeft,
				eventInfo.MinutesLeft,
				eventInfo.TargetDateTime.ToString("g", CultureInfo.CurrentCulture),
				_localizationService.GetString("AppName"));
		}

		await _systemSharingService.ShareTextAsync(sharedText);
	}
}
