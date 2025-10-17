
using Microsoft.UI;

namespace Awaitick.Core.Models;

public class EventCountdown
{
	public EventCountdown()
	{
	}

	public string Id { get; set; }

	public string Name { get; set; }

	public string CelebrationMessage { get; set; }

	public DateTimeOffset TargetDateTime { get; set; } = DateTimeOffset.Now.AddDays(1);

	public Uri? BackgroundImageUri { get; set; }

	public TextTheme TextTheme { get; set; }

	public double BackgroundImageOpacity { get; set; } = 0.8;

	public string BackgroundColor { get; set; } = ColorHelper.ToHex(Colors.Transparent);
}
