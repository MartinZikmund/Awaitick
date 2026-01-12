using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using Awaitick.Core.Infrastructure;
using Awaitick.Core.Services.DeepLink;

namespace Awaitick.Droid;

[Activity(
	MainLauncher = true,
	LaunchMode = LaunchMode.SingleTop, // Important for handling notification taps when app is running
	ConfigurationChanges = global::Uno.UI.ActivityHelper.AllConfigChanges,
	WindowSoftInputMode = SoftInput.AdjustNothing | SoftInput.StateHidden
)]
public class MainActivity : Microsoft.UI.Xaml.ApplicationActivity
{
	protected override void OnCreate(Bundle? bundle)
	{
		global::AndroidX.Core.SplashScreen.SplashScreen.InstallSplashScreen(this);

		base.OnCreate(bundle);

		// Handle notification tap that launched the app
		HandleIntent(Intent);
	}

	protected override void OnNewIntent(Intent? intent)
	{
		base.OnNewIntent(intent);

		// Handle notification tap when app is already running
		HandleIntent(intent);
	}

	private void HandleIntent(Intent? intent)
	{
		if (intent == null)
		{
			return;
		}

		var countdownId = intent.GetStringExtra(NotificationAlarmReceiver.ExtraCountdownId);
		if (!string.IsNullOrEmpty(countdownId))
		{
			// Set pending navigation for the deep link service to handle
			try
			{
				var deepLinkService = IoC.GetService<IDeepLinkService>();
				deepLinkService?.SetPendingNavigation(countdownId);
			}
			catch
			{
				// IoC may not be initialized yet during cold start
				// The deep link will be handled when MainViewModel loads
			}
		}
	}
}
