using EventBus.Messages.Common;


namespace EventBus.Messages.Events
{
    public class MatchEvent : BaseEvent
    {
        //Required by serialization/deserialization
        private MatchEvent() : base(Guid.Empty, default)
        {
            MatchId = string.Empty;
            PartnerOneId = string.Empty;
            PartnerTwoId = string.Empty;
        }

        public MatchEvent(Guid eventId, DateTimeOffset sentAt, string matchId, string partnerOneId, string partnerTwoId) : base(eventId, sentAt)
        {
            MatchId = matchId;
            PartnerOneId = partnerOneId;
            PartnerTwoId = partnerTwoId;
        }

        public string MatchId { get; private set; }

        public string PartnerOneId { get; private set; }

        public string PartnerTwoId { get; private set; }
    }
}
