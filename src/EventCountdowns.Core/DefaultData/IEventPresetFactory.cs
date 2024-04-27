using EventCountdowns.Core.Models;

namespace EventCountdowns.Core.DefaultData;

public interface IEventPresetFactory
{
	EventCountdown Create(EventPreset eventPreset);
}
