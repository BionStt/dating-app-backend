namespace Matching.Application.Domain.Entities
{
    public class Match
    {
        //Required by serialization/deserialization and EF Core
        private Match()
        {
            Id = default;
            PartnerOneId = string.Empty;
            PartnerTwoId = string.Empty;
            CreatedAt = default;
        }

        public Match(MatchId id, string partnerOneId, string partnerTwoId, DateTimeOffset createdAt)
        {
            Id = id;
            PartnerOneId = partnerOneId;
            PartnerTwoId = partnerTwoId;
            CreatedAt = createdAt;
        }

        public MatchId Id { get; private set; }
        public string PartnerOneId { get; private set; }
        public string PartnerTwoId { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
    }
}
