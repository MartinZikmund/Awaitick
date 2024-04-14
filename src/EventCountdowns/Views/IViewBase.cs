using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/* Unmerged change from project 'EventCountdowns (net8.0)'
Before:
namespace EventCountdowns.Views
{
	public interface IViewBase
After:
namespace EventCountdowns.Views;

	public interface IViewBase
*/
namespace EventCountdowns.Views;

public interface IViewBase
{
	object Model { get; }

/* Unmerged change from project 'EventCountdowns (net8.0)'
Removed:
}
*/
}
