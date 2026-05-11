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
		BackgroundColor = entity.BackgroundColor,
	};
}
