using Awaitick.Core.Models;
using Awaitick.Core.Services.ScheduledNotification;

namespace Awaitick.Services.ScheduledNotification;

#if __WASM__
public class ScheduledNotificationService : IScheduledNotificationService
{
	private const int ScheduleNotificationFutureSecondsLimit = 3;

	public ScheduledNotificationService()
	{
		// Request notification permission on initialization
		RequestNotificationPermission();
	}

	public void ScheduleCountdownNotification(EventCountdown eventCountdown)
	{
		try
		{
			if ((eventCountdown.TargetDateTime - DateTimeOffset.Now).TotalSeconds >
				ScheduleNotificationFutureSecondsLimit)
			{
				var id = eventCountdown.Id;
				var title = EscapeJavaScriptString(eventCountdown.Name ?? string.Empty);
				var body = EscapeJavaScriptString(eventCountdown.CelebrationMessage ?? string.Empty);
				var targetDateTime = eventCountdown.TargetDateTime.ToString("O"); // ISO 8601 format
				var imageUrl = EscapeJavaScriptString(eventCountdown.BackgroundImageUri?.ToString() ?? string.Empty);

				var script = $"AwaitickNotificationService.scheduleNotification('{id}', '{title}', '{body}', '{targetDateTime}', '{imageUrl}')";
				Uno.Foundation.WebAssemblyRuntime.InvokeJS(script);
			}
		}
		catch (Exception ex)
		{
			// Log error but don't crash the app
			Console.WriteLine($"Error scheduling notification: {ex.Message}");
		}
	}

	public void UnscheduleCountdownNotification(EventCountdown eventCountdown)
	{
		try
		{
			var id = eventCountdown.Id;
			var script = $"AwaitickNotificationService.unscheduleNotification('{id}')";
			Uno.Foundation.WebAssemblyRuntime.InvokeJS(script);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error unscheduling notification: {ex.Message}");
		}
	}

	public void SuppressCountdownNotification(EventCountdown eventCountdown)
	{
		// For WASM, suppressing is the same as unscheduling
		UnscheduleCountdownNotification(eventCountdown);
	}

	public void UnSuppressAllCountdownNotifications()
	{
		// For WASM, we don't need to do anything here
		// as notifications will be rescheduled when needed
	}

	private void RequestNotificationPermission()
	{
		try
		{
			var script = "AwaitickNotificationService.requestPermission()";
			Uno.Foundation.WebAssemblyRuntime.InvokeJS(script);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error requesting notification permission: {ex.Message}");
		}
	}

	private static string EscapeJavaScriptString(string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			return string.Empty;
		}

		return input
			.Replace("\\", "\\\\")
			.Replace("'", "\\'")
			.Replace("\"", "\\\"")
			.Replace("\n", "\\n")
			.Replace("\r", "\\r")
			.Replace("\t", "\\t");
	}
}
#endif
