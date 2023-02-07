using Matching.Application.Domain.Entities;

namespace Matching.Application.Domain.Factories
{
    public class SwipeIdFactory : ISwipeIdFactory
    {
        public SwipeId Create(string fromUserId, string toUserId, SwipeType type)
        {
            var value = $"{fromUserId}.{toUserId}.{type.ToString().ToLower()}";
            return new SwipeId(value);
        }
    }
}
