using SQLite;

namespace Awaitick.Core.Models.Database;

[Table("EventCountdowns")]
public class EventCountdownEntity
{
	[PrimaryKey]
	public string Id { get; set; } = string.Empty;

	public string Name { get; set; } = string.Empty;

	public string CelebrationMessage { get; set; } = string.Empty;

	public DateTimeOffset TargetDateTime { get; set; }

	public string? BackgroundImageUri { get; set; }

	public int TextTheme { get; set; }

	public double BackgroundImageOpacity { get; set; }

	// Nullable so legacy rows (added via CreateTable migration) read as null and fall back to centered.
	public double? BackgroundImageVerticalPosition { get; set; }

	public string BackgroundColor { get; set; } = string.Empty;
}
