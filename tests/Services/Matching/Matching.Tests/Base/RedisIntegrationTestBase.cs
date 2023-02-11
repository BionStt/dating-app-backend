using Matching.Application.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Matching.Tests.Base
{
    public class RedisIntegrationTestBase : IClassFixture<RedisIntegrationTestFactory>
    {
        public RedisIntegrationTestFactory Factory { get; private set; }

        public ISwipesCacheRepository CacheRepository { get; private set; }

        public RedisIntegrationTestBase(RedisIntegrationTestFactory factory)
        {
            Factory = factory;
            var scope = factory.Services.CreateScope();
            CacheRepository = scope.ServiceProvider.GetRequiredService<ISwipesCacheRepository>();
        }
    }
}
