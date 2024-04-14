namespace EventCountdowns.Core.Models;

public class EventCountdown
{
	public string Id { get; set; }

	public string Name { get; set; }

	public string CelebrationMessage { get; set; }

	public DateTimeOffset TargetDateTime { get; set; }

	public string BackgroundImagePath { get; set; }
}
