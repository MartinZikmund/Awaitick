namespace Awaitick.Core.Services.DeepLink;

/// <summary>
/// Service for handling deep link navigation from notifications and external sources.
/// </summary>
public interface IDeepLinkService
{
	/// <summary>
	/// Sets a pending navigation to a specific countdown.
	/// Called when the app is activated from a notification.
	/// </summary>
	/// <param name="countdownId">The countdown ID to navigate to.</param>
	void SetPendingNavigation(string countdownId);

	/// <summary>
	/// Consumes and returns the pending countdown ID for navigation.
	/// Returns null if no pending navigation exists.
	/// </summary>
	/// <returns>The countdown ID to navigate to, or null.</returns>
	string? ConsumePendingNavigation();

	/// <summary>
	/// Gets whether there is a pending navigation.
	/// </summary>
	bool HasPendingNavigation { get; }
}
