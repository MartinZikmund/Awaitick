namespace Awaitick.Core.Infrastructure;

public interface IAppUpdater
{
	Task EnsureAppUpToDateAsync();
}
