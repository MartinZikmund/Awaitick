using System.Reflection;

namespace EventCountdowns.Services.Navigation;

public interface INavigationService
{
	void ClearBackStack();

	void Navigate<TViewModel>();

	void Navigate<TViewModel>(object parameter);

	bool GoBack();

	void Initialize();

	void RegisterViewsFromAssembly(Assembly sourceAssembly);
}
