using CommunityToolkit.WinUI.Helpers;
using EventCountdowns.Core.Services.Settings;
using EventCountdowns.Services.Navigation;
using EventCountdowns.Services.Theming;
using Javax.Sql;
using Microsoft.UI;
using MZikmund.Toolkit.WinUI.Infrastructure;
using Windows.UI;
using Windows.UI.ViewManagement;

namespace EventCountdowns.ViewModels;

public partial class SettingsViewModel : PageViewModel
{
	private readonly IAppPreferences _appSettings;
	private readonly IThemeManager _themeManager;
	private readonly IXamlRootProvider _xamlRootProvider;
	private readonly IStoreService _storeService;
	private readonly IDialogService _dialogService;
	private readonly IDataSource _dataSource;

	private readonly UISettings _uiSettings = new();

	[ObservableProperty]
	private ElementTheme _theme;

	[ObservableProperty]
	private bool _hasProLicense;

	private bool _isInitializing = false;

	public SettingsViewModel(
		INavigationService navigationService,
		IAppPreferences appSettings,
		IThemeManager themeManager,
		IXamlRootProvider xamlRootProvider,
		IStoreService storeService,
		IDialogService dialogService,
		IDataSource dataSource) : base(navigationService)
	{
		_appSettings = appSettings;
		_themeManager = themeManager;
		_xamlRootProvider = xamlRootProvider;
		_storeService = storeService;
		_dialogService = dialogService;
		_dataSource = dataSource;
	}

	public override async void ViewNavigatedTo(object? parameter)
	{
		base.ViewNavigatedTo(parameter);
		try
		{
			_isInitializing = true;
			HasProLicense = await _storeService.HasProAsync();

			if (parameter is int EventCountdownsId)
			{
				if (_dataSource.EventCountdownses.Get(EventCountdownsId) is not { } EventCountdowns)
				{
					throw new InvalidOperationException("EventCountdowns with ID " + EventCountdownsId + " does not exist.");
				}

				_EventCountdowns = EventCountdowns;
				Theme = _EventCountdowns.Theme;

				BackgroundImageUri = _EventCountdowns.BackgroundImageUri is not null ? new(_EventCountdowns.BackgroundImageUri) : null;
				BackgroundImageOpacityPercent = _EventCountdowns.BackgroundImageOpacity * 100;
				BackgroundColor = ColorHelper.ToColor(_EventCountdowns.BackgroundColor);
			}
		}
		finally
		{
			_isInitializing = false;
		}
	}

	public override void GoBack()
	{
		SaveChanges();

		base.GoBack();
	}

	public ElementTheme[] ThemeOptions { get; } = [ElementTheme.Default, ElementTheme.Light, ElementTheme.Dark];

	partial void OnThemeChanged(ElementTheme value)
	{
		_themeManager.SetTheme(Theme);
		SaveChanges();
	}

	public bool KeepScreenOn
	{
		get => _appSettings.KeepScreenOn;
		set
		{
			if (_appSettings.KeepScreenOn != value)
			{
				_appSettings.KeepScreenOn = value;
				OnPropertyChanged();
			}
		}
	}

	partial void OnBackgroundImageOpacityPercentChanged(double value) => SaveChanges();

	public double BackgroundImageOpacity => BackgroundImageOpacityPercent / 100;

	public bool IsBackgroundImageSet => BackgroundImageUri is not null;

	public bool IsBackgroundColorSet => BackgroundColor != Colors.Transparent;

	public string PackageVersionString => Package.Current.Id.Version.ToFormattedString();

	[RelayCommand]
	private async Task ReviewAppAsync() => await SystemInformation.LaunchStoreForReviewAsync();

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

			SaveChanges();
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
			SaveChanges();
		}
		IsWorking = false;
	}

	[RelayCommand]
	private void RemoveBackgroundImage()
	{
		BackgroundImageUri = null;
		OnPropertyChanged(nameof(IsBackgroundImageSet));
		SaveChanges();
	}

	[RelayCommand]
	private void RemoveBackgroundColor()
	{
		BackgroundColor = Colors.Transparent;
		OnPropertyChanged(nameof(IsBackgroundColorSet));
		SaveChanges();
	}

	private void SaveChanges()
	{
		if (_isInitializing)
		{
			return;
		}

		_EventCountdowns.Theme = Theme;
		_EventCountdowns.BackgroundImageUri = BackgroundImageUri?.ToString();
		_EventCountdowns.BackgroundImageOpacity = BackgroundImageOpacityPercent / 100;
		_EventCountdowns.BackgroundColor = ColorHelper.ToHex(BackgroundColor);
		_dataSource.EventCountdownses.Update(_EventCountdowns);
	}
}
