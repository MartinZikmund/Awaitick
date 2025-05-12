using EventCountdowns.Core.Infrastructure;

namespace EventCountdowns.Services.Navigation;

public interface IWindowShellProvider : IWindowShell
{
	IWindowShell Shell { get; }

	Window Window { get; }
}
