using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventCountdowns.Core.Models;

namespace EventCountdowns.Core.DefaultData
{
    public interface ISampleEvents
    {
        EventCountdown[] GetSampleEvents();
    }
}
