
using Matching.Application.Domain.Entities;

namespace Matching.Tests.Domain.Entities
{
    public class MatchIdTests
    {
        [Fact]
        public void CreateMatchId_ShouldBeEqual_ToValue()
        {
            var value = "aaa.bbb";
            var matchId = new MatchId(value);
            Assert.Equal(value, matchId.Value);
        }
    }
}
