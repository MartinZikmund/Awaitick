using Awaitick.Core.Models;

namespace Awaitick.Core.DefaultData;

public interface IEventPresetFactory
{
	EventCountdown Create(EventPreset eventPreset);
}
