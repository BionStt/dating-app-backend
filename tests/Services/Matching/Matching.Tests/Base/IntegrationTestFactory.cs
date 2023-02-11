using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using MassTransit;
using Matching.Application.Features.Matches.EventHandlers;
using Matching.Application.Features.Swipes.EventHandlers;
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
    // Based on aricle : https://www.azureblue.io/asp-net-core-integration-tests-with-test-containers-and-postgres/
    public class IntegrationTestFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly TestcontainerDatabase _databaseContainer;
        private readonly TestcontainerDatabase _cacheContainer;

        public IntegrationTestFactory()
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

            _cacheContainer = new TestcontainersBuilder<RedisTestcontainer>()
                .WithDatabase(new RedisTestcontainerConfiguration
                {
                })
                .WithImage("redis:latest")
                .WithCleanUp(true)
                .Build();
        }


        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveCache();
                services.AddStackExchangeRedisCache(opt => opt.Configuration = _cacheContainer.ConnectionString);
                services.RemoveDbContext<MatchingDbContext>();
                services.AddDbContext<MatchingDbContext>(options => options.UseSqlServer(_databaseContainer.ConnectionString));
                services.RemoveDapper();
                services.Configure<DapperConfig>(opt => opt.ConnectionString = _databaseContainer.ConnectionString);
                services.EnsureDbCreated<MatchingDbContext>();

                services.AddMassTransitTestHarness(cfg =>
                {
                    cfg.AddConsumer<RightSwipeEventHandler>();
                    cfg.AddConsumer<LeftSwipeEventHandler>();
                    cfg.AddConsumer<MatchEventHandler>();
                });

            });
        }

        public async Task InitializeAsync()
        {

            await _databaseContainer.StartAsync();
            await _cacheContainer.StartAsync();
        }

        public new async Task DisposeAsync()
        {
            await _databaseContainer.DisposeAsync();
            await _cacheContainer.DisposeAsync();
        }
    }
}
