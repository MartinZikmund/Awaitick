using Awaitick.Core.Models;
using Awaitick.Core.Models.Database;

namespace Awaitick.Core.Services.Data;

internal static class EventCountdownMapper
{
	public static EventCountdownEntity ToEntity(EventCountdown source) => new()
	{
		Id = source.Id,
		Name = source.Name,
		CelebrationMessage = source.CelebrationMessage,
		TargetDateTime = source.TargetDateTime,
		BackgroundImageUri = source.BackgroundImageUri?.ToString(),
		TextTheme = (int)source.TextTheme,
		BackgroundImageOpacity = source.BackgroundImageOpacity,
		BackgroundImageVerticalPosition = source.BackgroundImageVerticalPosition,
		BackgroundColor = source.BackgroundColor,
	};

	public static EventCountdown ToModel(EventCountdownEntity entity) => new()
	{
		Id = entity.Id,
		Name = entity.Name,
		CelebrationMessage = entity.CelebrationMessage,
		TargetDateTime = entity.TargetDateTime,
		BackgroundImageUri = string.IsNullOrEmpty(entity.BackgroundImageUri) ? null : new Uri(entity.BackgroundImageUri),
		TextTheme = (TextTheme)entity.TextTheme,
		BackgroundImageOpacity = entity.BackgroundImageOpacity,
		// Legacy rows have no stored value; clamp to [0,1] and fall back to centered (0.5).
		BackgroundImageVerticalPosition = entity.BackgroundImageVerticalPosition is double position && position >= 0 && position <= 1 ? position : 0.5,
		BackgroundColor = entity.BackgroundColor,
	};
}
