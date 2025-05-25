using Awaitick.Core.Infrastructure;

namespace Awaitick.Services.Navigation;

public interface IWindowShellProvider : IWindowShell
{
	IWindowShell Shell { get; }

	Window Window { get; }
}
