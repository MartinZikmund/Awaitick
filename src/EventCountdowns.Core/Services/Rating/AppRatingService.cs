using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using EventCountdowns.Core.Services;
using EventCountdowns.Core.Services.Rating;
using EventCountdowns.Core.Services.Settings;
using EventCountdowns.Core.Services.StoreLauncher;

namespace EventCountdowns.Core.Services
{
    public class AppRatingService : IAppRatingService
    {
        private readonly ILocalizationService _localizationService;
        private readonly IAppSettings _appSettings;
        private readonly IStoreLauncherService _storeLauncherService;

        public AppRatingService(ILocalizationService localizationService, IAppSettings appSettings, IStoreLauncherService storeLauncherService)
        {
            _localizationService = localizationService;
            _appSettings = appSettings;
            _storeLauncherService = storeLauncherService;
        }

        public async Task AskUserForRatingAsync()
        {
            MessageDialog ratingDialog = new MessageDialog(
                string.Format(_localizationService.RatingDialogContentFormatString, _localizationService.AppName),
                string.Format(_localizationService.DoYouEnjoyAppFormatString, _localizationService.AppName)
                 );
            ratingDialog.Commands.Add(new UICommand(_localizationService.RateNow, RateNowHandler));
            ratingDialog.Commands.Add(new UICommand(_localizationService.Later, LaterHandler));
            await ratingDialog.ShowAsync();
        }

        private void DoNotRemindMe(IUICommand command)
        {
            _appSettings.OfferUserRating = false;
        }

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
}
