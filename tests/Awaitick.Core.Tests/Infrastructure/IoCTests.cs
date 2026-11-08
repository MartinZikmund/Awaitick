using Awaitick.Core.Infrastructure;

namespace Awaitick.Core.Tests.Infrastructure;

[TestClass]
[DoNotParallelize]
public class IoCTests
{
	[TestMethod]
	public void GetService_AfterSetProvider_ReturnsServiceFromProvider()
	{
		SampleService expected = new();

		IoC.SetProvider(new StubServiceProvider(expected));

		Assert.AreSame(expected, IoC.GetService<SampleService>());
	}

	[TestMethod]
	public void GetRequiredService_AfterSetProvider_ReturnsServiceFromProvider()
	{
		SampleService expected = new();

		IoC.SetProvider(new StubServiceProvider(expected));

		Assert.AreSame(expected, IoC.GetRequiredService<SampleService>());
	}

	[TestMethod]
	public void GetRequiredService_WhenServiceMissing_ThrowsInvalidOperationException()
	{
		IoC.SetProvider(new StubServiceProvider(null));

		Assert.ThrowsExactly<InvalidOperationException>(() => IoC.GetRequiredService<SampleService>());
	}

	[TestMethod]
	public void SetProvider_WithNull_ThrowsArgumentNullException() =>
		Assert.ThrowsExactly<ArgumentNullException>(() => IoC.SetProvider(null!));

	private sealed class SampleService;

	private sealed class StubServiceProvider(SampleService? service) : IServiceProvider
	{
		public object? GetService(Type serviceType) =>
			serviceType == typeof(SampleService) ? service : null;
	}
}
