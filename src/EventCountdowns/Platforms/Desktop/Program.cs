using Uno.UI.Runtime.Skia;

namespace EventCountdowns;
public class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        CountdownsApp.InitializeLogging();

		var host = SkiaHostBuilder.Create()
            .App(() => new CountdownsApp())
            .UseX11()
            .UseLinuxFrameBuffer()
            .UseMacOS()
            .UseWin32()
            .Build();

        host.Run();
    }
}
