#nullable enable

using System;
using Microsoft.Extensions.DependencyInjection;

namespace EventCountdowns.Core.Infrastructure
{
    public static class IoC

    {
        private static IServiceProvider? _serviceProvider;

        public static T? Resolve<T>()
            where T : class
        {
            EnsureServiceProvider();
            return _serviceProvider.GetService<T>();
        }

        public static T ResolveRequired<T>()
            where T : class
        {
            EnsureServiceProvider();
            return _serviceProvider.GetRequiredService<T>();
        }

        internal static void SetProvider(IServiceProvider serviceProvider)
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
