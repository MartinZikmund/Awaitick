namespace Awaitick.Core.Services.Display;

public sealed class KeepScreenOnService(IDisplayRequestManager displayRequestManager) : IKeepScreenOnService
{
	private IDisposable? _activeRequest;

	public void SetEnabled(bool isEnabled)
	{
		if (isEnabled)
		{
			_activeRequest ??= displayRequestManager.RequestActive();
		}
		else
		{
			_activeRequest?.Dispose();
			_activeRequest = null;
		}
	}
}
