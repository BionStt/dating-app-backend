using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using MassTransit;
using Matching.Tests.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Matching.Tests.Base
{
    public class RedisIntegrationTestFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly TestcontainerDatabase _cacheContainer;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveCache();
                services.AddStackExchangeRedisCache(opt => opt.Configuration = _cacheContainer.ConnectionString);
                services.AddMassTransitTestHarness(cfg =>{});
            });
        }


        public RedisIntegrationTestFactory()
        {
            _cacheContainer = new TestcontainersBuilder<RedisTestcontainer>()
               .WithDatabase(new RedisTestcontainerConfiguration
               {
               })
               .WithImage("redis:latest")
               .WithCleanUp(true)
               .Build();
        }

        public async Task InitializeAsync()
        {
            await _cacheContainer.StartAsync();
        }

        public new async Task DisposeAsync()
        {
            await _cacheContainer.DisposeAsync();
        }
    }
}
