using Matching.Application.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Matching.Tests.Base
{
    public class MssqlIntegrationTestBase : IClassFixture<MssqlIntegrationTestFactory>
    {
        public MssqlIntegrationTestFactory Factory { get; private set; }
        public MatchingDbContext Context { get; private set; }

        public MssqlIntegrationTestBase(MssqlIntegrationTestFactory factory)
        {
            Factory = factory;
            var scope = factory.Services.CreateScope();
            Context = scope.ServiceProvider.GetRequiredService<MatchingDbContext>();
        }

    }
}
