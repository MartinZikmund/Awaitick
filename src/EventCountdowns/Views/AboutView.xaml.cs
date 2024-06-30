using EventCountdowns.Core.ViewModels;

namespace EventCountdowns.Views;

public sealed partial class AboutView : AboutViewBase
{
	public AboutView()
	{
		InitializeComponent();
	}
}

public partial class AboutViewBase : PageBase<AboutViewModel>
{
}
