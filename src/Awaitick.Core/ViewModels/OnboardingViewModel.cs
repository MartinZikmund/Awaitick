using Awaitick.Services.Navigation;
using Windows.UI.StartScreen;

namespace Awaitick.Core.ViewModels;

public partial class OnboardingViewModel : PageViewModel
{
	public OnboardingViewModel(INavigationService navigationService) : base(navigationService)
	{
	}

	[ObservableProperty]
	public partial int Step { get; set; } = 0;

	[RelayCommand]
	public void NextStep() => Step = 1;

	[RelayCommand]
	public void StartApp()
	{
		NavigationService.Navigate<MainViewModel>();
	}
}
