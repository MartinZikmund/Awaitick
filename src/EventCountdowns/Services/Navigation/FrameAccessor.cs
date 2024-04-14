#nullable enable

using EventCountdowns.Core.Services.Navigation;
using Microsoft.UI.Xaml.Controls;

namespace EventCountdowns.Services.Navigation;

public class FrameAccessor : IFrameAccessor
{
    private Frame? _frame;

	public Frame GetFrame() => _frame ??= AppShell.GetForCurrentView().RootFrame;
}
