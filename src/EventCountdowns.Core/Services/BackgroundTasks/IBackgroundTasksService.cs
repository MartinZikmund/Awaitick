namespace EventCountdowns.Core.Services.BackgroundTasks;

public interface IBackgroundTasksService
{
	Task<bool> RegisterAsync();

	Task<bool> UnregisterAsync();
}
