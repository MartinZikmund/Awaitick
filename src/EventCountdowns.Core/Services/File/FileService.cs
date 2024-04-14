using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace EventCountdowns.Core.Services;

public class FileService : IFileService
{
	public async Task<string> GetDataFileContentsAsync(string filePath)
	{
		var rootFolder = ApplicationData.Current.LocalFolder;
		try
		{
			var file = await rootFolder.GetFileAsync(filePath);
			return await FileIO.ReadTextAsync(file);
		}
		catch
		{
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
		catch
		{

		}
	}
}
