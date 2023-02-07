using Matching.Application.Domain.Entities;
using Matching.Application.Domain.Factories;


namespace Matching.Tests.Domain.Entities
{
    public class MatchTests
    {
        private readonly IMatchIdFactory _factory;

        public MatchTests()
        {
            _factory = new MatchIdFactory();
        }

        [Fact]
        public void CreateMatch_Fields_ShouldBeEqual_ToValues()
        {
            var partnerOneId = "aaa";
            var partnerTwoId = "bbb";
            var matchId = _factory.Create(partnerOneId, partnerTwoId);
            var createdAt = DateTimeOffset.UtcNow;

            var match = new Match(matchId, partnerOneId, partnerTwoId, createdAt);

            Assert.Equal(matchId, match.Id);
            Assert.Equal(partnerOneId, match.PartnerOneId);
            Assert.Equal(partnerTwoId, match.PartnerTwoId);
            Assert.Equal(createdAt, match.CreatedAt);
        }
    }
}