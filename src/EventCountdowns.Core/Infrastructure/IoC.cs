#nullable enable

using System;
using Microsoft.Extensions.DependencyInjection;

namespace EventCountdowns.Core.Infrastructure
{
    public static class IoC
    {
        private static IServiceProvider? _serviceProvider;

        public static T? GetService<T>()
            where T : class
        {
            EnsureServiceProvider();
            return _serviceProvider.GetService<T>();
        }

        public static T GetRequiredService<T>()
            where T : class
        {
            EnsureServiceProvider();
            return _serviceProvider.GetRequiredService<T>();
        }

        public static void SetProvider(IServiceProvider serviceProvider)
        {
            if (serviceProvider is null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            _serviceProvider = serviceProvider;
        }

        private static void EnsureServiceProvider()
        {
            if (_serviceProvider == null)
            {
                throw new InvalidOperationException("Service provider was not yet initialized.");
            }
        }
    }
}
