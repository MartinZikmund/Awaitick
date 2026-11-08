using Uno.UI.Hosting;

namespace Awaitick;

internal static class Program
{
	// The Win32 host needs an STA main thread for OLE (drag and drop), and top-level statements can't declare one.
	[STAThread]
	public static void Main()
	{
		CountdownsApp.InitializeLogging();

		var host = UnoPlatformHostBuilder.Create()
			.App(() => new CountdownsApp())
			.UseX11()
			.UseLinuxFrameBuffer()
			.UseMacOS()
			.UseWin32()
			.Build();

		host.Run();
	}
}
