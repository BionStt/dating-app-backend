using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Matching.Api.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase<TContext>(this IHost host, Action<TContext, IServiceProvider> seeder, int? retry = 0) where TContext : DbContext
        {
            int retryForAvailability = retry.Value;
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<TContext>>();
            var context = services.GetRequiredService<TContext>();

            try
            {
                logger.LogInformation("Migrating database associated with context {DbContextName}", typeof(TContext).Name);
                InvokeSeeder(seeder, context, services);
                logger.LogInformation("Migrated database associated with context {DbContextName}", typeof(TContext).Name);
            }
            catch(SqlException ex)
            {
                logger.LogError(ex, "An error ocurred while migrated the database used on context {DbContextName}", typeof(TContext).Name);
                if (retry < 50)
                {
                    retryForAvailability++;
                    Thread.Sleep(500);
                    MigrateDatabase(host, seeder, retryForAvailability);
                }
            }

            return host;
        }

        private static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seeder, TContext context, IServiceProvider services) where TContext : DbContext
        {
            context.Database.Migrate();
            seeder(context, services);
        }
    }
}
