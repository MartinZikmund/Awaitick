using Microsoft.UI.Xaml;

namespace EventCountdowns.Core.Resources;

public class ResourceAccessor
{
    public static T GetResource<T>(string key) => (T)Application.Current.Resources[key];
}
