namespace Matching.Application.Domain.Entities
{
    public class Swipe
    {
        //Required by serialization/deserialization and EF Core
        private Swipe()
        {
            Id = default;
            FromUserId = string.Empty;
            ToUserId = string.Empty;
            Type = default;
            CreatedAt = default;
        }

        public Swipe(SwipeId id, string fromUserId, string toUserId, SwipeType type, DateTimeOffset createdAt)
        {
            Id = id;
            FromUserId = fromUserId;
            ToUserId = toUserId;
            Type = type;
            CreatedAt = createdAt;
        }

        public SwipeId Id { get; private set; }
        public string FromUserId { get; private set; }
        public string ToUserId { get; private set; }
        public SwipeType Type { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }

    }
}
