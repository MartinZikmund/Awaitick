using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.WinUI.Helpers;
using EventCountdowns.Core.DefaultData;
using EventCountdowns.Core.Models;
using EventCountdowns.Core.Services;
using EventCountdowns.Core.Services.Data;
using EventCountdowns.Core.Services.EventCountdownManager;
using EventCountdowns.Dialogs;
using EventCountdowns.Services.Dialogs;
using EventCountdowns.Services.Localization;
using EventCountdowns.Services.Navigation;
using EventCountdowns.Services.Store;
using EventCountdowns.Services.Theming;
using EventCountdowns.ViewModels;
using Microsoft.UI;
using MZikmund.Services.Dialogs;
using MZikmund.Toolkit.WinUI.Infrastructure;
using MZikmund.Toolkit.WinUI.Services;
using Windows.UI;
using Windows.UI.ViewManagement;

namespace EventCountdowns.Core.ViewModels;

public partial class CountdownEditorViewModel : PageViewModel
{
	private readonly ICountdownsDataService _eventCountdownManager;
	private readonly IImagePickerService _imagePickerService;
	private readonly IDataService _dataService;
	private readonly IDialogService _dialogService;
	private readonly IStoreService _storeService;
	private readonly IStringLocalizer _localizationService;
	private readonly INavigationService _navigationService;
	private readonly IDefaultBackgrounds _defaultBackgrounds;
	private readonly IXamlRootProvider _xamlRootProvider;
	private readonly UISettings _uiSettings = new();
	private EventCountdown? _editedEventCountdown;

	public CountdownEditorViewModel(
		ICountdownsDataService eventCountdownManager,
		IImagePickerService imagePickerService,
		IDataService dataService,
		IDialogService dialogService,
		IStoreService storeService,
		IStringLocalizer localizationService,
		INavigationService navigationService,
		IDefaultBackgrounds defaultBackgrounds,
		IXamlRootProvider xamlRootProvider) :
		base(navigationService)
	{
		_eventCountdownManager = eventCountdownManager ?? throw new ArgumentNullException(nameof(eventCountdownManager));
		_imagePickerService = imagePickerService ?? throw new ArgumentNullException(nameof(imagePickerService));
		_dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
		_dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
		_storeService = storeService ?? throw new ArgumentNullException(nameof(storeService));
		_localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
		_navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
		_defaultBackgrounds = defaultBackgrounds ?? throw new ArgumentNullException(nameof(defaultBackgrounds));
		_xamlRootProvider = xamlRootProvider ?? throw new ArgumentNullException(nameof(xamlRootProvider));
	}

	public ICountdownEditorViewService? View { get; set; }

	[ObservableProperty]
	public partial bool HasProLicense { get; set; }

	[ObservableProperty]
	public partial DefaultBackground? SelectedDefaultBackground { get; set; }

	[ObservableProperty]
	public partial EditorMode Mode { get; private set; } = EditorMode.Add;

	[ObservableProperty]
	public partial Uri? LastCustomBackgroundUri { get; set; }

	[ObservableProperty]
	public partial Uri? BackgroundUri { get; set; } = new Uri("ms-appx:///Assets/SampleBackgrounds/Thumbnails/BlankBackground.png", UriKind.Absolute);

	public ElementTheme[] ThemeOptions { get; } = [ElementTheme.Default, ElementTheme.Light, ElementTheme.Dark];

	[ObservableProperty]
	public partial ElementTheme Theme { get; set; }

	[ObservableProperty]
	public partial Uri? LastBackgroundImageUri { get; set; }

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(IsBackgroundImageSet))]
	public partial Uri? BackgroundImageUri { get; set; }

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(IsBackgroundColorSet))]
	public partial Color BackgroundColor { get; set; }

	[ObservableProperty]
	public partial double BackgroundImageOpacityPercent { get; set; }
	
	[ObservableProperty]
	public partial string Name { get; set; } = "";

	[ObservableProperty]
	public partial DateTimeOffset Date { get; set; } = DateTimeOffset.UtcNow.AddDays(7);

	[ObservableProperty]
	public partial TimeSpan Time { get; set; } = TimeSpan.FromHours(DateTimeOffset.UtcNow.TimeOfDay.Hours);

	[ObservableProperty]
	public partial string CelebrationMessage { get; set; } = "";

	[ObservableProperty]
	public partial CountdownViewModel SampleCountdown { get; set; }

	public double BackgroundImageOpacity => BackgroundImageOpacityPercent / 100;

	public bool IsBackgroundImageSet => BackgroundImageUri is not null;

	public bool IsBackgroundColorSet => BackgroundColor != Colors.Transparent;

	public ObservableCollection<DefaultBackground> DefaultBackgrounds { get; } = new();

	public string DefaultCelebrationMessage => string.Format(CultureInfo.CurrentCulture, _localizationService.GetString("DefaultCelebration"), Name);

	public override async void ViewNavigatedTo(object? parameter)
	{
		if (parameter is not NavigationModel navigationModel)
		{
			throw new ArgumentException("Parameter must be CountdownEditorViewModel.NavigationModel.", nameof(parameter));
		}

		Mode = navigationModel.Mode;
		HasProLicense = await _storeService.HasProAsync();

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
		CelebrationMessage = _editedEventCountdown.CelebrationMessage ?? string.Format(Localizer.Instance.GetString("DefaultCelebration"), Name);
		BackgroundUri = _editedEventCountdown.BackgroundImageUri;
		LastCustomBackgroundUri = BackgroundUri;
	}


	[RelayCommand]
	private void Cancel() => _navigationService.GoBack();

	[RelayCommand]
	private async Task PickBackgroundImageAsync()
	{
		if (!HasProLicense)
		{
			var proOnlyFeatureDialog = new ProOnlyFeatureDialog();
			await _dialogService.ShowAsync(proOnlyFeatureDialog);
			return;
		}

		IsWorking = true;
		try
		{

			if (await _imagePickerService.PickAsync() is { } imageUri)
			{
				BackgroundImageUri = imageUri;
				OnPropertyChanged(nameof(IsBackgroundImageSet));
			}
		}
		finally
		{
			IsWorking = false;
		}
	}

	[RelayCommand]
	private async Task PickBackgroundColor()
	{
		IsWorking = true;

		var pickerDialog = new ColorPickerDialog
		{
			XamlRoot = _xamlRootProvider.XamlRoot,
			SelectedColor = IsBackgroundColorSet ? BackgroundColor : _uiSettings.GetColorValue(UIColorType.Accent),
		};

		if (await pickerDialog.ShowAsync() == ContentDialogResult.Primary)
		{
			BackgroundColor = pickerDialog.SelectedColor;
			OnPropertyChanged(nameof(IsBackgroundColorSet));
		}
		IsWorking = false;
	}

	[RelayCommand]
	private void RemoveBackgroundImage()
	{
		BackgroundImageUri = null;
		OnPropertyChanged(nameof(IsBackgroundImageSet));
	}

	[RelayCommand]
	private void RemoveBackgroundColor()
	{
		BackgroundColor = Colors.Transparent;
		OnPropertyChanged(nameof(IsBackgroundColorSet));
	}

	[RelayCommand]
	private async Task SaveAsync()
	{
		IsWorking = true;
		if (Mode == EditorMode.Add)
		{
			_editedEventCountdown = new EventCountdown() { Id = Guid.NewGuid().ToString() };
		}

		if (_editedEventCountdown is null)
		{
			throw new InvalidOperationException("Edited Countdown should be set.");
		}

		_editedEventCountdown.Name = Name;
		TimeSpan fixedTime = new TimeSpan(Time.Hours, Time.Minutes, 0);
		_editedEventCountdown.TargetDateTime = Date.Date + fixedTime;
		_editedEventCountdown.BackgroundImageUri = BackgroundUri;
		_editedEventCountdown.BackgroundColor = BackgroundColor.ToHex();
		_editedEventCountdown.Theme = Theme;
		_editedEventCountdown.BackgroundImageOpacity = BackgroundImageOpacity;
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
