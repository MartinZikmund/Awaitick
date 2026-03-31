namespace Awaitick.Core.Services.ScheduledNotification;

public interface INotificationPermissionService
{
	/// <summary>
	/// Requests notification permission with pre-permission dialog and denied guidance.
	/// Returns true if permission was granted, false otherwise.
	/// </summary>
	Task<bool> RequestPermissionWithDialogsAsync();
}
