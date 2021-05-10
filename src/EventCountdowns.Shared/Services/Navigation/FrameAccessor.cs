#nullable enable

using EventCountdowns.Core.Services.Navigation;
using Windows.UI.Xaml.Controls;

namespace EventCountdowns.Services.Navigation
{
    public class FrameAccessor : IFrameAccessor
    {
        private Frame? _frame = null;

        public Frame GetFrame() => _frame ??= AppShell.GetForCurrentView().RootFrame;
    }
}
