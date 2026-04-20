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

	public IEnumerable<EventPreset> Presets => EventPresets.InDisplayOrder();

	public EventPreset[] SelectedPresets { get; set; } = Array.Empty<EventPreset>();

	[ObservableProperty]
	public partial int Step { get; set; } = 0;

	[RelayCommand]
	private void NextStep()
	{
		if (Step < 2)
		{
			Step++;
		}
	}

	[RelayCommand]
	private async Task CreateCustomEventAsync()
	{
		await StartAppAsync();

		NavigationService.Navigate<CountdownEditorViewModel>(CountdownEditorViewModel.NavigationModel.CreateAdd());
	}

	[RelayCommand]
	private async Task StartAppAsync()
	{
		await SavePresetsAsync();

		_appPreferences.FirstStart = false;
		NavigationService.Navigate<MainViewModel>();

		NavigationService.ClearBackStack();
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
