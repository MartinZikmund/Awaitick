using Awaitick.Core.Models.Presets;
using Awaitick.Core.Services.Data;
using Awaitick.Core.Services.EventCountdownManager;
using Awaitick.Core.Services.Settings;
using Awaitick.Services.Navigation;

namespace Awaitick.Core.ViewModels;

public partial class OnboardingViewModel : PageViewModel
{
	private readonly IAppPreferences _appPreferences;
	private readonly ICountdownsDataService _dataService;

	public OnboardingViewModel(INavigationService navigationService, IAppPreferences appPreferences, ICountdownsDataService dataService) : base(navigationService)
	{
		_appPreferences = appPreferences;
		_dataService = dataService;
	}

	public override void ViewLoaded()
	{
		_appPreferences.FirstStart = false;
	}

	public EventPreset[] Presets => EventPresets.Presets;

	public EventPreset[] SelectedPresets { get; set; }

	[ObservableProperty]
	public partial int Step { get; set; } = 0;

	[RelayCommand]
	public void NextStep() => Step = 1;

	[RelayCommand]
	public void SkipPresets() => StartApp();

	[RelayCommand]
	public async Task SavePresets()
	{
		foreach(var preset in SelectedPresets)
		{
			var eventCountdown = preset.Create();
			await _dataService.AddCountdownAsync(eventCountdown);
		}

		StartApp();
	}

	private void StartApp()
	{
		_appPreferences.FirstStart = false;
		NavigationService.Navigate<MainViewModel>();
	}
}
