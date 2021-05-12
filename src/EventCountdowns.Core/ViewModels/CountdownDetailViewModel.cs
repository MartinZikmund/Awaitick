using System;
using System.Threading.Tasks;
using System.Windows.Input;
using EventCountdowns.Core.Models;
using EventCountdowns.Core.Services;
using EventCountdowns.Core.Services.ConfirmationDialog;
using EventCountdowns.Core.Services.Data;
using EventCountdowns.Core.Services.EventCountdownManager;
using EventCountdowns.Core.Services.ScheduledNotification;
using EventCountdowns.Core.Services.Share;
using EventCountdowns.Core.Services.Tiles;

namespace EventCountdowns.Core.ViewModels
{
    public class CountdownDetailViewModel : ViewModel
    {
        public class NavigationModel
        {
            public NavigationModel()
            {
            }

            public NavigationModel(string countdownId)
            {
                CountdownId = countdownId;
            }

            public string CountdownId { get; set; } = string.Empty;
        }

        private readonly IEventCountdownManager _eventCountdownManager;
        private readonly IDataService _dataService;
        private readonly ITileService _tileService;
        private readonly IScheduledNotificationService _scheduledNotificationService;
        private readonly ISystemSharingService _sharingService;
        private readonly IConfirmationDialogService _confirmationDialogService;
        private readonly ILocalizationService _localizationService;

        private EventCountdownObservable? _eventCountdown = null;
        private bool _isTilePinned = false;
        private string _targetDateString = "";

        public CountdownDetailViewModel(IEventCountdownManager eventCountdownManager, IDataService dataService, ITileService tileService, IScheduledNotificationService scheduledNotificationService, ISystemSharingService sharingService, IConfirmationDialogService confirmationDialogService, ILocalizationService localizationService)
        {
            _eventCountdownManager = eventCountdownManager;
            _dataService = dataService;
            _tileService = tileService;
            _scheduledNotificationService = scheduledNotificationService;
            _sharingService = sharingService;
            _confirmationDialogService = confirmationDialogService;
            _localizationService = localizationService;
        }

        public override async Task LoadAsync(object? parameter)
        {
            if (parameter is not NavigationModel navigationModel)
            {
                throw new ArgumentException("Parameter must be CountdownDetailViewModel.NavigationModel.", nameof(parameter));
            }

            EventCountdown = new EventCountdownObservable(await _dataService.GetCountdownAsync(navigationModel.CountdownId));
            if (EventCountdown != null)
            {
                TargetDateString = EventCountdown.TargetDateTime.ToString("f");
                IsTilePinned = _tileService.IsCountdownPinned(EventCountdown.Id);
                _scheduledNotificationService.SuppressCountdownNotification(EventCountdown.Model);
            }
        }

        public EventCountdownObservable EventCountdown
        {
            get => _eventCountdown;
            set => SetProperty(ref _eventCountdown, value);
        }

        public ICommand DeletePromptCommand => GetOrCreateCommand(DeletePrompt);

        private async void DeletePrompt()
        {
            //show delete dialog
            await
                _confirmationDialogService.ShowAsync(_localizationService.ConfirmDelete,
                    string.Format(_localizationService.AreYouSureDeleteTextFormat, EventCountdown.Name), DeleteConfirmed,
                    () => { });
        }

        private async void DeleteConfirmed()
        {
            await _eventCountdownManager.DeleteCountdownAsync(EventCountdown.Model);
            Navigation.GoBack();
        }


        public ICommand EditCommand => GetOrCreateCommand(Edit);

        private void Edit()
        {
            Navigation.Navigate<CountdownEditorViewModel>(CountdownEditorViewModel.NavigationModel.CreateEdit(EventCountdown.Id));
        }

        public ICommand ShareCommand => GetOrCreateCommand(Share);

        private void Share()
        {
            string sharedText = "";
            if (EventCountdown.Finished)
            {
                sharedText = string.Format(_localizationService.SharingFinishedEventFormatString,
                    EventCountdown.CelebrationMessage, _localizationService.AppSocialHandle);
            }
            else
            {
                sharedText = string.Format(_localizationService.SharingFormatString,
                    EventCountdown.Name,
                    EventCountdown.DaysLeft,
                    EventCountdown.HoursLeft,
                    EventCountdown.MinutesLeft,
                    EventCountdown.TargetDateTime.ToString("g"),
                    _localizationService.AppSocialHandle);
            }
            _sharingService.ShareTextAsync(sharedText);
        }


        public string TargetDateString
        {
            get => _targetDateString;
            set => SetProperty(ref _targetDateString, value);
        }

        public bool IsTilePinned
        {
            get => _isTilePinned;
            set => SetProperty(ref _isTilePinned, value);
        }

        public ICommand PinCommand => GetOrCreateCommand(Pin);

        private async void Pin()
        {
            IsTilePinned = await _tileService.PinCountdownAsync(EventCountdown.Model);
            _tileService.UpdateCountdownTile(EventCountdown.Model);
            _tileService.ScheduleCountdownNotification(EventCountdown.Model);
        }

        public ICommand UnPinCommand => GetOrCreateCommand(UnPin);

        private async void UnPin()
        {
            var unpinSuccessful = await _tileService.UnpinCountdownAsync(EventCountdown.Model);
            IsTilePinned = !unpinSuccessful;
        }


        public void UpdateCountdowns()
        {
            EventCountdown?.UpdateBindings();
        }
    }
}
