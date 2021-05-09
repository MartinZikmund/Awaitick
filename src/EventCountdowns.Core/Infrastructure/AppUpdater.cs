using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Platform;
using EventCountdowns.Core.Configuration;
using EventCountdowns.Core.Services.BackgroundTasks;
using EventCountdowns.Core.Services.Settings;

namespace EventCountdowns.Core.Infrastructure
{
    public class AppUpdater : IAppUpdater
    {
        private readonly IAppSettings _settings;

        public AppUpdater( IAppSettings settings )
        {
            _settings = settings;
        }

        public async Task EnsureAppUpToDateAsync()
        {
            if ( _settings.LaunchCount == 0 )
            {
                _settings.DataVersion = ApplicationReleaseInfo.DataVersion;
            }
            int retryCounter = 0;
            while ( _settings.DataVersion < ApplicationReleaseInfo.DataVersion )
            {
                int originalDataVersion = _settings.DataVersion;
                if ( _settings.DataVersion == 0 )
                {
                    await UpdateFromVersion0ToVersion1Async();
                }
                if ( _settings.DataVersion == originalDataVersion )
                {
                    retryCounter++;
                    if ( retryCounter >= 3 )
                    {
                        //error 
                        throw new InvalidDataException( "Application not properly updated" );
                    }
                }
            }
        }

        public async Task UpdateFromVersion0ToVersion1Async()
        {
            //unregister background task
            if ( await Mvx.Resolve<IBackgroundTasksService>().UnregisterAsync() )
            {
                _settings.DataVersion = 1;
            }
        }
    }
}
