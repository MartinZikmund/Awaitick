using EventCountdowns.Core.Infrastructure;

namespace EventCountdowns.Services.Navigation;

public interface IWindowShellProvider : IWindowShell
{
	XamlRoot XamlRoot { get; }

	Window Window { get; }
}
