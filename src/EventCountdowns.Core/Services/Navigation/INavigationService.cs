namespace EventCountdowns.Core.Services.Navigation;

public interface INavigationService
{
	void Navigate<TViewModel>();

	void Navigate<TViewModel>(object navigationModel);

	void GoBack();

	bool CanGoBack { get; }

	INavigationService RegisterForNavigation<TViewModel, TPage>()
		where TPage : Page;
}
