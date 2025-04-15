using Uno.UI.Runtime.Skia.WebAssembly.Browser;

namespace EventCountdowns;

public class Program
{
    public static async Task Main(string[] args)
    {
        CountdownsApp.InitializeLogging();

		var host = new WebAssemblyBrowserHost(() => new CountdownsApp());
		await host.Run();
    }
}
