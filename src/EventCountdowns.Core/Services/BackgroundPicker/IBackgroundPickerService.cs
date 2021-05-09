using System.Threading.Tasks;

namespace EventCountdowns.Core.Services.BackgroundPicker
{
    public interface IBackgroundPickerService
    {
        Task<string> PickBackgroundAsync();
    }
}
