using Matching.Application.Domain.Entities;

namespace Matching.Application.Domain.Factories
{
    public interface IMatchIdFactory
    {
        MatchId Create(string partnerOneId, string partnerTwoId);
    }
}
