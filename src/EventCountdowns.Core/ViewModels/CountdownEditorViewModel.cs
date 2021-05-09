using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmCross.Core.Parse.StringDictionary;
using MvvmCross.Core.ViewModels;
using MvvmCross.Core.Views;
using EventCountdowns.Core.DefaultData;
using EventCountdowns.Core.Models;
using EventCountdowns.Core.Services;
using EventCountdowns.Core.Services.BackgroundPicker;
using EventCountdowns.Core.Services.Data;
using EventCountdowns.Core.Services.EventCountdownManager;

namespace EventCountdowns.Core.ViewModels
{
    public class CountdownEditorViewModel : BaseViewModel
    {
        private readonly IEventCountdownManager _eventCountdownManager;
        private readonly IBackgroundPickerService _backgroundPickerService;
        private readonly IDataService _dataService;
        private readonly ILocalizationService _localizationService;
        private readonly IDefaultBackgrounds _defaultBackgrounds;

        public enum EditorMode
        {
            Add, Edit
        }

        public class NavigationModel
        {
            public NavigationModel()
            {

            }

            private NavigationModel( string id )
            {
                if ( id == null ) throw new ArgumentNullException( nameof( id ) );
                Mode = EditorMode.Edit;
                Id = id;
            }

            public static NavigationModel CreateAdd()
            {
                return new NavigationModel();
            }

            public static NavigationModel CreateEdit( string id )
            {
                return new NavigationModel( id );
            }

            public string Id { get; set; }
            public EditorMode Mode { get; set; } = EditorMode.Add;
        }

        public CountdownEditorViewModel( IEventCountdownManager eventCountdownManager, IBackgroundPickerService backgroundPickerService, IDataService dataService, ILocalizationService localizationService, IDefaultBackgrounds defaultBackgrounds )
        {
            _eventCountdownManager = eventCountdownManager;
            _backgroundPickerService = backgroundPickerService;
            _dataService = dataService;
            _localizationService = localizationService;
            _defaultBackgrounds = defaultBackgrounds;
        }

        public async void Init( NavigationModel model )
        {
            Mode = model.Mode;

            if ( Mode == EditorMode.Edit )
            {
                _editedEventCountdown = await _dataService.GetCountdownAsync( model.Id );
                if ( _editedEventCountdown != null )
                {
                    LoadEditedCountdown();
                }
                else
                {
                    Close( this );
                }
            }

            if ( DefaultBackgrounds.Count == 0 )
            {
                var defaultBackgrounds = _defaultBackgrounds.GetDefaultBackgrounds();
                foreach ( var background in defaultBackgrounds )
                {
                    DefaultBackgrounds.Add( background );
                }
            }
        }

        private DefaultBackground _selectedDefaultBackground = null;

        public DefaultBackground SelectedDefaultBackground
        {
            get
            {
                return _selectedDefaultBackground;
            }
            set
            {
                SetProperty( ref _selectedDefaultBackground, value );
                if ( _selectedDefaultBackground != null )
                {
                    BackgroundPath = _selectedDefaultBackground.BackgroundPath;
                }
                else
                {
                    BackgroundPath = LastCustomBackgroundPath;
                }
            }
        }

        private string _lastCustomBackgroundPath = null;

        public string LastCustomBackgroundPath
        {
            get
            {
                return _lastCustomBackgroundPath;
            }
            set { SetProperty( ref _lastCustomBackgroundPath, value ); }
        }

        private string _backgroundPath = null;

        public string BackgroundPath
        {
            get { return _backgroundPath; }
            set
            {
                SetProperty( ref _backgroundPath, value );
                LastCustomBackgroundPath = value;
            }
        }

        private void LoadEditedCountdown()
        {
            if ( _editedEventCountdown == null ) throw new NullReferenceException( "Edited Countdown is null" );
            Name = _editedEventCountdown.Name;
            Date = _editedEventCountdown.TargetDateTime.Date;
            Time = _editedEventCountdown.TargetDateTime.TimeOfDay;
            CelebrationMessage = _editedEventCountdown.CelebrationMessage;
            BackgroundPath = _editedEventCountdown.BackgroundImagePath;
            LastCustomBackgroundPath = BackgroundPath;
        }

        public ObservableCollection<DefaultBackground> DefaultBackgrounds { get; } = new ObservableCollection<DefaultBackground>();

        private EventCountdown _editedEventCountdown = null;

        private EditorMode _mode = EditorMode.Add;

        public EditorMode Mode
        {
            get { return _mode; }
            set { SetProperty( ref _mode, value ); }
        }

        private string _name = "";

        public string Name
        {
            get { return _name; }
            set
            {
                SetProperty( ref _name, value );
                RaisePropertyChanged( () => DefaultCelebrationMessage );
            }
        }

        private DateTimeOffset _date = DateTimeOffset.UtcNow.AddDays( 1 );
        public DateTimeOffset Date
        {
            get { return _date; }
            set { SetProperty( ref _date, value ); }
        }

        private TimeSpan _time = TimeSpan.Zero;

        public TimeSpan Time
        {
            get { return _time; }
            set
            {
                SetProperty( ref _time, value );
            }
        }

        private string _celebrationMessage = "";

        public string CelebrationMessage
        {
            get { return _celebrationMessage; }
            set
            {
                SetProperty( ref _celebrationMessage, value );
            }
        }

        public string DefaultCelebrationMessage => string.Format( _localizationService.DefaultCelebration, Name );

        private ICommand _cancelCommand = null;
        public ICommand CancelCommand => _cancelCommand ?? ( _cancelCommand = new MvxCommand( Cancel ) );

        private void Cancel()
        {
            Close( this );
        }

        private ICommand _chooseYourImageCommand = null;

        public ICommand ChooseYourImageCommand
            => _chooseYourImageCommand ?? ( _chooseYourImageCommand = new MvxCommand( ChooseYourImage ) );

        private async void ChooseYourImage()
        {
            IsWorking = true;
            BackgroundPath = ( await _backgroundPickerService.PickBackgroundAsync() ) ?? LastCustomBackgroundPath;
            IsWorking = false;
        }

        private ICommand _saveCommand = null;
        public ICommand SaveCommand => _saveCommand ?? ( _saveCommand = new MvxCommand( Save ) );

        private async void Save()
        {
            IsWorking = true;
            if ( Mode == EditorMode.Add )
            {
                _editedEventCountdown = new EventCountdown() { Id = Guid.NewGuid().ToString() };
            }
            _editedEventCountdown.Name = Name;
            TimeSpan fixedTime = new TimeSpan( Time.Hours, Time.Minutes, 0 );
            _editedEventCountdown.TargetDateTime = Date.Date + fixedTime;
            _editedEventCountdown.BackgroundImagePath = BackgroundPath;
            _editedEventCountdown.CelebrationMessage = string.IsNullOrWhiteSpace( CelebrationMessage ) ? string.Format( _localizationService.DefaultCelebration, Name ) : CelebrationMessage;
            if ( Mode == EditorMode.Edit )
            {
                await _eventCountdownManager.UpdateCountdownAsync( _editedEventCountdown );
            }
            else
            {
                await _eventCountdownManager.AddCountdownAsync( _editedEventCountdown );
            }
            IsWorking = false;
            Close( this );
        }
    }
}
