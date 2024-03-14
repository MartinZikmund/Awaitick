using System.Threading.Tasks;
using EventCountdowns.Core.Models;

namespace EventCountdowns.Core.Services.Tiles
{
    public interface ITileService
    {
        Task<bool> PinCountdownAsync(EventCountdown eventCountdown);
        Task<bool> UnpinCountdownAsync(EventCountdown eventCountdown);
        bool IsCountdownPinned(string id);
        void UpdateCountdownTile(EventCountdown eventCountdown);
        void UpdateMainTile(params EventCountdown[] countdowns);

        void ScheduleCountdownNotification( EventCountdown eventCountdown );

        void UnscheduleCountdownNotification( EventCountdown eventCountdown );
    }
}
