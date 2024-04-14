using EventCountdowns.Core.ViewModels;

namespace EventCountdowns.Views;

public sealed partial class CountdownEditorView : CountdownEditorViewBase
{
	public CountdownEditorView()
	{
		this.InitializeComponent();
	}
}

public partial class CountdownEditorViewBase : ViewBase<CountdownEditorViewModel>
{
}
