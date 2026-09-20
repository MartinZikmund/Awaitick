using Awaitick.Core.ViewModels;

namespace Awaitick.Views;

public sealed partial class LicensesView : LicensesViewBase
{
	public LicensesView()
	{
		this.InitializeComponent();
	}
}

public abstract partial class LicensesViewBase : PageBase<LicensesViewModel>
{
}
