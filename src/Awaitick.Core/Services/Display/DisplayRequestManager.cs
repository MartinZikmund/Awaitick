using Windows.System.Display;

namespace Awaitick.Core.Services.Display;

/// <remarks>
/// <see cref="DisplayRequest"/> isn't agile, so create and use this on the UI thread.
/// </remarks>
public sealed class DisplayRequestManager : IDisplayRequestManager
{
	private readonly DisplayRequest _displayRequest = new();

	public IDisposable RequestActive()
	{
		_displayRequest.RequestActive();
		return new ActiveRequest(_displayRequest);
	}

	private sealed class ActiveRequest(DisplayRequest displayRequest) : IDisposable
	{
		private bool _released;

		public void Dispose()
		{
			if (!_released)
			{
				_released = true;
				displayRequest.RequestRelease();
			}
		}
	}
}
