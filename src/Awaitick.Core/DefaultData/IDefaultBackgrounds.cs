using Awaitick.Core.Models;

namespace Awaitick.Core.DefaultData;

public interface IDefaultBackgrounds
{
	DefaultBackground[] GetDefaultBackgrounds();

	DefaultBackground GetSampleEventBackground(EventPreset sampleEventKind);
}
