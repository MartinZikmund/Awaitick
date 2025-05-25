using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Awaitick.Core.Services.Data;

namespace Awaitick.Services
{
    public static class DbContextFactory
    {
        public static void AddSQLiteDbContext(this IServiceCollection services)
        {
			var path = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Data", "events.db");
            services.AddDbContext<CountdownDbContext>(options =>
                options.UseSqlite($"Data Source={path}"));

            services.AddScoped<IDataService, SQLiteDataService>();
        }

	}
}
