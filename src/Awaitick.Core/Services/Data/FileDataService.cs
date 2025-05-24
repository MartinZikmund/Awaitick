using Awaitick.Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Awaitick.Core.Services.Data;

public class FileDataService : IDataService
{
	private const string DataFileName = "events.data";

	private readonly IFileService _fileService;

	private List<EventCountdown> _eventCountdowns = new();

	public FileDataService(IFileService fileService)
	{
		_fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
	}

	public async Task InitializeAsync()
	{
		try
		{
			_eventCountdowns.Clear();
			var eventsJson = await _fileService.GetDataFileContentsAsync(DataFileName);
			if (string.IsNullOrEmpty(eventsJson))
			{
				// No data file found or empty, initialize with an empty list
				_eventCountdowns = new List<EventCountdown>();
				return;
			}

			var eventsArray = JArray.Parse(eventsJson);
			foreach (var item in eventsArray)
			{
				try
				{
					var parsedItem = item.ToObject<EventCountdown>();
					if (parsedItem != null)
					{
						_eventCountdowns.Add(parsedItem);
					}
				}
				catch
				{
					//TODO:LOG
				}
			}
		}
		catch
		{
			_eventCountdowns = new List<EventCountdown>();
		}
	}

	private async Task SaveDataAsync()
	{
		try
		{
			await _fileService.SetDataFileContentsAsync(DataFileName, JsonConvert.SerializeObject(_eventCountdowns));
		}
		catch
		{
			//TODO:LOG
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
			//update values                
			existingCountdown.Name = eventCountdown.Name;
			existingCountdown.TargetDateTime = eventCountdown.TargetDateTime;
			existingCountdown.BackgroundImagePath = eventCountdown.BackgroundImagePath;
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
