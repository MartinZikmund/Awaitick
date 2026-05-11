using Awaitick.Core.Models;
using Awaitick.Core.Models.Database;
using SQLite;

namespace Awaitick.Core.Services.Data;

public class SqliteDataService : IDataService
{
	private const string DatabaseFileName = "awaitick.db";

	private readonly ILogger<SqliteDataService> _logger;
	private SQLiteAsyncConnection? _connection;

	public SqliteDataService(ILogger<SqliteDataService> logger)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	public async Task InitializeAsync()
	{
		try
		{
			var dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, DatabaseFileName);
			_connection = new SQLiteAsyncConnection(dbPath);
			await _connection.CreateTableAsync<EventCountdownEntity>();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to initialize SQLite database: {FileName}", DatabaseFileName);
		}
	}

	public async Task<List<EventCountdown>> GetCountdownsAsync()
	{
		var connection = GetConnection();
		var entities = await connection.Table<EventCountdownEntity>().ToListAsync();
		return entities
			.OrderBy(e => e.TargetDateTime)
			.Select(EventCountdownMapper.ToModel)
			.ToList();
	}

	public async Task<EventCountdown?> GetCountdownAsync(string id)
	{
		var connection = GetConnection();
		var entity = await connection.FindAsync<EventCountdownEntity>(id);
		return entity is null ? null : EventCountdownMapper.ToModel(entity);
	}

	public async Task AddCountdownAsync(EventCountdown eventCountdown)
	{
		var connection = GetConnection();
		await connection.InsertAsync(EventCountdownMapper.ToEntity(eventCountdown));
	}

	public async Task AddCountdownsAsync(params EventCountdown[] sampleEvents)
	{
		var connection = GetConnection();
		var entities = sampleEvents.Select(EventCountdownMapper.ToEntity).ToList();
		await connection.InsertAllAsync(entities);
	}

	public async Task UpdateCountdownAsync(EventCountdown eventCountdown)
	{
		var connection = GetConnection();
		await connection.InsertOrReplaceAsync(EventCountdownMapper.ToEntity(eventCountdown));
	}

	public async Task UpdateCountdownsAsync(params EventCountdown[] eventCountdowns)
	{
		var connection = GetConnection();
		var entities = eventCountdowns.Select(EventCountdownMapper.ToEntity).ToList();
		await connection.RunInTransactionAsync(tx =>
		{
			foreach (var entity in entities)
			{
				tx.InsertOrReplace(entity);
			}
		});
	}

	public async Task DeleteCountdownAsync(string id)
	{
		var connection = GetConnection();
		await connection.DeleteAsync<EventCountdownEntity>(id);
	}

	public async Task DeleteAllCountdownsAsync()
	{
		var connection = GetConnection();
		await connection.DeleteAllAsync<EventCountdownEntity>();
	}

	private SQLiteAsyncConnection GetConnection() =>
		_connection ?? throw new InvalidOperationException(
			$"{nameof(SqliteDataService)} has not been initialized. Call {nameof(InitializeAsync)} first.");
}
