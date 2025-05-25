namespace Awaitick.Core.Models.Database;

public class EventEntity
{
	public Guid Id { get; set; }

	public string Name { get; set; }

	public string CelebrationMessage { get; set; }

	public DateTimeOffset TargetDateTime { get; set; }

	public string BackgroundImageUri { get; set; }
}
