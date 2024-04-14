using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Input;
using EventCountdowns.Core.DefaultData;
using EventCountdowns.Core.Models;
using EventCountdowns.Core.Services;
using EventCountdowns.Core.Services.BackgroundPicker;
using EventCountdowns.Core.Services.Data;
using EventCountdowns.Core.Services.EventCountdownManager;

namespace EventCountdowns.Core.ViewModels;

public class CountdownEditorViewModel : ViewModel
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
	private readonly ILocalizationService _localizationService;
	private readonly IDefaultBackgrounds _defaultBackgrounds;

	private DefaultBackground? _selectedDefaultBackground;
	private string? _lastCustomBackgroundPath;
	private string? _backgroundPath;
	private EventCountdown? _editedEventCountdown;
	private EditorMode _mode = EditorMode.Add;
	private string _name = "";
	private DateTimeOffset _date = DateTimeOffset.UtcNow.AddDays(1);
	private TimeSpan _time = TimeSpan.Zero;
	private string _celebrationMessage = "";

	public CountdownEditorViewModel(
		IEventCountdownManager eventCountdownManager,
		IBackgroundPickerService backgroundPickerService,
		IDataService dataService,
		ILocalizationService localizationService,
		IDefaultBackgrounds defaultBackgrounds)
	{
		_eventCountdownManager = eventCountdownManager ?? throw new ArgumentNullException(nameof(eventCountdownManager));
		_backgroundPickerService = backgroundPickerService ?? throw new ArgumentNullException(nameof(backgroundPickerService));
		_dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
		_localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
		_defaultBackgrounds = defaultBackgrounds ?? throw new ArgumentNullException(nameof(defaultBackgrounds));
	}

	public override async Task LoadAsync(object? parameter)
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
				Navigation.GoBack();
			}

			Title = _localizationService.EditEvent;
		}
		else
		{
			Title = _localizationService.AddEvent;
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

	public DefaultBackground? SelectedDefaultBackground
	{
		get => _selectedDefaultBackground;
		set
		{
			SetProperty(ref _selectedDefaultBackground, value);
			if (_selectedDefaultBackground != null)
			{
				BackgroundPath = _selectedDefaultBackground.BackgroundPath;
			}
			else
			{
				BackgroundPath = LastCustomBackgroundPath;
			}
		}
	}

	public string? LastCustomBackgroundPath
	{
		get => _lastCustomBackgroundPath;
		set => SetProperty(ref _lastCustomBackgroundPath, value);
	}

	public string? BackgroundPath
	{
		get => _backgroundPath;
		set
		{
			SetProperty(ref _backgroundPath, value);
			LastCustomBackgroundPath = value;
		}
	}

	private void LoadEditedCountdown()
	{
		if (_editedEventCountdown == null) throw new NullReferenceException("Edited Countdown is null");
		Name = _editedEventCountdown.Name;
		Date = _editedEventCountdown.TargetDateTime.Date;
		Time = _editedEventCountdown.TargetDateTime.TimeOfDay;
		CelebrationMessage = _editedEventCountdown.CelebrationMessage;
		BackgroundPath = _editedEventCountdown.BackgroundImagePath;
		LastCustomBackgroundPath = BackgroundPath;
	}

	public ObservableCollection<DefaultBackground> DefaultBackgrounds { get; } = new ObservableCollection<DefaultBackground>();

	public EditorMode Mode
	{
		get => _mode;
		set => SetProperty(ref _mode, value);
	}

	public string Name
	{
		get => _name;
		set
		{
			SetProperty(ref _name, value);
			OnPropertyChanged(nameof(DefaultCelebrationMessage));
		}
	}

	public DateTimeOffset Date
	{
		get => _date;
		set => SetProperty(ref _date, value);
	}

	public TimeSpan Time
	{
		get => _time;
		set => SetProperty(ref _time, value);
	}

	public string CelebrationMessage
	{
		get => _celebrationMessage;
		set => SetProperty(ref _celebrationMessage, value);
	}

	public string DefaultCelebrationMessage => string.Format(CultureInfo.CurrentCulture, _localizationService.DefaultCelebration, Name);

	public ICommand CancelCommand => GetOrCreateCommand(Cancel);

	private void Cancel() => Navigation.GoBack();

	public ICommand ChooseYourImageCommand => GetOrCreateCommand(ChooseYourImage);

	private async void ChooseYourImage()
	{
		IsWorking = true;
		BackgroundPath = (await _backgroundPickerService.PickBackgroundAsync()) ?? LastCustomBackgroundPath;
		IsWorking = false;
	}

	public ICommand SaveCommand => GetOrCreateCommand(Save);

	private async void Save()
	{
		IsWorking = true;
		if (Mode == EditorMode.Add)
		{
			_editedEventCountdown = new EventCountdown() { Id = Guid.NewGuid().ToString() };
		}
		_editedEventCountdown.Name = Name;
		TimeSpan fixedTime = new TimeSpan(Time.Hours, Time.Minutes, 0);
		_editedEventCountdown.TargetDateTime = Date.Date + fixedTime;
		_editedEventCountdown.BackgroundImagePath = BackgroundPath;
		_editedEventCountdown.CelebrationMessage = string.IsNullOrWhiteSpace(CelebrationMessage) ? string.Format(CultureInfo.CurrentCulture, _localizationService.DefaultCelebration, Name) : CelebrationMessage;
		if (Mode == EditorMode.Edit)
		{
			await _eventCountdownManager.UpdateCountdownAsync(_editedEventCountdown);
		}
		else
		{
			await _eventCountdownManager.AddCountdownAsync(_editedEventCountdown);
		}
		IsWorking = false;
		Navigation.GoBack();
	}
}
