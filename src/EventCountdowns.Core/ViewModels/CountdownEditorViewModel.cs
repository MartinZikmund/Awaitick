using System.Collections.ObjectModel;
using System.Globalization;
using EventCountdowns.Core.DefaultData;
using EventCountdowns.Core.Models;
using EventCountdowns.Core.Services.BackgroundPicker;
using EventCountdowns.Core.Services.Data;
using EventCountdowns.Core.Services.EventCountdownManager;
using EventCountdowns.Services.Navigation;
using EventCountdowns.ViewModels;

namespace EventCountdowns.Core.ViewModels;

public partial class CountdownEditorViewModel : PageViewModel
{
	public enum EditorMode
	{
		Add, Edit
	}

	public class NavigationModel
	{
		public NavigationModel()
		{
		}

		private NavigationModel(string id)
		{
			if (id == null) throw new ArgumentNullException(nameof(id));
			Mode = EditorMode.Edit;
			Id = id;
		}

		public static NavigationModel CreateAdd()
		{
			return new NavigationModel();
		}

		public static NavigationModel CreateEdit(string id)
		{
			return new NavigationModel(id);
		}

		public string Id { get; set; } = "";

		public EditorMode Mode { get; set; } = EditorMode.Add;
	}

	private readonly IEventCountdownManager _eventCountdownManager;
	private readonly IBackgroundPickerService _backgroundPickerService;
	private readonly IDataService _dataService;
	private readonly IStringLocalizer _localizationService;
	private readonly INavigationService _navigationService;
	private readonly IDefaultBackgrounds _defaultBackgrounds;

	[ObservableProperty]
	private DefaultBackground? _selectedDefaultBackground;

	[ObservableProperty]
	private EditorMode _mode = EditorMode.Add;

	[ObservableProperty]
	private Uri? _lastCustomBackgroundUri;

	[ObservableProperty]
	private Uri? _backgroundUri = new Uri("ms-appx:///EventCountdowns/Assets/SampleBackgrounds/Thumbnails/BlankBackground.png", UriKind.Absolute);

	[ObservableProperty]
	private string _name = "";

	[ObservableProperty]
	private DateTimeOffset _date = DateTimeOffset.UtcNow.AddDays(1);

	[ObservableProperty]
	private TimeSpan _time = TimeSpan.Zero;

	[ObservableProperty]
	private string _celebrationMessage = "";

	private EventCountdown? _editedEventCountdown;

	public CountdownEditorViewModel(
		IEventCountdownManager eventCountdownManager,
		IBackgroundPickerService backgroundPickerService,
		IDataService dataService,
		IStringLocalizer localizationService,
		INavigationService navigationService,
		IDefaultBackgrounds defaultBackgrounds)
	{
		_eventCountdownManager = eventCountdownManager ?? throw new ArgumentNullException(nameof(eventCountdownManager));
		_backgroundPickerService = backgroundPickerService ?? throw new ArgumentNullException(nameof(backgroundPickerService));
		_dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
		_localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
		_navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
		_defaultBackgrounds = defaultBackgrounds ?? throw new ArgumentNullException(nameof(defaultBackgrounds));
	}

	public ObservableCollection<DefaultBackground> DefaultBackgrounds { get; } = new();

	public string DefaultCelebrationMessage => string.Format(CultureInfo.CurrentCulture, _localizationService.GetString("DefaultCelebration"), Name);

	public override async void ViewNavigatedTo(object? parameter)
	{
		if (parameter is not NavigationModel navigationModel)
		{
			throw new ArgumentException("Parameter must be CountdownDetailViewModel.NavigationModel.", nameof(parameter));
		}

		Mode = navigationModel.Mode;

		if (Mode == EditorMode.Edit)
		{
			_editedEventCountdown = await _dataService.GetCountdownAsync(navigationModel.Id);
			if (_editedEventCountdown != null)
			{
				LoadEditedCountdown();
			}
			else
			{
				_navigationService.GoBack();
			}

			Title = _localizationService.GetString("EditEvent");
		}
		else
		{
			Title = _localizationService.GetString("AddEvent");
		}

		if (DefaultBackgrounds.Count == 0)
		{
			var defaultBackgrounds = _defaultBackgrounds.GetDefaultBackgrounds();
			foreach (var background in defaultBackgrounds)
			{
				DefaultBackgrounds.Add(background);
			}
		}
	}

	partial void OnSelectedDefaultBackgroundChanged(DefaultBackground? value)
	{
		if (SelectedDefaultBackground is not null)
		{
			BackgroundUri = SelectedDefaultBackground.BackgroundUri;
		}
		else
		{
			BackgroundUri = LastCustomBackgroundUri;
		}
	}

	partial void OnBackgroundUriChanged(Uri? value) => LastCustomBackgroundUri = value;

	partial void OnNameChanged(string value) => OnPropertyChanged(nameof(DefaultCelebrationMessage));

	private void LoadEditedCountdown()
	{
		if (_editedEventCountdown == null) throw new NullReferenceException("Edited Countdown is null");
		Name = _editedEventCountdown.Name;
		Date = _editedEventCountdown.TargetDateTime.Date;
		Time = _editedEventCountdown.TargetDateTime.TimeOfDay;
		CelebrationMessage = _editedEventCountdown.CelebrationMessage;
		BackgroundUri = _editedEventCountdown.BackgroundImageUri;
		LastCustomBackgroundUri = BackgroundUri;
	}


	[RelayCommand]
	private void Cancel() => _navigationService.GoBack();

	[RelayCommand]
	private async Task ChooseYourImageAsync()
	{
		IsWorking = true;
		BackgroundUri = (await _backgroundPickerService.PickBackgroundAsync()) ?? LastCustomBackgroundUri;
		IsWorking = false;
	}

	[RelayCommand]
	private async Task SaveAsync()
	{
		IsWorking = true;
		if (Mode == EditorMode.Add)
		{
			_editedEventCountdown = new EventCountdown() { Id = Guid.NewGuid().ToString() };
		}
		_editedEventCountdown.Name = Name;
		TimeSpan fixedTime = new TimeSpan(Time.Hours, Time.Minutes, 0);
		_editedEventCountdown.TargetDateTime = Date.Date + fixedTime;
		_editedEventCountdown.BackgroundImageUri = BackgroundUri;
		_editedEventCountdown.CelebrationMessage = string.IsNullOrWhiteSpace(CelebrationMessage) ? string.Format(CultureInfo.CurrentCulture, _localizationService.GetString("DefaultCelebration"), Name) : CelebrationMessage;
		if (Mode == EditorMode.Edit)
		{
			await _eventCountdownManager.UpdateCountdownAsync(_editedEventCountdown);
		}
		else
		{
			await _eventCountdownManager.AddCountdownAsync(_editedEventCountdown);
		}
		IsWorking = false;
		_navigationService.GoBack();
	}
}
