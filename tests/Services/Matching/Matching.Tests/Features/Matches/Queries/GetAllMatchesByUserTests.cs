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
    public class GetAllMatchesByUserTests : MssqlIntegrationTestBase
    {
        private readonly IMatchIdFactory _matchIdFactory;
        private readonly IDateTimeProvider _dateTimeProvider;

        public GetAllMatchesByUserTests(MssqlIntegrationTestFactory factory) : base(factory)
        {
            var scope = factory.Services.CreateScope();
            _matchIdFactory = scope.ServiceProvider.GetRequiredService<IMatchIdFactory>();
            _dateTimeProvider = scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();
        }

        [Fact]
        public async Task GetAllMatchesByUser_When_UserHaveNot_Matches_ShouldReturn_EmptyList()
        {
            var partnerOneId = Guid.NewGuid().ToString();

            var client = Factory.CreateClient();

            var getAllMatchesByUserResponse = await client.GetAsync($"/api/matches/{partnerOneId}/all");

            Assert.Equal(HttpStatusCode.OK, getAllMatchesByUserResponse.StatusCode);

            var matches = await getAllMatchesByUserResponse.Content.ReadFromJsonAsync<List<GetAllMatchesByUserResponse>>();
            Assert.NotNull(matches);
            Assert.Empty(matches);
        }


        private Match GenerateMatch(string partnerOne, bool isEven)
        {
            var partnerOneId = isEven ? Guid.NewGuid().ToString() : partnerOne;
            var partnerTwoId = isEven ? partnerOne : Guid.NewGuid().ToString();
            var matchId = _matchIdFactory.Create(partnerOneId, partnerTwoId);
            var createdAt = _dateTimeProvider.NowUtcOffset();
            return new Match(matchId, partnerOneId, partnerTwoId, createdAt);
        }

        private List<Match> GenerateMatches(string partnerOne)
        {
            var matches = new List<Match>();
            for (int i = 1; i <= 20; i++)
            {
                var isEven = i % 2 == 0;
                matches.Add(GenerateMatch(partnerOne, isEven));
            }
            return matches;
        }

        [Fact]
        public async Task GetAllMatchesByUser_When_UserHaveMatches_ShouldReturn_List()
        {
            var partnerOneId = Guid.NewGuid().ToString();
            var generatedMatches = GenerateMatches(partnerOneId);

            Context.Matches.AddRange(generatedMatches);

            await Context.SaveChangesAsync();

            var client = Factory.CreateClient();

            var getAllMatchesByUserResponse = await client.GetAsync($"/api/matches/{partnerOneId}/all");

            Assert.Equal(HttpStatusCode.OK, getAllMatchesByUserResponse.StatusCode);

            var matches = await getAllMatchesByUserResponse.Content.ReadFromJsonAsync<List<GetAllMatchesByUserResponse>>();
            Assert.NotNull(matches);
            Assert.NotEmpty(matches);

            Assert.Equal(20, matches.Count);
        }

    }
}
