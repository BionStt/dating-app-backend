using EventBus.Messages.Common;

namespace EventBus.Messages.Events
{
    public class LeftSwipeEvent : BaseEvent
    {
        //Required by serialization/deserialization
        private LeftSwipeEvent() : base(Guid.Empty, default) 
        {
            FromUserId = string.Empty;
            ToUserId = string.Empty; 
        }

        public LeftSwipeEvent(Guid eventId, DateTimeOffset sentAt, string fromUserId, string toUserId) : base(eventId, sentAt)
        {
            FromUserId = fromUserId; 
            ToUserId = toUserId; 
        }
        public string FromUserId { get; private set; }

        public string ToUserId { get; private set;}
    }
}
