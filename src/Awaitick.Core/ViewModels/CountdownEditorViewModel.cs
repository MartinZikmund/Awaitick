using System.Collections.ObjectModel;
using System.Globalization;
using Awaitick.Core.DefaultData;
using Awaitick.Core.Models;
using Awaitick.Core.Models.Presets;
using Awaitick.Core.Services;
using Awaitick.Core.Services.Data;
using Awaitick.Core.Services.EventCountdownManager;
using Awaitick.Core.Services.ScheduledNotification;
using Awaitick.Core.Services.Settings;
using Awaitick.Dialogs;
using Awaitick.Services.Dialogs;
using Awaitick.Services.Localization;
using Awaitick.Services.Navigation;
using Awaitick.Services.Store;
using CommunityToolkit.WinUI.Helpers;
using Microsoft.UI;
using MZikmund.Services.Dialogs;
using MZikmund.Toolkit.WinUI.Infrastructure;
using Windows.UI;
using Windows.UI.ViewManagement;

namespace Awaitick.Core.ViewModels;

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
	private readonly INotificationPermissionService _notificationPermissionService;
	private readonly IAppPreferences _appPreferences;
	private readonly IScheduledNotificationService _scheduledNotificationService;
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
		IXamlRootProvider xamlRootProvider,
		INotificationPermissionService notificationPermissionService,
		IAppPreferences appPreferences,
		IScheduledNotificationService scheduledNotificationService) :
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
		_notificationPermissionService = notificationPermissionService ?? throw new ArgumentNullException(nameof(notificationPermissionService));
		_appPreferences = appPreferences ?? throw new ArgumentNullException(nameof(appPreferences));
		_scheduledNotificationService = scheduledNotificationService ?? throw new ArgumentNullException(nameof(scheduledNotificationService));

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

	public TextTheme[] TextThemeOptions { get; } = [TextTheme.Default, TextTheme.Light, TextTheme.Dark];

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(SampleCountdown))]
	public partial TextTheme TextTheme { get; set; }

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(SampleCountdown))]
	public partial Uri? LastBackgroundImageUri { get; set; }

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(IsBackgroundImageSet))]
	[NotifyPropertyChangedFor(nameof(SampleCountdown))]
	public partial Uri? BackgroundImageUri { get; set; }

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(IsBackgroundColorSet))]
	[NotifyPropertyChangedFor(nameof(SampleCountdown))]
	public partial Color BackgroundColor { get; set; }

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(SampleCountdown))]
	public partial double BackgroundImageOpacityPercent { get; set; }

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(SampleCountdown))]
	[NotifyPropertyChangedFor(nameof(DefaultCelebrationMessage))]
	public partial string Name { get; set; } = "";

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(SampleCountdown))]
	public partial DateTimeOffset Date { get; set; } = DateTimeOffset.UtcNow.AddDays(7);

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(SampleCountdown))]
	public partial TimeSpan Time { get; set; } = TimeSpan.FromHours(DateTimeOffset.UtcNow.TimeOfDay.Hours);

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(SampleCountdown))]
	public partial string CelebrationMessage { get; set; } = "";

	public CountdownViewModel SampleCountdown
	{
		get
		{
			var sampleCountdown = new EventCountdown();
			SetCountdownProperties(sampleCountdown);
			return new(sampleCountdown, null);
		}
	}

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

		var backgrounds = await _defaultBackgrounds.GetDefaultBackgroundsAsync();
		foreach (var background in backgrounds)
		{
			DefaultBackgrounds.Add(background);
		}

		if (Mode == EditorMode.Edit)
		{
			Title = _localizationService.GetString("EditEvent");

			_editedEventCountdown = await _dataService.GetCountdownAsync(navigationModel.Id);
			if (_editedEventCountdown is null)
			{
				_navigationService.GoBack();
			}
		}
		else
		{
			Title = _localizationService.GetString("AddEvent");
			if (!string.IsNullOrEmpty(navigationModel.PresetKey)
				&& EventPresets.GetByKey(navigationModel.PresetKey) is { } preset)
			{
				_editedEventCountdown = preset.Create();
				_editedEventCountdown.Id = Guid.NewGuid().ToString();
			}
			else
			{
				_editedEventCountdown = new EventCountdown() { Id = Guid.NewGuid().ToString() };
			}
		}

		await LoadEditedCountdownAsync();
	}

	partial void OnSelectedDefaultBackgroundChanged(DefaultBackground? value)
	{
		if (SelectedDefaultBackground is not null)
		{
			BackgroundImageUri = SelectedDefaultBackground.BackgroundUri;
		}
		else
		{
			BackgroundImageUri = LastCustomBackgroundUri;
		}
	}

	partial void OnBackgroundImageUriChanged(Uri? value) => LastCustomBackgroundUri = value;

	private async Task LoadEditedCountdownAsync()
	{
		if (_editedEventCountdown == null) throw new NullReferenceException("Edited Countdown is null");
		Name = _editedEventCountdown.Name;
		Date = _editedEventCountdown.TargetDateTime.Date;
		Time = _editedEventCountdown.TargetDateTime.TimeOfDay;
		CelebrationMessage = _editedEventCountdown.CelebrationMessage;
		BackgroundImageUri = _editedEventCountdown.BackgroundImageUri;
		BackgroundColor = ColorHelper.ToColor(_editedEventCountdown.BackgroundColor);
		TextTheme = _editedEventCountdown.TextTheme;
		BackgroundImageOpacityPercent = _editedEventCountdown.BackgroundImageOpacity * 100;
		var backgrounds = await _defaultBackgrounds.GetDefaultBackgroundsAsync();
		SelectedDefaultBackground = backgrounds.FirstOrDefault(x => x.BackgroundUri == BackgroundImageUri);
		LastCustomBackgroundUri = BackgroundImageUri;
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

		if (_editedEventCountdown is null)
		{
			throw new InvalidOperationException("Edited Countdown should be set.");
		}

		SetCountdownProperties(_editedEventCountdown);

		// Request notification permission if notifications are enabled but not yet granted
		if (_appPreferences.NotificationsEnabled && !_scheduledNotificationService.HasPermission)
		{
			await _notificationPermissionService.RequestPermissionWithDialogsAsync();
		}

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

	private void SetCountdownProperties(EventCountdown eventCountdown)
	{
		eventCountdown.Name = Name;
		TimeSpan fixedTime = new TimeSpan(Time.Hours, Time.Minutes, 0);
		eventCountdown.TargetDateTime = Date.Date + fixedTime;
		eventCountdown.BackgroundImageUri = BackgroundImageUri;
		eventCountdown.BackgroundColor = BackgroundColor.ToHex();
		eventCountdown.TextTheme = TextTheme;
		eventCountdown.BackgroundImageOpacity = BackgroundImageOpacity;
		eventCountdown.CelebrationMessage = string.IsNullOrWhiteSpace(CelebrationMessage) ? string.Format(CultureInfo.CurrentCulture, _localizationService.GetString("DefaultCelebration"), Name) : CelebrationMessage;
	}
}
