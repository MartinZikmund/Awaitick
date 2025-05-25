using System.Diagnostics.CodeAnalysis;

namespace Awaitick.Core.Services.Navigation;

public interface IViewProvider
{
	void RegisterView<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewType, TViewModel>();

	bool TryFindViewForViewModel(Type viewModelType, out Type? viewType);
}
