using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using EventCountdowns.Core.ViewModels;
using EventCountdowns.Views;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;


/* Unmerged change from project 'EventCountdowns (net8.0)'
Before:
namespace EventCountdowns.Views
{
	public sealed partial class CountdownEditorView : CountdownEditorViewBase
After:
namespace EventCountdowns.Views;

	public sealed partial class CountdownEditorView : CountdownEditorViewBase
*/
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

/* Unmerged change from project 'EventCountdowns (net8.0)'
Removed:
}
*/
}
