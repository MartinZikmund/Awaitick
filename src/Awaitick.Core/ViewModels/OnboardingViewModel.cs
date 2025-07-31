using Awaitick.Core.Models.Presets;
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

	protected override void ViewNavigatedFrom(object? parameter) => NavigationService.ClearBackStack();

	public EventPreset[] Presets => EventPresets.Presets;

	public EventPreset[] SelectedPresets { get; set; }

	[ObservableProperty]
	public partial int Step { get; set; } = 0;

	[RelayCommand]
	public void NextStep() => Step++;

	[RelayCommand]
	private async Task StartAppAsync()
	{
		await SavePresetsAsync();
		_appPreferences.FirstStart = false;

		// Clear the back stack to prevent returning
		NavigationService.Navigate<MainViewModel>();
		NavigationService.ClearBackStack();
	}

	[RelayCommand]
	private async Task CreateCustomEventAsync()
	{
		// Clear the back stack to prevent returning
		// to the onboarding view and navigate to the main view model
		await StartAppAsync();

		// Navigate to the Countdown Editor ViewModel to create a custom event
		NavigationService.Navigate<CountdownEditorViewModel>(CountdownEditorViewModel.NavigationModel.CreateAdd());
	}

	private async Task SavePresetsAsync()
	{
		foreach (var preset in SelectedPresets)
		{
			var eventCountdown = preset.Create();
			await _dataService.AddCountdownAsync(eventCountdown);
		}
	}
}
