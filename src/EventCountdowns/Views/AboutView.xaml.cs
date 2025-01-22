using EventCountdowns.Core.ViewModels;

namespace EventCountdowns.Views;

public sealed partial class AboutView : AboutViewBase
{
	public AboutView()
	{
		InitializeComponent();
	}
}

public abstract partial class AboutViewBase : PageBase<AboutViewModel>
{
}
