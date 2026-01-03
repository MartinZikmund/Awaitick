using Awaitick.Core.Services.Settings;
using Awaitick.Services.Navigation;
using Awaitick.Services.Store;
using Awaitick.Services.Theming;
using CommunityToolkit.WinUI.Helpers;
using Microsoft.Toolkit.Uwp.Helpers;
using MZikmund.Services.Dialogs;
using MZikmund.Toolkit.WinUI.Infrastructure;
using MZikmund.Toolkit.WinUI.Services;
using Windows.UI.ViewManagement;

namespace Awaitick.Core.ViewModels;

public partial class SettingsViewModel : PageViewModel
{
	private readonly IAppPreferences _appSettings;
	private readonly IThemeManager _themeManager;
	private readonly IXamlRootProvider _xamlRootProvider;
	private readonly IStoreService _storeService;
	private readonly IDialogService _dialogService;

	private readonly UISettings _uiSettings = new();

	private bool _isInitializing = false;

	public SettingsViewModel(
		INavigationService navigationService,
		IAppPreferences appSettings,
		IThemeManager themeManager,
		IXamlRootProvider xamlRootProvider,
		IStoreService storeService,
		IDialogService dialogService,
		IStringLocalizer stringLocalizer) : base(navigationService)
	{
		_appSettings = appSettings;
		_themeManager = themeManager;
		_xamlRootProvider = xamlRootProvider;
		_storeService = storeService;
		_dialogService = dialogService;

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
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"[SettingsViewModel] Error in ViewNavigatedTo: {ex}");
			// Use defaults on error
			HasProLicense = false;
			Theme = ElementTheme.Default;
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

	public string PackageVersionString => Package.Current.Id.Version.ToFormattedString();

	[RelayCommand]
	private async Task ReviewAppAsync() => await SystemInformation.LaunchStoreForReviewAsync();

	[RelayCommand]
	private void GoToOnboarding() => NavigationService.Navigate<OnboardingViewModel>();

	[RelayCommand]
	private void ClearPreferences()
	{
		ApplicationData.Current.LocalSettings.Values.Clear();
	}

	private void SaveChanges()
	{
		if (_isInitializing)
		{
			return;
		}
	}
}
