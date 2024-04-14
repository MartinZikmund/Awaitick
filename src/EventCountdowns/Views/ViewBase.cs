using EventCountdowns.Core.Infrastructure;
using EventCountdowns.Core.ViewModels;
using Windows.ApplicationModel;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;


/* Unmerged change from project 'EventCountdowns (net8.0)'
Before:
namespace EventCountdowns.Views
{
	public partial class ViewBase<TViewModel> : Page, IViewBase
After:
namespace EventCountdowns.Views;

	public partial class ViewBase<TViewModel> : Page, IViewBase
*/
namespace EventCountdowns.Views;

public partial class ViewBase<TViewModel> : Page, IViewBase
	where TViewModel : ViewModel
{
	private TViewModel? _model = null;

	public ViewBase()
	{
		//TODO: Move to later?
		DataContext = Model;
	}

	public virtual TViewModel Model
	{
		get
		{
			if (DesignMode.DesignMode2Enabled)
			{
				return null;
			}

			return _model ??= IoC.GetRequiredService<TViewModel>();
		}
	}

	object IViewBase.Model => Model;

	protected override async void OnNavigatedTo(NavigationEventArgs e)
	{
		base.OnNavigatedTo(e);
		await Model.LoadAsync(e.Parameter);
	}
}
