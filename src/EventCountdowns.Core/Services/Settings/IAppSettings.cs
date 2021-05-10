using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventCountdowns.Models.Theming;

namespace EventCountdowns.Core.Services.Settings
{
    public interface IAppSettings
    {
        int DataVersion { get; set; }

        bool FirstStart { get; set; }

        int LaunchCount { get; set; }

        bool OfferUserRating { get; set; }

        AppTheme Theme { get; set; }
    }
}
