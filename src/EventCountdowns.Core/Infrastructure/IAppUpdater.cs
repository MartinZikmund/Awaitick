namespace EventCountdowns.Core.Infrastructure;

public interface IAppUpdater
{
	Task EnsureAppUpToDateAsync();
}
