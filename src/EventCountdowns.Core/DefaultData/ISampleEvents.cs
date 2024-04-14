using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventCountdowns.Core.Models;


/* Unmerged change from project 'EventCountdowns (net8.0)'
Before:
namespace EventCountdowns.Core.DefaultData
{
	public interface ISampleEvents
After:
namespace EventCountdowns.Core.DefaultData;

	public interface ISampleEvents
*/
namespace EventCountdowns.Core.DefaultData;

public interface ISampleEvents
{
	EventCountdown[] GetSampleEvents();

/* Unmerged change from project 'EventCountdowns (net8.0)'
Removed:
}
*/
}
