using Awaitick.Core.Models.Presets;
using Awaitick.Core.Models.Licensing;
using Awaitick.Core.Services.Data;
using Awaitick.Core.Services.ScheduledNotification;
using Awaitick.Core.Services.Settings;
using Awaitick.Services.Navigation;
using Awaitick.Services.Store;
using Awaitick.Services.Theming;
using CommunityToolkit.WinUI.Helpers;
using Microsoft.Toolkit.Uwp.Helpers;
using MZikmund.Services.Dialogs;
using MZikmund.Toolkit.WinUI.Infrastructure;
using MZikmund.Toolkit.WinUI.Services;
using Windows.System;
using Windows.UI.ViewManagement;

namespace Awaitick.Core.ViewModels;

public partial class SettingsViewModel : PageViewModel
{
	private readonly IAppPreferences _appSettings;
	private readonly IThemeManager _themeManager;
	private readonly IXamlRootProvider _xamlRootProvider;
	private readonly IStoreService _storeService;
	private readonly IDialogService _dialogService;
	private readonly IScheduledNotificationService _scheduledNotificationService;
	private readonly INotificationPermissionService _notificationPermissionService;
	private readonly IDataService _dataService;

	private readonly UISettings _uiSettings = new();

	private bool _isInitializing = false;

	public SettingsViewModel(
		INavigationService navigationService,
		IAppPreferences appSettings,
		IThemeManager themeManager,
		IXamlRootProvider xamlRootProvider,
		IStoreService storeService,
		IDialogService dialogService,
		IScheduledNotificationService scheduledNotificationService,
		INotificationPermissionService notificationPermissionService,
		IDataService dataService,
		IStringLocalizer stringLocalizer) : base(navigationService)
	{
		_appSettings = appSettings;
		_themeManager = themeManager;
		_xamlRootProvider = xamlRootProvider;
		_storeService = storeService;
		_dialogService = dialogService;
		_scheduledNotificationService = scheduledNotificationService;
		_notificationPermissionService = notificationPermissionService;
		_dataService = dataService;

		Title = stringLocalizer.GetString("Settings");
	}

	public override async void ViewNavigatedTo(object? parameter)
	{
		base.ViewNavigatedTo(parameter);
		try
		{
			_isInitializing = true;
			HasProLicense = await _storeService.HasProAsync();
			Theme = _appSettings.Theme;
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

	public bool IsDebug =>
#if DEBUG
		true;
#else
		false;
#endif

	[ObservableProperty]
	public partial bool HasProLicense { get; private set; }

	public ElementTheme[] ThemeOptions { get; } = [ElementTheme.Default, ElementTheme.Light, ElementTheme.Dark];

	[ObservableProperty]
	public partial ElementTheme Theme { get; set; }

	partial void OnThemeChanged(ElementTheme value)
	{
		_themeManager.SetTheme(Theme);
		_appSettings.Theme = value;
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

	public bool NotificationsEnabled
	{
		get => _appSettings.NotificationsEnabled;
		set
		{
			if (_appSettings.NotificationsEnabled != value)
			{
				if (value)
				{
					_ = EnableNotificationsAsync();
				}
				else
				{
					_appSettings.NotificationsEnabled = false;
					OnPropertyChanged();
					_ = DisableNotificationsAsync();
				}
			}
		}
	}

	private async Task EnableNotificationsAsync()
	{
		var granted = await _notificationPermissionService.RequestPermissionWithDialogsAsync();
		if (granted)
		{
			_appSettings.NotificationsEnabled = true;
			OnPropertyChanged(nameof(NotificationsEnabled));
			var countdowns = await _dataService.GetCountdownsAsync();
			var futureCountdowns = countdowns.Where(c => c.TargetDateTime > DateTimeOffset.Now);
			await _scheduledNotificationService.RescheduleAllNotificationsAsync(futureCountdowns);
		}
		else
		{
			// Revert toggle UI
			OnPropertyChanged(nameof(NotificationsEnabled));
		}
	}

	private async Task DisableNotificationsAsync()
	{
		var countdowns = await _dataService.GetCountdownsAsync();
		var futureCountdowns = countdowns.Where(c => c.TargetDateTime > DateTimeOffset.Now);
		foreach (var countdown in futureCountdowns)
		{
			_scheduledNotificationService.UnscheduleCountdownNotification(countdown);
		}
	}

	public string PackageVersionString => Package.Current.Id.Version.ToFormattedString();

	public string CopyrightLine => AppLicenseInfo.Copyright;

	[RelayCommand]
	private async Task ReviewAppAsync() => await SystemInformation.LaunchStoreForReviewAsync();

	[RelayCommand]
	private void OpenLicenses() => NavigationService.Navigate<LicensesViewModel>();

	[RelayCommand]
	private async Task OpenSourceCodeAsync() => await Launcher.LaunchUriAsync(new Uri(AppLicenseInfo.RepositoryUrl));

	[RelayCommand]
	private void GoToOnboarding() => NavigationService.Navigate<OnboardingViewModel>();

	[RelayCommand]
	private void ClearPreferences()
	{
		ApplicationData.Current.LocalSettings.Values.Clear();
	}

	[RelayCommand]
	private async Task SeedRandomEventsAsync()
	{
		var random = new Random();
		var presets = EventPresets.Presets.OrderBy(_ => random.Next()).Take(5).ToArray();
		var events = presets.Select(p =>
		{
			var e = p.Create();
			e.Id = Guid.NewGuid().ToString().ToLowerInvariant();
			return e;
		}).ToArray();
		await _dataService.AddCountdownsAsync(events);
	}

	[RelayCommand]
	private async Task AddAllPresetEventsAsync()
	{
		var events = EventPresets.Presets.Select(p =>
		{
			var e = p.Create();
			e.Id = Guid.NewGuid().ToString().ToLowerInvariant();
			return e;
		}).ToArray();
		await _dataService.AddCountdownsAsync(events);
	}

	[RelayCommand]
	private async Task DeleteAllEventsAsync()
	{
		await _dataService.DeleteAllCountdownsAsync();
	}

	private void SaveChanges()
	{
		if (_isInitializing)
		{
			return;
		}
	}
}
