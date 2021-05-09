using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using EventCountdowns.Core.Services.TelemetryService;

namespace EventCountdowns.Core.Services
{
    public class TelemetryService : ITelemetryService
    {
        private readonly TelemetryClient _telemetryClient = new TelemetryClient();

        public void TrackEvent( string eventName )
        {
            _telemetryClient.TrackEvent( eventName );
        }
    }
}
