using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventCountdowns.Core.Services.BackgroundTasks
{
    public interface IBackgroundTasksService
    {
        Task<bool> RegisterAsync();

        Task<bool> UnregisterAsync();
    }
}
