using Awaitick.iOS;
using Uno.UI.Hosting;
using UserNotifications;

CountdownsApp.InitializeLogging();

// Register the notification delegate before building the host
UNUserNotificationCenter.Current.Delegate = new NotificationDelegate();

var host = UnoPlatformHostBuilder.Create()
	.App(() => new CountdownsApp())
	.UseAppleUIKit()
	.Build();

host.Run();
