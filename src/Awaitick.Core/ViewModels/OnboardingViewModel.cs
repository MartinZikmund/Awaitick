using Awaitick.Core.Services.Settings;
using Awaitick.Services.Navigation;
using Windows.UI.StartScreen;

namespace Awaitick.Core.ViewModels;

public partial class OnboardingViewModel : PageViewModel
{
	private readonly IAppPreferences _appPreferences;

	public OnboardingViewModel(INavigationService navigationService, IAppPreferences appPreferences) : base(navigationService)
	{
		_appPreferences = appPreferences;
	}

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
