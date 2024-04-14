using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventCountdowns.Core.Models;

namespace EventCountdowns.Core.Services.EventCountdownManager;

public interface IEventCountdownManager
{
	Task AddCountdownAsync(EventCountdown eventCountdown);
	Task UpdateCountdownAsync(EventCountdown eventCountdown);
	Task DeleteCountdownAsync(EventCountdown eventCountdown);
}
