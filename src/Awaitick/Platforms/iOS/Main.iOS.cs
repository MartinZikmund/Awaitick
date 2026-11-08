using Awaitick.iOS;
using Uno.UI.Hosting;
using UserNotifications;

CountdownsApp.InitializeLogging();

// Uno 7 keeps LocalSettings in its own suite and does not bring over what earlier versions wrote.
try
{
	Uno.Storage.ApplicationDataMigrator.MigrateSettings();
}
catch (IOException)
{
	// The old values stay where they are, so the next launch retries.
}

// Register the notification delegate before building the host
UNUserNotificationCenter.Current.Delegate = new NotificationDelegate();

var host = UnoPlatformHostBuilder.Create()
	.App(() => new CountdownsApp())
	.UseAppleUIKit()
	.Build();

host.Run();
