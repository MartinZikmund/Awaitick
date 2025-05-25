using System.Diagnostics.CodeAnalysis;

namespace Awaitick.Core.Services.Navigation;

public class ViewProvider : IViewProvider
{
	private readonly Dictionary<Type, Type> _viewModelViews = new();

	public bool TryFindViewForViewModel(Type viewModelType, out Type? viewType) => _viewModelViews.TryGetValue(viewModelType, out viewType);

	public void RegisterView<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewType, TViewModel>()
	{
		var viewType = typeof(TViewType);
		var viewModelType = typeof(TViewModel);
		_viewModelViews.Add(viewModelType, viewType);
	}
}
