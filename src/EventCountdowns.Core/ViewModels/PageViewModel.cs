
using EventCountdowns.Core.ViewModels;

namespace EventCountdowns.ViewModels;

public abstract partial class PageViewModel : ViewModelBase
{
	[ObservableProperty]
	private string _title = "";

	public virtual void ViewCreated() { }

	public virtual void ViewLoading() { }

	public virtual void ViewLoaded() { }

	public virtual void ViewUnloaded() { }

	public virtual void ViewNavigatedTo(object? parameter) { }
}
