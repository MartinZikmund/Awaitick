using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using EventCountdowns.Core.Services.BackgroundTasks;

namespace EventCountdowns.Core.Services;

public class BackgroundTasksService : IBackgroundTasksService
{
	public async Task<bool> RegisterAsync()
	{
		try
		{
			string myTaskName = "TileUpdaterBackgroundTask";

			// check if task is already registered
			var task =
				BackgroundTaskRegistration.AllTasks.Where(cur => cur.Value.Name == myTaskName).Select(c => c.Value).SingleOrDefault();
			if (task != null)
			{
				//do not register again                    
				return true;
			}

			var backgroundAccess = await BackgroundExecutionManager.RequestAccessAsync();

			if (backgroundAccess == BackgroundAccessStatus.AlwaysAllowed ||
				backgroundAccess == BackgroundAccessStatus.AllowedSubjectToSystemPolicy)
			{
				// register a new task
				BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder
				{
					Name = myTaskName,
					TaskEntryPoint = "MyCountdowns.Client.WindowsUniversal.TileUpdateTask.TileUpdateBackgroundTask"
				};

				taskBuilder.SetTrigger(new TimeTrigger(30, false));

				BackgroundTaskRegistration myFirstTask = taskBuilder.Register();
				return true;
			}
		}
		catch (Exception ex)
		{
			//TODO:Track exception
		}
		return false;
	}

	public Task<bool> UnregisterAsync()
	{
		try
		{
			string myTaskName = "TileUpdaterBackgroundTask";

			// check if task is already registered
			var task =
				BackgroundTaskRegistration.AllTasks.Where(cur => cur.Value.Name == myTaskName).Select(c => c.Value).SingleOrDefault();
			if (task != null)
			{
				//do not register again                    
				task.Unregister(false);
			}
			return Task.FromResult(true);
		}
		catch (Exception ex)
		{
			//TODO:Track exception
		}
		return Task.FromResult(false);
	}
}
