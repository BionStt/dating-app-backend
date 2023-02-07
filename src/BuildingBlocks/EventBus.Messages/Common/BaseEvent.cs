namespace EventBus.Messages.Common
{
    public abstract class BaseEvent
    {
        public BaseEvent(Guid eventId, DateTimeOffset sentAt)
        {
            EventId = eventId;
            SentAt = sentAt;
        }

        public Guid EventId { get; protected set; }
        public DateTimeOffset SentAt { get; protected set; }
    }
}
