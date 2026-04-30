using System.Reflection;

namespace Awaitick.Services.Navigation;

public interface INavigationService
{
	void ClearBackStack();

	void RemoveLastFromBackStack();

	void Navigate<TViewModel>();

	void Navigate<TViewModel>(object? parameter);

	bool GoBack();

	bool CanGoBack { get; }

	void Initialize();

	void RegisterViewsFromAssembly(Assembly sourceAssembly);
}
