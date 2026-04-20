using Awaitick.Core.Models.Presets;
using Awaitick.Core.ViewModels;

namespace Awaitick.Views;

public sealed partial class NewCountdownView : NewCountdownViewBase
{
	public NewCountdownView()
	{
		this.InitializeComponent();
	}

	private void PresetsItemInvoked(ItemsView sender, ItemsViewItemInvokedEventArgs args)
	{
		if (ViewModel is null)
		{
			return;
		}

		if (args.InvokedItem is EventPreset preset)
		{
			ViewModel.SelectPresetCommand.Execute(preset);
		}
	}
}

public abstract partial class NewCountdownViewBase : PageBase<NewCountdownViewModel>
{
}
