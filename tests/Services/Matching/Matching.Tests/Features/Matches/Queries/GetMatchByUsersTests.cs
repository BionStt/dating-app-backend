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
    public class GetMatchByUsersTests : MssqlIntegrationTestBase
    {
        private readonly IMatchIdFactory _matchIdFactory;
        private readonly IDateTimeProvider _dateTimeProvider;
        public GetMatchByUsersTests(MssqlIntegrationTestFactory factory) : base(factory)
        {
            var scope = factory.Services.CreateScope();
            _matchIdFactory = scope.ServiceProvider.GetRequiredService<IMatchIdFactory>();
            _dateTimeProvider = scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();
        }

        [Fact]
        public async Task GetMatchByUsers_WhenMatch_NotExists_ShouldReturn404()
        {
            var partnerOneId = Guid.NewGuid().ToString();
            var partnerTwoId = Guid.NewGuid().ToString();
            var client = Factory.CreateClient();
            
            var getMatchByUsersResponse = await client.GetAsync($"/api/matches/{partnerOneId}/{partnerTwoId}");
            Assert.Equal(HttpStatusCode.NotFound, getMatchByUsersResponse.StatusCode);
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
        public async Task GetMatchByUsers_WhenMatch_Exists_ShouldReturnMatch()
        {
            var match = CreateMatch();
            var partnerOneId = match.PartnerOneId;
            var partnerTwoId = match.PartnerTwoId;

            Context.Matches.Add(match);
            await Context.SaveChangesAsync();

            var client = Factory.CreateClient();

            var getMatchByUsersResponse = await client.GetAsync($"/api/matches/{partnerOneId}/{partnerTwoId}");
            AssertMatchResponse(getMatchByUsersResponse);

            var getMatchByUsersResponse1 = await client.GetAsync($"/api/matches/{partnerTwoId}/{partnerOneId}");
            AssertMatchResponse(getMatchByUsersResponse1);
        }


        private async void AssertMatchResponse(HttpResponseMessage message)
        {
            Assert.Equal(HttpStatusCode.OK, message.StatusCode);
            var foundMatch = await message.Content.ReadFromJsonAsync<GetMatchByUsersResponse>();
            Assert.NotNull(foundMatch);

        }
    }
}
