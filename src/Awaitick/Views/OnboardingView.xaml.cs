using Awaitick.Core.ViewModels;

namespace Awaitick.Views;

public sealed partial class OnboardingView : OnboardingViewBase
{
	public OnboardingView()
	{
		this.InitializeComponent();
	}
}

public abstract partial class OnboardingViewBase : PageBase<OnboardingViewModel>
{
}
