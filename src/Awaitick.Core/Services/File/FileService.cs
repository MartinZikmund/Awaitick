namespace Awaitick.Core.Services;

public class FileService : IFileService
{
	private readonly ILogger<FileService> _logger;

	public FileService(ILogger<FileService> logger)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	public async Task<string?> GetDataFileContentsAsync(string filePath)
	{
		var rootFolder = ApplicationData.Current.LocalFolder;
		try
		{
			var file = await rootFolder.GetFileAsync(filePath);
			return await FileIO.ReadTextAsync(file);
		}
		catch (FileNotFoundException)
		{
			// File not found is expected on first run - log at debug level
			_logger.LogDebug("Data file not found: {FilePath}", filePath);
			return null;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to read data file: {FilePath}", filePath);
			return null;
		}
	}

	public async Task SetDataFileContentsAsync(string filePath, string contents)
	{
		var rootFolder = ApplicationData.Current.LocalFolder;
		try
		{
			var file = await rootFolder.CreateFileAsync(filePath, CreationCollisionOption.ReplaceExisting);
			await FileIO.WriteTextAsync(file, contents);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to write data file: {FilePath}. Data may be lost!", filePath);
		}
	}
}
