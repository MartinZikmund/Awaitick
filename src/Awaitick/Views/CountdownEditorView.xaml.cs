using Awaitick.Core.ViewModels;

namespace Awaitick.Views;

public sealed partial class CountdownEditorView : CountdownEditorViewBase
{
	public CountdownEditorView()
	{
		this.InitializeComponent();
	}
}

public abstract partial class CountdownEditorViewBase : PageBase<CountdownEditorViewModel>
{
}
