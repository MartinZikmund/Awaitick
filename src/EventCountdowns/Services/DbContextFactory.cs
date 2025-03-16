using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using EventCountdowns.Core.Services.Data;

namespace EventCountdowns.Services
{
    public static class DbContextFactory
    {
        public static void AddSQLiteDbContext(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<CountdownDbContext>(options =>
                options.UseSqlite(connectionString));

            services.AddScoped<IDataService, SQLiteDataService>();
        }
    }
}