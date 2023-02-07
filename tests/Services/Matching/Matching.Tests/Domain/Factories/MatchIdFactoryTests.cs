using Matching.Application.Domain.Entities;
using Matching.Application.Domain.Factories;
namespace Matching.Tests.Domain.Factories
{
    public class MatchIdFactoryTests
    {
        private readonly IMatchIdFactory _factory;

        public MatchIdFactoryTests()
        { 
            _factory = new MatchIdFactory();
        }

        [Fact]
        public void CreateMatchId_MatchId_ShouldBeEqualToValue()
        {
            var partnerOneId = "aaa";
            var partnerTwoId = "bbb";
            var value = "aaa.bbb";
            
            var expected = new MatchId(value);
            var actual = _factory.Create(partnerOneId, partnerTwoId);

            Assert.Equal(expected, actual);
        }

    }
}
