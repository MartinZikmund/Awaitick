using System.Threading.Tasks;

namespace EventCountdowns.Core.Services.Share
{
    public interface ISystemSharingService
    {
        Task ShareTextAsync(string data);
    }
}
