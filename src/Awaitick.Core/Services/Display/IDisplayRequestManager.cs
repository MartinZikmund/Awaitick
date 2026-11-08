namespace Awaitick.Core.Services.Display;

public interface IDisplayRequestManager
{
	/// <summary>
	/// Keeps the display on until the returned object is disposed.
	/// </summary>
	IDisposable RequestActive();
}
