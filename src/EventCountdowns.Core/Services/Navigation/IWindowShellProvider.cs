using EventCountdowns.App.Core.Infrastructure;

namespace EventCountdowns.Services.Navigation;

public interface IWindowShellProvider : IWindowShell
{
	Window Window { get; }
}
