using System.Text.Json;
using Awaitick.Core.Models;

namespace Awaitick.Core.Services.Data;

public class FileDataService : IDataService
{
	private const string DataFileName = "events.data";

	private readonly IFileService _fileService;
	private readonly ILogger<FileDataService> _logger;
	private List<EventCountdown> _eventCountdowns = new();

	public FileDataService(IFileService fileService, ILogger<FileDataService> logger)
	{
		_fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	public async Task InitializeAsync()
	{
		try
		{
			var eventsJson = await _fileService.GetDataFileContentsAsync(DataFileName);
			_eventCountdowns = JsonSerializer.Deserialize(eventsJson, EventCountdownSerializerContext.Default.ListEventCountdown) ?? new List<EventCountdown>();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to initialize data from file: {FileName}", DataFileName);
			// If the file is corrupted or unreadable, reset the list
			_eventCountdowns = new List<EventCountdown>();
		}
	}

	private async Task SaveDataAsync()
	{
		try
		{
			await _fileService.SetDataFileContentsAsync(DataFileName, JsonSerializer.Serialize(_eventCountdowns, EventCountdownSerializerContext.Default.ListEventCountdown));
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to save data to file: {FileName}", DataFileName);
		}
	}

	public Task<List<EventCountdown>> GetCountdownsAsync() => Task.FromResult(new List<EventCountdown>(from eventCountdown in _eventCountdowns orderby eventCountdown.TargetDateTime select eventCountdown));

	public async Task UpdateCountdownAsync(EventCountdown eventCountdown)
	{
		UpdateCountdownInList(eventCountdown);
		await SaveDataAsync();
	}

	private bool UpdateCountdownInList(EventCountdown eventCountdown)
	{
		var existingCountdown = (from countdown in _eventCountdowns
								 where countdown.Id == eventCountdown.Id
								 select countdown).SingleOrDefault();
		if (existingCountdown != null)
		{
			existingCountdown.Name = eventCountdown.Name;
			existingCountdown.CelebrationMessage = eventCountdown.CelebrationMessage;
			existingCountdown.TargetDateTime = eventCountdown.TargetDateTime;
			existingCountdown.BackgroundImageUri = eventCountdown.BackgroundImageUri;
			existingCountdown.Theme = eventCountdown.Theme;
			existingCountdown.BackgroundImageOpacity = eventCountdown.BackgroundImageOpacity;
			existingCountdown.BackgroundColor = eventCountdown.BackgroundColor;
			return true;
		}
		return false;
	}

	public async Task UpdateCountdownsAsync(params EventCountdown[] eventCountdowns)
	{
		foreach (var countdown in eventCountdowns)
		{
			UpdateCountdownInList(countdown);
		}
		await SaveDataAsync();
	}

	public async Task DeleteCountdownAsync(string id)
	{
		var selectedCountdown =
			(from countdown in _eventCountdowns where countdown.Id == id select countdown).SingleOrDefault();
		if (selectedCountdown != null)
		{
			_eventCountdowns.Remove(selectedCountdown);
			await SaveDataAsync();
		}
	}

	public async Task AddCountdownAsync(EventCountdown eventCountdown)
	{
		_eventCountdowns.Add(eventCountdown);
		await SaveDataAsync();
	}

	public Task<EventCountdown?> GetCountdownAsync(string id) => Task.FromResult((from c in _eventCountdowns where c.Id == id select c).SingleOrDefault());

	public async Task AddCountdownsAsync(params EventCountdown[] sampleEvents)
	{
		_eventCountdowns.AddRange(sampleEvents);
		await SaveDataAsync();
	}
}
