using System.Diagnostics.CodeAnalysis;

namespace EventCountdowns.Core.Services.Navigation;

public interface IViewProvider
{
	void RegisterView<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewType, TViewModel>();

	bool TryFindViewForViewModel(Type viewModelType, out Type? viewType);
}
