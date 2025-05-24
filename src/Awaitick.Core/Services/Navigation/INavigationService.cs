using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Awaitick.Services.Navigation;

public interface INavigationService
{
	void ClearBackStack();

	void Navigate<TViewModel>();

	void Navigate<TViewModel>(object? parameter);

	bool GoBack();

	bool CanGoBack { get; }

	void Initialize();
}
