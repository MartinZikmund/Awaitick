namespace EventCountdowns.Core.Configuration;

public record AppConfig
{
	public string? Environment { get; init; }

	public string? ApiUrl { get; init; }
}
