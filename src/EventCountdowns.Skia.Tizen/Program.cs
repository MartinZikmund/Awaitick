using Tizen.Applications;
using Uno.UI.Runtime.Skia;

namespace EventCountdowns.Skia.Tizen
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new TizenHost(() => new EventCountdowns.App(), args);
            host.Run();
        }
    }
}
