using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;

namespace Matching.Tests.Extensions
{
    /**
     * 
     * The extensions methiods RemoveDbContext and EnsureDbCreated where took from :
     * https://www.azureblue.io/asp-net-core-integration-tests-with-test-containers-and-postgres/
     * **/
    public static class ServiceCollectionExtensions
    {
        public static void RemoveDbContext<T>(this IServiceCollection services) where T : DbContext
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<T>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }
        }

        public static void EnsureDbCreated<T>(this IServiceCollection services) where T : DbContext
        {
            var serviceProvider = services.BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var context = serviceProvider.GetRequiredService<T>();
            context.Database.EnsureCreated();
        }


        public static void RemoveCache(this IServiceCollection services)
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(RedisCacheOptions));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            var descriptor1 = services.SingleOrDefault(d => d.ServiceType == typeof(IDistributedCache));
            if (descriptor1 != null)
            {
                services.Remove(descriptor1);
            }
        }

    }
}
