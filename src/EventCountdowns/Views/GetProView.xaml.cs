using EventCountdowns.ViewModels;

namespace EventCountdowns.Views;

public sealed partial class GetProView : GetProViewBase
{
	public GetProView()
	{
		this.InitializeComponent();
	}
}

public partial class GetProViewBase : PageBase<GetProViewModel>
{
}
