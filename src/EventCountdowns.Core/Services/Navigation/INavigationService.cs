using System.Reflection;

namespace EventCountdowns.Services.Navigation;

public interface INavigationService
{
	void ClearBackStack();

	void Navigate<TViewModel>();

	void Navigate<TViewModel>(object? parameter);

	bool GoBack();

	bool CanGoBack { get; }

	event EventHandler? Navigated;

	void Initialize();

	void RegisterViewsFromAssembly(Assembly sourceAssembly);
}
