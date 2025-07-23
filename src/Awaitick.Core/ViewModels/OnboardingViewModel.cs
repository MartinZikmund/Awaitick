using Awaitick.Core.Models.Presets;
using Awaitick.Core.Services.Settings;
using Awaitick.Services.Navigation;

namespace Awaitick.Core.ViewModels;

public partial class OnboardingViewModel : PageViewModel
{
	private readonly IAppPreferences _appPreferences;

	public OnboardingViewModel(INavigationService navigationService, IAppPreferences appPreferences) : base(navigationService)
	{
		_appPreferences = appPreferences;
	}

	public EventPreset[] Presets => EventPresets.Presets;

	[ObservableProperty]
	public partial int Step { get; set; } = 0;

	[RelayCommand]
	public void NextStep() => Step = 1;

	[RelayCommand]
	public void StartApp()
	{
		_appPreferences.FirstStart = false;
		NavigationService.Navigate<MainViewModel>();
	}
}
