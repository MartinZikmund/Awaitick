using Awaitick.Core.Models;

namespace Awaitick.Core.DefaultData;

public interface IDefaultBackgrounds
{
	Task<DefaultBackground[]> GetDefaultBackgroundsAsync();
}
