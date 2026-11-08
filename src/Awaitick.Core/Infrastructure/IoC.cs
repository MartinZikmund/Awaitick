#nullable enable

namespace Awaitick.Core.Infrastructure;

public static class IoC
{
	private static IServiceProvider? _serviceProvider;

	public static T? GetService<T>()
		where T : class => EnsureServiceProvider().GetService<T>();

	public static T GetRequiredService<T>()
		where T : class => EnsureServiceProvider().GetRequiredService<T>();

	public static void SetProvider(IServiceProvider serviceProvider)
	{
		if (serviceProvider is null)
		{
			throw new ArgumentNullException(nameof(serviceProvider));
		}

		_serviceProvider = serviceProvider;
	}

	/// <summary>
	/// Clears the stored provider, mainly so tests can start from a clean state.
	/// </summary>
	public static void Reset() => _serviceProvider = null;

	// Returns the provider so callers don't read the field again after a concurrent Reset().
	private static IServiceProvider EnsureServiceProvider() =>
		_serviceProvider ?? throw new InvalidOperationException("Service provider was not yet initialized.");
}
