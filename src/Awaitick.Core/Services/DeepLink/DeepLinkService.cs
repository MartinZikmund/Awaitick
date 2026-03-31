namespace Awaitick.Core.Services.DeepLink;

/// <summary>
/// Thread-safe implementation of deep link navigation handling.
/// </summary>
public class DeepLinkService : IDeepLinkService
{
	private string? _pendingCountdownId;
	private readonly object _lock = new();

	public void SetPendingNavigation(string countdownId)
	{
		lock (_lock)
		{
			_pendingCountdownId = countdownId;
		}
	}

	public string? ConsumePendingNavigation()
	{
		lock (_lock)
		{
			var id = _pendingCountdownId;
			_pendingCountdownId = null;
			return id;
		}
	}

	public bool HasPendingNavigation
	{
		get
		{
			lock (_lock)
			{
				return _pendingCountdownId != null;
			}
		}
	}
}
