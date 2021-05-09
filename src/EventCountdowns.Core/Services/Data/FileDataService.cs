using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventCountdowns.Core.Models;
using EventCountdowns.Core.Services.File;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EventCountdowns.Core.Services.Data
{
    public class FileDataService : IDataService
    {
        private const string DataFileName = "events.data";

        private readonly IFileService _fileService;

        

        private List<EventCountdown> _eventCountdowns = null;

        public FileDataService(IFileService fileService)
        {
            _fileService = fileService;
        }

        public async Task InitializeAsync()
        {
            await LoadDataAsync();         
        }

        private async Task LoadDataAsync()
        {
            try
            {
                _eventCountdowns = new List<EventCountdown>();
                var eventsJson = await _fileService.GetDataFileContentsAsync(DataFileName);
                var eventsArray = JArray.Parse(eventsJson);
                foreach (var item in eventsArray)
                {
                    try
                    {
                        _eventCountdowns.Add(item.ToObject<EventCountdown>());
                    }
                    catch
                    {
                        //skip invalid value
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

            }
        }

        public Task<List<EventCountdown>> GetCountdownsAsync()
        {
            return Task.FromResult(new List<EventCountdown>(from eventCountdown in _eventCountdowns orderby eventCountdown.TargetDateTime select eventCountdown));
        }

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

        public Task<EventCountdown> GetCountdownAsync(string id)
        {
            return Task.FromResult((from c in _eventCountdowns where c.Id == id select c).SingleOrDefault());
        }

        public async Task AddCountdownsAsync( params EventCountdown[] sampleEvents)
        {
            _eventCountdowns.AddRange( sampleEvents );
            await SaveDataAsync();
        }
    }
}
