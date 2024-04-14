using Microsoft.UI.Xaml;


/* Unmerged change from project 'EventCountdowns (net8.0)'
Before:
namespace EventCountdowns.Core.Resources
{
	public class ResourceAccessor
After:
namespace EventCountdowns.Core.Resources;

	public class ResourceAccessor
*/
namespace EventCountdowns.Core.Resources;

public class ResourceAccessor
{
    public static T GetResource<T>(string key) => (T)Application.Current.Resources[key];


/* Unmerged change from project 'EventCountdowns (net8.0)'
Before:
/* Unmerged change from project 'EventCountdowns (net8.0)'
Removed:
}
*/
After:
    /* Unmerged change from project 'EventCountdowns (net8.0)'
    Removed:
    }
    */
*/
	/* Unmerged change from project 'EventCountdowns (net8.0)'
	Removed:
	}
	*/
}
