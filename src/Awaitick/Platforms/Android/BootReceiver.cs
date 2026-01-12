#if __ANDROID__
using Android.App;
using Android.Content;

namespace Awaitick.Droid;

/// <summary>
/// BroadcastReceiver that handles device boot completed events.
/// Used to reschedule notifications after device restart.
/// </summary>
[BroadcastReceiver(Enabled = true, Exported = false)]
[IntentFilter(new[] { Intent.ActionBootCompleted })]
public class BootReceiver : BroadcastReceiver
{
	public override void OnReceive(Context? context, Intent? intent)
	{
		if (context == null || intent == null)
		{
			return;
		}

		if (intent.Action != Intent.ActionBootCompleted)
		{
			return;
		}

		// The actual rescheduling will happen when the app starts.
		// This receiver ensures the app has the opportunity to reschedule notifications.
		// For immediate rescheduling, we would need to start a service or JobScheduler,
		// but for this app, notifications will be rescheduled when the user opens the app.

		// Note: If immediate rescheduling is required, consider using WorkManager
		// to schedule a one-time job that reschedules all notifications.
	}
}
#endif
