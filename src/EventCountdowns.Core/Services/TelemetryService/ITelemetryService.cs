namespace EventCountdowns.Core.Services.TelemetryService
{
    public interface ITelemetryService
    {
        void TrackEvent(string eventName);
    }
}
