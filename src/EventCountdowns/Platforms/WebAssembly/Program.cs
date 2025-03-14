namespace EventCountdowns;

public class Program
{
    private static CountdownsApp? _app;

    public static int Main(string[] args)
    {
        CountdownsApp.InitializeLogging();

        Microsoft.UI.Xaml.Application.Start(_ => _app = new CountdownsApp());

        return 0;
    }
}
