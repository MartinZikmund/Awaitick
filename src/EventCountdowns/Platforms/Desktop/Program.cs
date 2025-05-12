using Uno.UI.Hosting;

CountdownsApp.InitializeLogging();

var host = UnoPlatformHostBuilder.Create()
	.App(() => new CountdownsApp())
	.UseX11()
	.UseLinuxFrameBuffer()
	.UseMacOS()
	.UseWin32()
	.Build();

host.Run();
