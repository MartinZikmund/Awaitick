using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using EventCountdowns.Core.Services.Data;
using System.Diagnostics.CodeAnalysis;

namespace EventCountdowns.Services
{
    public static class DbContextFactory
    {
		[RequiresUnreferencedCode("EF does not support NativeAOT yet")]
		public static void AddSQLiteDbContext(this IServiceCollection services)
        {
			var path = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Data", "events.db");
            services.AddDbContext<CountdownDbContext>(options =>
                options.UseSqlite($"Data Source={path}"));

            services.AddScoped<IDataService, SQLiteDataService>();
        }

	}
}
