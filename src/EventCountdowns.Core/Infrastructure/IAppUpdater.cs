using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventCountdowns.Core.Infrastructure
{
    public interface IAppUpdater
    {
        Task EnsureAppUpToDateAsync();
    }
}
