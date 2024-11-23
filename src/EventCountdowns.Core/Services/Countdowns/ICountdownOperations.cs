namespace EventCountdowns.Core.Services.Countdowns;

internal interface ICountdownOperations
{
	Task<bool> PromptDeleteAsync();
}
