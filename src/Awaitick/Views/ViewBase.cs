using System.Reflection;
using EventCountdowns.Core.ViewModels;
using EventCountdowns.ViewModels;

namespace EventCountdowns.Views;

public abstract partial class PageBase<TViewModel> : Page
	where TViewModel : PageViewModel
{
	private object? _pendingParameter;
	private bool _isNavigationDelayed;

	protected PageBase()
	{
		Loading += PageLoading;
		Loaded += PageLoaded;
		Unloaded += PageUnloaded;
	}

	public virtual bool BlendsInTitleBar => false;

	public virtual TViewModel? ViewModel { get; private set; }

	private void PageLoading(object sender, object args)
	{
		SetTitleBarPadding();

		EnsureViewModel();

		if (_isNavigationDelayed)
		{
			ViewModel?.ViewNavigatedToInternal(_pendingParameter);
			_pendingParameter = null;
		}

		ViewModel?.ViewLoading();
	}

	private void SetTitleBarPadding()
	{
		if (BlendsInTitleBar)
		{
			return;
		}

		var titleBarHeight = (double)Application.Current.Resources["TitleBarHeight"];
		if (Content is Grid grid)
		{
			grid.Padding = new Thickness(grid.Padding.Left, titleBarHeight, grid.Padding.Right, grid.Padding.Bottom);
		}
		else if (Content is Border border)
		{
			border.Padding = new Thickness(border.Padding.Left, titleBarHeight, border.Padding.Right, border.Padding.Bottom);
		}
	}

	private void PageLoaded(object sender, RoutedEventArgs e) => ViewModel?.ViewLoaded();

	protected override void OnNavigatedTo(NavigationEventArgs e)
	{
		EnsureViewModel();

		if (ViewModel is not null)
		{
			ViewModel.ViewNavigatedToInternal(_pendingParameter ?? e.Parameter);
		}
		else
		{
			_isNavigationDelayed = true;
			_pendingParameter = e.Parameter;
		}
	}

	private void PageUnloaded(object sender, RoutedEventArgs e) => ViewModel?.ViewUnloaded();

	private void EnsureViewModel()
	{
		if (ViewModel is not null)
		{
			return;
		}

		if (XamlRoot?.Content is WindowShell windowShell)
		{
			ViewModel = windowShell.ServiceProvider.GetRequiredService<TViewModel>();
			DataContext = ViewModel;
		}
	}
}
