using Matching.Application.Domain.Entities;

namespace Matching.Application.Domain.Factories
{
    public interface ISwipeIdFactory
    {
        SwipeId Create(string fromUserId, string toUserId, SwipeType type);
    }
}
