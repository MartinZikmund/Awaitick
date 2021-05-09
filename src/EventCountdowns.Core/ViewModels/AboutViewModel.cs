using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using EventCountdowns.Core.Services.StoreLauncher;

namespace EventCountdowns.Core.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        private readonly IStoreLauncherService _storeLauncherService;

        public AboutViewModel( IStoreLauncherService storeLauncherService )
        {
            _storeLauncherService = storeLauncherService;
        }

        private ICommand _moreAppsCommand = null;

        public ICommand MoreAppsCommand => _moreAppsCommand ?? (_moreAppsCommand = new MvxCommand( MoreApps));

        private async void MoreApps()
        {
            IsWorking = true;
            await _storeLauncherService.MoreAppsByPublisherAsync();
            IsWorking = false;
        }
    }
}
