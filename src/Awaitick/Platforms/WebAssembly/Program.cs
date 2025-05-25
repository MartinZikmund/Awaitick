using Uno.UI.Hosting;

CountdownsApp.InitializeLogging();

var host = UnoPlatformHostBuilder.Create()
	.App(() => new CountdownsApp())
	.Build();

await host.RunAsync();
