using Awaitick.Core.Configuration;
using Awaitick.Core.Services.Settings;

namespace Awaitick.Core.Infrastructure;

public class AppUpdater : IAppUpdater
{
	private readonly IAppPreferences _settings;

	public AppUpdater(IAppPreferences settings)
	{
		_settings = settings;
	}

	public async Task EnsureAppUpToDateAsync()
	{
		if (_settings.LaunchCount == 0)
		{
			_settings.DataVersion = ApplicationReleaseInfo.DataVersion;
		}

		int retryCounter = 0;
		while (_settings.DataVersion < ApplicationReleaseInfo.DataVersion)
		{
			int originalDataVersion = _settings.DataVersion;
			if (_settings.DataVersion == 0)
			{
				await UpdateFromVersion0ToVersion1Async();
			}
			if (_settings.DataVersion == originalDataVersion)
			{
				retryCounter++;
				if (retryCounter >= 3)
				{
					//error 
					throw new InvalidDataException("Application not properly updated");
				}
			}
		}
	}

	public async Task UpdateFromVersion0ToVersion1Async()
	{
	}
}
