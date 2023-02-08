using EventBus.Messages.Common;


namespace EventBus.Messages.Events
{
    public class RightSwipeEvent : BaseEvent
    {
        //Required by serialization/deserialization
        private RightSwipeEvent() : base(Guid.Empty, default)
        {
            SwipeId = string.Empty;
            FromUserId = string.Empty;
            ToUserId = string.Empty;
        }
        public RightSwipeEvent(Guid eventId, DateTimeOffset sentAt, string swipeId, string fromUserId, string toUserId) : base(eventId, sentAt)
        {
            SwipeId = swipeId;
            FromUserId = fromUserId;
            ToUserId = toUserId;
        }
        public string SwipeId { get; set; }
        public string FromUserId { get; private set; }

        public string ToUserId { get; private set; }
    }
}
