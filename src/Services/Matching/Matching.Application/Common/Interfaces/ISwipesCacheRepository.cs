using Matching.Application.Domain.Entities;

namespace Matching.Application.Common.Interfaces
{
    public interface ISwipesCacheRepository
    {
        Task CreateAsync(Swipe swipe, CancellationToken cancellationToken = default);
        Task<Swipe?> GetByIdAsync(SwipeId swipeId, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(SwipeId swipeId, CancellationToken cancellationToken = default);
        Task<bool> BothExistsByFromUserIdAndToUserIdAndType(string fromUserId, string toUserId, SwipeType type, CancellationToken cancellationToken = default);

    }
}
