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
	public interface IDefaultBackgrounds
After:
namespace EventCountdowns.Core.DefaultData;

	public interface IDefaultBackgrounds
*/
namespace EventCountdowns.Core.DefaultData;

public interface IDefaultBackgrounds
{
	DefaultBackground[] GetDefaultBackgrounds();

	DefaultBackground GetSampleEventBackground(SampleEventTypes sampleEventKind);

/* Unmerged change from project 'EventCountdowns (net8.0)'
Removed:
}
*/
}
