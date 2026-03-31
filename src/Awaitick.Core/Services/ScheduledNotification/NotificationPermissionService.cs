using MZikmund.Services.Dialogs;

namespace Awaitick.Core.Services.ScheduledNotification;

public class NotificationPermissionService : INotificationPermissionService
{
	private readonly IScheduledNotificationService _scheduledNotificationService;
	private readonly IDialogService _dialogService;
	private readonly IStringLocalizer _stringLocalizer;

	public NotificationPermissionService(
		IScheduledNotificationService scheduledNotificationService,
		IDialogService dialogService,
		IStringLocalizer stringLocalizer)
	{
		_scheduledNotificationService = scheduledNotificationService;
		_dialogService = dialogService;
		_stringLocalizer = stringLocalizer;
	}

	public async Task<bool> RequestPermissionWithDialogsAsync()
	{
		if (_scheduledNotificationService.HasPermission)
		{
			return true;
		}

		// Show pre-permission dialog
		var preDialog = new ContentDialog
		{
			Title = _stringLocalizer.GetString("NotificationPermission_PreDialog_Title"),
			Content = _stringLocalizer.GetString("NotificationPermission_PreDialog_Content"),
			PrimaryButtonText = _stringLocalizer.GetString("Continue"),
			CloseButtonText = _stringLocalizer.GetString("Cancel"),
			DefaultButton = ContentDialogButton.Primary,
		};

		var preResult = await _dialogService.ShowAsync(preDialog);
		if (preResult != ContentDialogResult.Primary)
		{
			return false;
		}

		// Request OS permission
		var granted = await _scheduledNotificationService.RequestPermissionAsync();
		if (granted)
		{
			return true;
		}

		// Show denied guidance dialog
		var deniedDialog = new ContentDialog
		{
			Title = _stringLocalizer.GetString("NotificationPermission_Denied_Title"),
			Content = _stringLocalizer.GetString("NotificationPermission_Denied_Content"),
			PrimaryButtonText = _stringLocalizer.GetString("NotificationPermission_OpenSettings"),
			CloseButtonText = _stringLocalizer.GetString("Cancel"),
			DefaultButton = ContentDialogButton.Primary,
		};

		var deniedResult = await _dialogService.ShowAsync(deniedDialog);
		if (deniedResult == ContentDialogResult.Primary)
		{
			await _scheduledNotificationService.OpenNotificationSettingsAsync();
		}

		return false;
	}
}
