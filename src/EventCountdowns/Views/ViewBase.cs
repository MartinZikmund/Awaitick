using EventCountdowns.Core.Infrastructure;
using EventCountdowns.Core.ViewModels;

namespace EventCountdowns.Views;

public partial class ViewBase<TViewModel> : Page, IViewBase
	where TViewModel : ViewModel
{
	private TViewModel? _model;

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
