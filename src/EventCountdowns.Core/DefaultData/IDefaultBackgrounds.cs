using EventCountdowns.Core.Models;

namespace EventCountdowns.Core.DefaultData;

public interface IDefaultBackgrounds
{
	DefaultBackground[] GetDefaultBackgrounds();

	DefaultBackground GetSampleEventBackground(SampleEventTypes sampleEventKind);
}
