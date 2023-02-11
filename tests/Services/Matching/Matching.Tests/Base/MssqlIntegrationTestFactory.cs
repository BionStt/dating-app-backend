using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using MassTransit;
using Matching.Application.Infrastructure.Dapper;
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
        private readonly TestcontainerDatabase _databaseContainer;
       
        public MssqlIntegrationTestFactory()
        {
            _databaseContainer = new TestcontainersBuilder<MsSqlTestcontainer>()
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
                services.AddDbContext<MatchingDbContext>(options => options.UseSqlServer(_databaseContainer.ConnectionString));
                services.RemoveDapper();
                services.Configure<DapperConfig>(opt => opt.ConnectionString = _databaseContainer.ConnectionString);
                services.EnsureDbCreated<MatchingDbContext>();
                services.AddMassTransitTestHarness(cfg => {});
            });
        }

        public async Task InitializeAsync()
        {
            await _databaseContainer.StartAsync();
        }

        public new async Task DisposeAsync()
        {
            await _databaseContainer.DisposeAsync();
        }
    }
}
