using Awaitick.Core.Infrastructure;

namespace Awaitick.Core.Tests.Infrastructure;

[TestClass]
[DoNotParallelize]
public class IoCTests
{
	[TestCleanup]
	public void TestCleanup() => IoC.Reset();

	[TestMethod]
	public void GetService_AfterReset_ThrowsInvalidOperationException()
	{
		IoC.SetProvider(new StubServiceProvider(new SampleService()));

		IoC.Reset();

		Assert.ThrowsExactly<InvalidOperationException>(() => IoC.GetService<SampleService>());
	}

	[TestMethod]
	public void GetRequiredService_AfterReset_ThrowsInvalidOperationException()
	{
		IoC.SetProvider(new StubServiceProvider(new SampleService()));

		IoC.Reset();

		Assert.ThrowsExactly<InvalidOperationException>(() => IoC.GetRequiredService<SampleService>());
	}

	[TestMethod]
	public void SetProvider_AfterReset_ResolvesFromNewProvider()
	{
		SampleService first = new();
		SampleService second = new();
		IoC.SetProvider(new StubServiceProvider(first));

		IoC.Reset();
		IoC.SetProvider(new StubServiceProvider(second));

		Assert.AreSame(second, IoC.GetRequiredService<SampleService>());
	}

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
