namespace EventCountdowns.Core.Services;

public interface ISystemSharingService
{
	Task ShareTextAsync(string data);
}
