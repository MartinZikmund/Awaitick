using Awaitick.Core.ViewModels;

namespace Awaitick.Views;

public sealed partial class SettingsView : SettingsViewBase
{
	public SettingsView()
	{
		this.InitializeComponent();
	}
}

public abstract partial class SettingsViewBase : PageBase<SettingsViewModel>
{
}

