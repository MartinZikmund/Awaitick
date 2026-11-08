using Awaitick.Core.Services.Settings;
using Awaitick.Core.Tests.Fakes;

namespace Awaitick.Core.Tests.Services;

[TestClass]
public class AppPreferencesTests
{
	[TestMethod]
	public void KeepScreenOn_WhenNeverSet_IsOff()
	{
		AppPreferences preferences = new(new FakePreferences());

		Assert.IsFalse(preferences.KeepScreenOn);
	}

	[TestMethod]
	public void KeepScreenOn_WhenTurnedOn_StaysOn()
	{
		AppPreferences preferences = new(new FakePreferences());

		preferences.KeepScreenOn = true;

		Assert.IsTrue(preferences.KeepScreenOn);
	}
}
