using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventCountdowns.Core.Services.Settings
{
    public class AppSettings : IAppSettings
    {
        private readonly ISettingsService _settingsService;

        public AppSettings( ISettingsService settingsService )
        {
            _settingsService = settingsService;
        }

        private const string DataVersionKey = "AppDataVersion";

        public int DataVersion
        {
            get { return _settingsService.GetSetting( DataVersionKey, () => 0 ); }
            set { _settingsService.SetSetting( DataVersionKey, value ); }
        }

        private const string FirstStartKey = "AppFirstStart";

        public bool FirstStart
        {
            get { return _settingsService.GetSetting( FirstStartKey, () => true ); }
            set { _settingsService.SetSetting( FirstStartKey, value ); }
        }

        private const string LaunchCountKey = "AppLaunchCount";

        public int LaunchCount
        {
            get { return _settingsService.GetSetting( LaunchCountKey, () => 0 ); }
            set { _settingsService.SetSetting( LaunchCountKey, value ); }
        }

        private const string OfferUserRatingKey = "OfferUserRating";

        public bool OfferUserRating
        {
            get { return _settingsService.GetSetting( OfferUserRatingKey, () => true, true ); }
            set
            {
                _settingsService.SetSetting( OfferUserRatingKey, value, true );
            }
        }
    }
}