using System.Diagnostics.CodeAnalysis;
using EventCountdowns.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace EventCountdowns.Core.Services.Data;

public class SQLiteDataService : IDataService
{
	private readonly CountdownDbContext _context;

	public SQLiteDataService(CountdownDbContext context)
	{
		_context = context;
	}

	public async Task InitializeAsync()
	{
		await ApplicationData.Current.LocalFolder.CreateFolderAsync("Data", CreationCollisionOption.OpenIfExists);
		await _context.Database.EnsureCreatedAsync();
	}

	public async Task<List<EventCountdown>> GetCountdownsAsync() => await _context.Countdowns.AsNoTracking().ToListAsync();

	public async Task UpdateCountdownAsync(EventCountdown eventCountdown)
	{
		_context.Countdowns.Update(eventCountdown);
		await _context.SaveChangesAsync();
	}

	public async Task UpdateCountdownsAsync(params EventCountdown[] eventCountdowns)
	{
		_context.Countdowns.UpdateRange(eventCountdowns);
		await _context.SaveChangesAsync();
	}

	public async Task DeleteCountdownAsync(string id)
	{
		var countdown = await _context.Countdowns.FindAsync(id);
		if (countdown != null)
		{
			_context.Countdowns.Remove(countdown);
			await _context.SaveChangesAsync();
		}
	}

	public async Task AddCountdownAsync(EventCountdown eventCountdown)
	{
		await _context.Countdowns.AddAsync(eventCountdown);
		await _context.SaveChangesAsync();
	}

	public async Task<EventCountdown?> GetCountdownAsync(string id) => await _context.Countdowns.FindAsync(id);

	public async Task AddCountdownsAsync(params EventCountdown[] sampleEvents)
	{
		await _context.Countdowns.AddRangeAsync(sampleEvents);
		await _context.SaveChangesAsync();
	}
}

public class CountdownDbContext : DbContext
{
	[RequiresUnreferencedCode("EF does not support NativeAOT yet")]
	public CountdownDbContext(DbContextOptions<CountdownDbContext> options) : base(options) { }

	public DbSet<EventCountdown> Countdowns { get; set; }
}
