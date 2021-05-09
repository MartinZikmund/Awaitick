using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmCross.Binding.ValueConverters;
using MvvmCross.Core.ViewModels;
using EventCountdowns.Core.Models;
using EventCountdowns.Core.Services;
using EventCountdowns.Core.Services.ConfirmationDialog;
using EventCountdowns.Core.Services.Data;
using EventCountdowns.Core.Services.EventCountdownManager;
using EventCountdowns.Core.Services.ScheduledNotification;
using EventCountdowns.Core.Services.Share;
using EventCountdowns.Core.Services.Tile;

namespace EventCountdowns.Core.ViewModels
{
    public class CountdownDetailViewModel : BaseViewModel
    {
        public class NavigationModel
        {
            public NavigationModel()
            {

            }

            public NavigationModel( string countdownId )
            {
                CountdownId = countdownId;
            }

            public string CountdownId { get; set; }
        }

        private readonly IEventCountdownManager _eventCountdownManager;
        private readonly IDataService _dataService;
        private readonly ITileService _tileService;
        private readonly IScheduledNotificationService _scheduledNotificationService;
        private readonly ISystemSharingService _sharingService;
        private readonly IConfirmationDialogService _confirmationDialogService;
        private readonly ILocalizationService _localizationService;

        public CountdownDetailViewModel( IEventCountdownManager eventCountdownManager, IDataService dataService, ITileService tileService, IScheduledNotificationService scheduledNotificationService, ISystemSharingService sharingService, IConfirmationDialogService confirmationDialogService, ILocalizationService localizationService )
        {
            _eventCountdownManager = eventCountdownManager;
            _dataService = dataService;
            _tileService = tileService;
            _scheduledNotificationService = scheduledNotificationService;
            _sharingService = sharingService;
            _confirmationDialogService = confirmationDialogService;
            _localizationService = localizationService;
        }

        public async void Init( NavigationModel navigationModel )
        {
            EventCountdown = new EventCountdownObservable( await _dataService.GetCountdownAsync( navigationModel.CountdownId ) );
            if (EventCountdown != null)
            {
                TargetDateString = EventCountdown.TargetDateTime.ToString("f");
            }
            IsTilePinned = _tileService.IsCountdownPinned( EventCountdown.Id );
            _scheduledNotificationService.SuppressCountdownNotification( EventCountdown.Model );
        }


        private EventCountdownObservable _eventCountdown = null;

        public EventCountdownObservable EventCountdown
        {
            get { return _eventCountdown; }
            set { SetProperty( ref _eventCountdown, value ); }
        }

        private ICommand _deletePromptCommand = null;

        public ICommand DeletePromptCommand
            => _deletePromptCommand ?? ( _deletePromptCommand = new MvxCommand( DeletePrompt ) );

        private async void DeletePrompt()
        {
            //show delete dialog
            await
                _confirmationDialogService.ShowAsync( _localizationService.ConfirmDelete,
                    string.Format( _localizationService.AreYouSureDeleteTextFormat, EventCountdown.Name ), DeleteConfirmed,
                    () => { } );
        }

        private async void DeleteConfirmed()
        {
            await _eventCountdownManager.DeleteCountdownAsync( EventCountdown.Model );
            Close( this );
        }

        private ICommand _editCommand = null;

        public ICommand EditCommand => _editCommand ?? ( _editCommand = new MvxCommand( Edit ) );

        private void Edit()
        {
            ShowViewModel<CountdownEditorViewModel>( CountdownEditorViewModel.NavigationModel.CreateEdit( EventCountdown.Id ) );
        }

        private ICommand _shareCommand = null;

        public ICommand ShareCommand => _shareCommand ?? ( _shareCommand = new MvxCommand( Share ) );

        private void Share()
        {
            string sharedText = "";
            if ( EventCountdown.Finished )
            {
                sharedText = string.Format( _localizationService.SharingFinishedEventFormatString,
                    EventCountdown.CelebrationMessage, _localizationService.AppSocialHandle );
            }
            else
            {
                sharedText = string.Format( _localizationService.SharingFormatString,
                    EventCountdown.Name,
                    EventCountdown.DaysLeft,
                    EventCountdown.HoursLeft,
                    EventCountdown.MinutesLeft,
                    EventCountdown.TargetDateTime.ToString( "g" ),
                    _localizationService.AppSocialHandle );
            }
            _sharingService.ShareTextAsync( sharedText );
        }

        private string _targetDateString = "";

        public string TargetDateString
        {
            get { return _targetDateString; }
            set { SetProperty( ref _targetDateString, value ); }
        }

        private bool _isTilePinned = false;

        public bool IsTilePinned
        {
            get
            {
                return _isTilePinned;
            }
            set { SetProperty( ref _isTilePinned, value ); }
        }

        private ICommand _pinCommand = null;

        public ICommand PinCommand => _pinCommand ?? ( _pinCommand = new MvxCommand( Pin ) );

        private async void Pin()
        {
            IsTilePinned = await _tileService.PinCountdownAsync( EventCountdown.Model );
            _tileService.UpdateCountdownTile( EventCountdown.Model );
            _tileService.ScheduleCountdownNotification( EventCountdown.Model );
        }

        private ICommand _unPinCommand = null;

        public ICommand UnPinCommand => _unPinCommand ?? ( _unPinCommand = new MvxCommand( UnPin ) );

        private async void UnPin()
        {
            var unpinSuccessful = await _tileService.UnpinCountdownAsync( EventCountdown.Model );
            IsTilePinned = !unpinSuccessful;
        }


        public void UpdateCountdowns()
        {
            EventCountdown?.UpdateBindings();
        }
    }
}
