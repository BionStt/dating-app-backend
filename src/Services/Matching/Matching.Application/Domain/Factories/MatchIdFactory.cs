using Matching.Application.Domain.Entities;

namespace Matching.Application.Domain.Factories
{
    public class MatchIdFactory : IMatchIdFactory
    {
        public MatchId Create(string partnerOneId, string partnerTwoId)
        {
            var value = $"{partnerOneId}.{partnerTwoId}";
            return new MatchId(value);
        }
    }
}
