using Uno.UI.Hosting;

CountdownsApp.InitializeLogging();

var host = UnoPlatformHostBuilder.Create()
	.App(() => new CountdownsApp())
	.UseWebAssembly()
	.Build();

await host.RunAsync();
