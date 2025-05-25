using Awaitick.ViewModels;
using Microsoft.UI.Dispatching;

namespace Awaitick.Core.Infrastructure;

public interface IWindowShell
{
	WindowShellViewModel ViewModel { get; }

	XamlRoot? XamlRoot { get; }

	IServiceProvider ServiceProvider { get; }

	DispatcherQueue DispatcherQueue { get; }

	Frame RootFrame { get; }
}
