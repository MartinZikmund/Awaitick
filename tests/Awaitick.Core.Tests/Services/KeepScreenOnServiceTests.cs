using Awaitick.Core.Services.Display;

namespace Awaitick.Core.Tests.Services;

[TestClass]
public class KeepScreenOnServiceTests
{
	[TestMethod]
	public void SetEnabled_WhenEnabled_RequestsActiveDisplay()
	{
		FakeDisplayRequestManager manager = new();
		KeepScreenOnService service = new(manager);

		service.SetEnabled(true);

		Assert.AreEqual(1, manager.ActiveRequests);
	}

	[TestMethod]
	public void SetEnabled_WhenEnabledTwice_HoldsOneRequest()
	{
		FakeDisplayRequestManager manager = new();
		KeepScreenOnService service = new(manager);

		service.SetEnabled(true);
		service.SetEnabled(true);

		Assert.AreEqual(1, manager.ActiveRequests);
	}

	[TestMethod]
	public void SetEnabled_WhenDisabledAfterEnabled_ReleasesRequest()
	{
		FakeDisplayRequestManager manager = new();
		KeepScreenOnService service = new(manager);
		service.SetEnabled(true);

		service.SetEnabled(false);

		Assert.AreEqual(0, manager.ActiveRequests);
	}

	[TestMethod]
	public void SetEnabled_WhenDisabledTwice_ReleasesOnce()
	{
		FakeDisplayRequestManager manager = new();
		KeepScreenOnService service = new(manager);
		service.SetEnabled(true);

		service.SetEnabled(false);
		service.SetEnabled(false);

		Assert.AreEqual(0, manager.ActiveRequests);
	}

	[TestMethod]
	public void SetEnabled_WhenDisabledWithoutRequest_DoesNotRequest()
	{
		FakeDisplayRequestManager manager = new();
		KeepScreenOnService service = new(manager);

		service.SetEnabled(false);

		Assert.AreEqual(0, manager.TotalRequests);
	}

	[TestMethod]
	public void SetEnabled_WhenEnabledAgain_RequestsAgain()
	{
		FakeDisplayRequestManager manager = new();
		KeepScreenOnService service = new(manager);
		service.SetEnabled(true);
		service.SetEnabled(false);

		service.SetEnabled(true);

		Assert.AreEqual(1, manager.ActiveRequests);
		Assert.AreEqual(2, manager.TotalRequests);
	}

	private sealed class FakeDisplayRequestManager : IDisplayRequestManager
	{
		public int ActiveRequests { get; private set; }

		public int TotalRequests { get; private set; }

		public IDisposable RequestActive()
		{
			ActiveRequests++;
			TotalRequests++;
			return new Release(this);
		}

		private sealed class Release(FakeDisplayRequestManager owner) : IDisposable
		{
			public void Dispose() => owner.ActiveRequests--;
		}
	}
}
