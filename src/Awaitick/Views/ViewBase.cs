using System.Diagnostics.CodeAnalysis;
using Awaitick.Core.ViewModels;
using Awaitick.ViewModels;

namespace Awaitick.Views;

public interface IBlendsInTitleBar
{
	bool BlendsInTitleBar { get; }
}

public abstract partial class PageBase<TViewModel> : Page, IBlendsInTitleBar
	where TViewModel : PageViewModel
{
	protected PageBase()
	{
		Loading += OnPageLoading;
		Loaded += OnPageLoaded;
		Unloaded += OnPageUnloaded;
	}

	public virtual bool BlendsInTitleBar => false;

	/// <summary>
	/// Gets the ViewModel for this page. Resolved from DI on first access.
	/// </summary>
	public TViewModel? ViewModel { get; private set; }

	[MemberNotNull(nameof(ViewModel))]
	private void EnsureViewModel()
	{
		SetTitleBarPadding();

		if (ViewModel is not null)
		{
			return;
		}

		if (FindWindowShell(Frame.XamlRoot?.Content) is not WindowShell windowShell)
		{
			throw new InvalidOperationException("View must be hosted inside a WindowShell");
		}

		ViewModel = windowShell.ServiceProvider.GetRequiredService<TViewModel>();
		DataContext = ViewModel;
		ViewModel.ViewCreated();
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

	private WindowShell? FindWindowShell(UIElement? windowRoot)
	{
		if (windowRoot is WindowShell shell)
		{
			return shell;
		}

		// This happens when Hot Design takes over the root.
		if (windowRoot is ContentControl { Content: WindowShell windowShell })
		{
			return windowShell;
		}

		return null;
	}

	private void OnPageLoading(FrameworkElement sender, object args)
	{
		EnsureViewModel();

		ViewModel?.ViewLoading();
	}

	private void OnPageLoaded(object sender, RoutedEventArgs e)
	{
		ViewModel?.ViewLoaded();
	}

	private void OnPageUnloaded(object sender, RoutedEventArgs e)
	{
		ViewModel?.ViewUnloaded();
	}

	protected override void OnNavigatedTo(NavigationEventArgs e)
	{
		base.OnNavigatedTo(e);
		EnsureViewModel();

		ViewModel.ViewNavigatedToInternal(e.Parameter);
	}

	protected override void OnNavigatedFrom(NavigationEventArgs e)
	{
		base.OnNavigatedFrom(e);
		ViewModel?.ViewNavigatedFrom();
	}
}
