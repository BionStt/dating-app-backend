using Matching.Application.Common.Interfaces;
using Matching.Application.Domain.Entities;
using Matching.Application.Domain.Factories;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Matching.Application.Infrastructure.Cache
{
    public class SwipesCacheRepository : ISwipesCacheRepository
    {
        private readonly IDistributedCache _cache;
        private readonly ISwipeIdFactory _factory;

        public SwipesCacheRepository(IDistributedCache cache, ISwipeIdFactory factory)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public async Task CreateAsync(Swipe swipe, CancellationToken cancellationToken = default)
        {
            var objectToStr = JsonSerializer.Serialize(swipe);
            await _cache.SetStringAsync(swipe.Id.Value, objectToStr, cancellationToken);
        }

        public async Task<Swipe?> GetByIdAsync(SwipeId swipeId, CancellationToken cancellationToken = default)
        {
            var jsonStr = await _cache.GetStringAsync(swipeId.Value, cancellationToken);
            if (string.IsNullOrEmpty(jsonStr))
            {
                return null;
            }
            return JsonSerializer.Deserialize<Swipe>(jsonStr);
        }

        public async Task<bool> BothExistsByFromUserIdAndToUserIdAndType(string fromUserId, string toUserId, SwipeType type, CancellationToken cancellationToken = default)
        {
            var fromUserSwipeId = _factory.Create(fromUserId, toUserId, type);
            var toUserSwipeId = _factory.Create(toUserId, fromUserId, type);
            return await ExistsAsync(fromUserSwipeId, cancellationToken) &&  await ExistsAsync(toUserSwipeId, cancellationToken);
        }

        public async Task<bool> ExistsAsync(SwipeId swipeId, CancellationToken cancellationToken = default)
        {
            return await GetByIdAsync(swipeId, cancellationToken) != null;
        }

    }
}
