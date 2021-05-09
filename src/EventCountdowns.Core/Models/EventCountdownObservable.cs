using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;

namespace EventCountdowns.Core.Models
{
    public class EventCountdownObservable : MvxNotifyPropertyChanged
    {
        private readonly EventCountdown _eventCountdown;

        public EventCountdownObservable(EventCountdown eventCountdown)
        {
            _eventCountdown = eventCountdown;
        }

        public EventCountdown Model => _eventCountdown;

        public string Id => _eventCountdown.Id;

        public string Name => _eventCountdown.Name;

        public string BackgroundImagePath => _eventCountdown.BackgroundImagePath;

        public bool Finished => _eventCountdown.TargetDateTime < DateTimeOffset.Now;

        public TimeSpan TimeLeft => _eventCountdown.TargetDateTime - DateTimeOffset.Now;

        public int DaysLeft => TimeLeft.Days;

        public int HoursLeft => TimeLeft.Hours;

        public int MinutesLeft => TimeLeft.Minutes;

        public int SecondsLeft => TimeLeft.Seconds;

        public DateTimeOffset TargetDateTime => _eventCountdown.TargetDateTime;

        public string CelebrationMessage => _eventCountdown.CelebrationMessage;

        public void UpdateBindings()
        {
            RaisePropertyChanged(() => DaysLeft);
            RaisePropertyChanged(() => HoursLeft);
            RaisePropertyChanged(() => MinutesLeft);
            RaisePropertyChanged(() => SecondsLeft);
            RaisePropertyChanged(() => TimeLeft);
            RaisePropertyChanged(() => Finished);
        }
    }
}
