namespace Awaitick.Core.Services;

public interface IFileService
{
	Task<string> GetDataFileContentsAsync(string filePath);

	Task SetDataFileContentsAsync(string filePath, string contents);
}
