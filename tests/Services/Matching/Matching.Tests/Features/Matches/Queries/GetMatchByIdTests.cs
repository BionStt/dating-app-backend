using Matching.Application.Domain.Entities;
using Matching.Application.Domain.Factories;
using Matching.Application.Features.Matches.Queries;
using Matching.Tests.Base;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Utils.Time;

namespace Matching.Tests.Features.Matches.Queries
{
    public class GetMatchByIdTests : MssqlIntegrationTestBase
    {
        private readonly IMatchIdFactory _matchIdFactory;
        private readonly IDateTimeProvider _dateTimeProvider;

        public GetMatchByIdTests(MssqlIntegrationTestFactory factory) : base(factory)
        {
            var scope = factory.Services.CreateScope();
            _matchIdFactory = scope.ServiceProvider.GetRequiredService<IMatchIdFactory>();
            _dateTimeProvider = scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();
        }

        [Fact]
        public async Task GetMatchById_WhenMatch_NotExists_ShouldReturn404()
        {
            var matchId = Guid.NewGuid().ToString();
            var client = Factory.CreateClient();

            var getMatchByIdResponse = await client.GetAsync($"/api/matches/{matchId}");
            Assert.Equal(HttpStatusCode.NotFound, getMatchByIdResponse.StatusCode);
        }


        private Match CreateMatch()
        {
            var partnerOneId = Guid.NewGuid().ToString();
            var partnerTwoId = Guid.NewGuid().ToString();
            var createdAt = _dateTimeProvider.NowUtcOffset();
            var swipeId = _matchIdFactory.Create(partnerOneId, partnerTwoId);
            return new Match(swipeId, partnerOneId, partnerTwoId, createdAt);
        }

        [Fact]
        public async Task GetMatchById_WhenMatch_Exists_ShouldReturnMatch()
        {
            var match = CreateMatch();
            var matchId = match.Id.Value;

            Context.Matches.Add(match);
            await Context.SaveChangesAsync();

            var client = Factory.CreateClient();
            var getMatchByIdResponse = await client.GetAsync($"/api/matches/{matchId}");

            Assert.Equal(HttpStatusCode.OK, getMatchByIdResponse.StatusCode);

            var foundMatch = await getMatchByIdResponse.Content.ReadFromJsonAsync<GetMatchByIdResponse>();

            Assert.NotNull(foundMatch);
            Assert.Equal(matchId, foundMatch.Id);
            Assert.Equal(match.PartnerOneId, foundMatch.PartnerOneId);
            Assert.Equal(match.PartnerTwoId, foundMatch.PartnerTwoId);
            Assert.Equal(match.CreatedAt, foundMatch.CreatedAt);
        }
    }
}
