//using EventCountdowns.Core.Models;
//using LiteDB;

//namespace EventCountdowns.Core.Services.Data;
//internal class LiteDbDataService : IDataService
//{
//	public Task AddCountdownAsync(EventCountdown eventCountdown)
//	{

//	}
//	public Task AddCountdownsAsync(params EventCountdown[] sampleEvents) => throw new NotImplementedException();
//	public Task DeleteCountdownAsync(string id) => throw new NotImplementedException();
//	public Task<EventCountdown> GetCountdownAsync(string id) => throw new NotImplementedException();
//	public Task<List<EventCountdown>> GetCountdownsAsync() => throw new NotImplementedException();
//	public Task InitializeAsync()
//	{
//		BsonMapper.Global.RegisterType<DateTimeOffset>
//		(
//			serialize: obj =>
//			{
//				var doc = new BsonDocument();
//				doc["DateTime"] = obj.DateTime.Ticks;
//				doc["Offset"] = obj.Offset.Ticks;
//				return doc;
//			},
//			deserialize: doc => new DateTimeOffset(doc["DateTime"].AsInt64, new TimeSpan(doc["Offset"].AsInt64))
//		);
//		return Task.CompletedTask;
//	}
//	public Task UpdateCountdownAsync(EventCountdown eventCountdown) => throw new NotImplementedException();
//	public Task UpdateCountdownsAsync(params EventCountdown[] eventCountdowns) => throw new NotImplementedException();
//}
