using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Matching.Application.Infrastructure.Persistence;
using Matching.Tests.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Matching.Tests.Base
{
    public class MssqlIntegrationTestFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly TestcontainerDatabase _container;

        public MssqlIntegrationTestFactory()
        {
            _container = new TestcontainersBuilder<MsSqlTestcontainer>()
               .WithDatabase(new MsSqlTestcontainerConfiguration 
               {
                   Database = "MatchingDb",
                   Password = "Admin.1234"
               })
               .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
               .WithCleanUp(true)
               .Build();
        }


        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveDbContext<MatchingDbContext>();
                services.AddDbContext<MatchingDbContext>(options => options.UseSqlServer(_container.ConnectionString));
                services.EnsureDbCreated<MatchingDbContext>();
            });
        }

        public async Task InitializeAsync() => await _container.StartAsync();

        public new async Task DisposeAsync()
        {
            await _container.DisposeAsync();
        }
    }
}
