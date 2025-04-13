
namespace EventCountdowns.Core.Models;

public class EventCountdown
{
	private Uri? _backgroundImageUri;

	public string Id { get; set; }

	public string Name { get; set; }

	public string? CelebrationMessage { get; set; }

	public DateTimeOffset TargetDateTime { get; set; }

	public string? BackgroundImagePath { get; set; }

	public Uri? BackgroundImageUri
	{
		get => _backgroundImageUri ?? (Uri.TryCreate(BackgroundImagePath, UriKind.Absolute, out var parsedPath) ? parsedPath : null);
		set => _backgroundImageUri = value;
	}
	public double BackgroundImageOpacity { get; set; }

	public ElementTheme Theme { get; set; }
}
