using System.Text;
using Windows.UI.Popups;
using EventCountdowns.Core.Services.Rating;
using EventCountdowns.Core.Services.Settings;
using EventCountdowns.Core.Services.StoreLauncher;
using System.Globalization;

namespace EventCountdowns.Core.Services;

public class AppRatingService : IAppRatingService
{
	private readonly IStringLocalizer _localizationService;
	private readonly IAppPreferences _appSettings;
	private readonly IStoreLauncherService _storeLauncherService;

	public AppRatingService(IStringLocalizer localizationService, IAppPreferences appSettings, IStoreLauncherService storeLauncherService)
	{
		_localizationService = localizationService;
		_appSettings = appSettings;
		_storeLauncherService = storeLauncherService;
	}

	public async Task AskUserForRatingAsync()
	{
		MessageDialog ratingDialog = new MessageDialog(
			string.Format(CultureInfo.CurrentCulture, _localizationService.GetString("RatingDialogContentFormatString"), _localizationService.GetString("AppName")),
			string.Format(CultureInfo.CurrentCulture, _localizationService.GetString("DoYouEnjoyAppFormatString"), _localizationService.GetString("AppName")));
		ratingDialog.Commands.Add(new UICommand(_localizationService.GetString("RateNow"), RateNowHandler));
		ratingDialog.Commands.Add(new UICommand(_localizationService.GetString("Later"), LaterHandler));
		await ratingDialog.ShowAsync();
	}

	private void DoNotRemindMe(IUICommand command) => _appSettings.OfferUserRating = false;

	private void LaterHandler(IUICommand command)
	{
		//ignore
	}

	private async void RateNowHandler(IUICommand command)
	{
		//launch store review
		_appSettings.OfferUserRating = false;
		await _storeLauncherService.RateAppAsync();
	}
}
