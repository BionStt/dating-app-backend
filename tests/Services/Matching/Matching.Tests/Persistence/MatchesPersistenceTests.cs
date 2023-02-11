using Matching.Application.Domain.Entities;
using Matching.Application.Domain.Factories;
using Matching.Tests.Base;
using Microsoft.Extensions.DependencyInjection;
using Utils.Time;

namespace Matching.Tests.Persistence
{
    public class MatchesPersistenceTests : MssqlIntegrationTestBase
    {
        private readonly IMatchIdFactory _matchIdfactory;
        private readonly IDateTimeProvider _dateTimeProvider;

        public MatchesPersistenceTests(MssqlIntegrationTestFactory factory) : base(factory)
        {
            var scope = factory.Services.CreateScope();
            _matchIdfactory = scope.ServiceProvider.GetRequiredService<IMatchIdFactory>();
            _dateTimeProvider = scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();
        }

        [Fact]
        public async Task CreateMatch_When_MatchExists_ShouldThrow_InvalidOperationException()
        {
            var match = new Match(
                _matchIdfactory.Create("bbb", "aaa"),
                "bbb",
                "aaa",
                _dateTimeProvider.NowUtcOffset()
            );

            var newMatch = new Match(
                _matchIdfactory.Create("bbb", "aaa"),
                "bbb",
                "aaa",
                _dateTimeProvider.NowUtcOffset()
            );


            var task = async () =>
            {
                Context.Matches.Add(match);
                await Context.SaveChangesAsync();
                Context.Matches.Add(newMatch);
                await Context.SaveChangesAsync();
            };

            await Assert.ThrowsAsync<InvalidOperationException>(task);

        }

        [Fact]
        public async Task CreateMatch_When_FindCreatedMatchById_ShouldReturnMatch()
        {
            var partnerOneId = "aaa";
            var partnerTwoId = "bbb";
            var matchId = _matchIdfactory.Create(partnerOneId, partnerTwoId);
            var createdAt = _dateTimeProvider.NowUtcOffset();

            var match = new Match(
                matchId,
                partnerOneId,
                partnerTwoId,
                createdAt
            );

            Context.Matches.Add(match);

            await Context.SaveChangesAsync();

            var foundMatch = await Context.Matches.FindAsync(matchId);

            Assert.NotNull(foundMatch);

            Assert.Equal(match.Id, foundMatch.Id);
            Assert.Equal(match.PartnerOneId, foundMatch.PartnerOneId);
            Assert.Equal(match.PartnerTwoId, foundMatch.PartnerTwoId);
            Assert.Equal(match.CreatedAt, foundMatch.CreatedAt);
        }
        

    }
}
