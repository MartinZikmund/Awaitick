using Awaitick.Core.Models.Presets;
using Awaitick.Core.ViewModels;

namespace Awaitick.Views;

public sealed partial class OnboardingView : OnboardingViewBase
{
	public OnboardingView()
	{
		this.InitializeComponent();
	}

	private void PresetsSelectionChanged(ItemsView sender, ItemsViewSelectionChangedEventArgs args)
	{
		if (ViewModel is null)
		{
			return;
		}

		var presets = sender.SelectedItems.OfType<EventPreset>().ToArray();
		ViewModel.SelectedPresets = presets;
	}
}

public abstract partial class OnboardingViewBase : PageBase<OnboardingViewModel>
{
}
