#nullable enable

using EventCountdowns.Core.Services.Navigation;
using Microsoft.UI.Xaml.Controls;


/* Unmerged change from project 'EventCountdowns (net8.0)'
Before:
namespace EventCountdowns.Services.Navigation
{
	public class FrameAccessor : IFrameAccessor
After:
namespace EventCountdowns.Services.Navigation;

	public class FrameAccessor : IFrameAccessor
*/
namespace EventCountdowns.Services.Navigation;

public class FrameAccessor : IFrameAccessor
{
	private Frame? _frame;

    public Frame GetFrame() => _frame ??= AppShell.GetForCurrentView().RootFrame;

/* Unmerged change from project 'EventCountdowns (net8.0)'
Removed:
}
*/
}
