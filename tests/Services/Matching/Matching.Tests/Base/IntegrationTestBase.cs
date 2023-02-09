
using Matching.Application.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Matching.Tests.Base
{
    public class IntegrationTestBase : IClassFixture<IntegrationTestFactory>
    {
        public readonly IntegrationTestFactory Factory;
        public readonly MatchingDbContext Context;

        public IntegrationTestBase(IntegrationTestFactory factory)
        {
            Factory = factory;
            var scope = factory.Services.CreateScope();
            Context = scope.ServiceProvider.GetRequiredService<MatchingDbContext>();
        }
    }
}
