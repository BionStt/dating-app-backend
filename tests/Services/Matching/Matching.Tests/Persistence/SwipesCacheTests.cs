using Matching.Application.Domain.Entities;
using Matching.Application.Domain.Factories;
using Matching.Tests.Base;
using Microsoft.Extensions.DependencyInjection;
using Utils.Time;

namespace Matching.Tests.Persistence
{
    public class SwipesCacheTests : RedisIntegrationTestBase
    {
        private readonly ISwipeIdFactory _swipeIdFactory;
        private readonly IDateTimeProvider _dateTimeProvider;

        public SwipesCacheTests(RedisIntegrationTestFactory factory) : base(factory)
        {
            var scope = factory.Services.CreateScope();
            _swipeIdFactory = scope.ServiceProvider.GetRequiredService<ISwipeIdFactory>();
            _dateTimeProvider = scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();
        }

        [Fact]
        public async Task CreateSwipe_ShouldBe_InCache()
        {
            var fromUserId = "aaa";
            var toUserId = "bbb";
            var type = SwipeType.Right;
            var swipeId = _swipeIdFactory.Create(fromUserId, toUserId, type);
            var createdAt = _dateTimeProvider.NowUtcOffset();

            var swipe = new Swipe(swipeId, fromUserId, toUserId, type, createdAt);
            
            await CacheRepository.CreateAsync(swipe);

            var cachedSwipe = await CacheRepository.GetByIdAsync(swipeId);
            var exists = await CacheRepository.ExistsAsync(swipeId);

            Assert.NotNull(cachedSwipe);
            Assert.True(exists);

            Assert.Equal(swipe.Id, cachedSwipe.Id);
            Assert.Equal(swipe.FromUserId, cachedSwipe.FromUserId);
            Assert.Equal(swipe.ToUserId, cachedSwipe.ToUserId);
            Assert.Equal(swipe.Type, cachedSwipe.Type);
            Assert.Equal(swipe.CreatedAt, cachedSwipe.CreatedAt);
        }

        [Fact]
        public async Task WhenBothUsersSwipe_ShouldReturnTrue()
        {
            var fromUserId = "aaa";
            var toUserId = "bbb";
            var type = SwipeType.Right;
            var swipeId = _swipeIdFactory.Create(fromUserId, toUserId, type);
            var createdAt = _dateTimeProvider.NowUtcOffset();

            var fromUserSwipe = new Swipe(swipeId, fromUserId, toUserId, type, createdAt);

            var fromUserId1 = "bbb";
            var toUserId1 = "aaa";
            var type1 = SwipeType.Right;
            var swipeId1 = _swipeIdFactory.Create(fromUserId1, toUserId1, type1);
            var createdAt1 = _dateTimeProvider.NowUtcOffset();
            
            var toUsersSwipe = new Swipe(swipeId1, fromUserId1, toUserId1, type1, createdAt1);


            await CacheRepository.CreateAsync(fromUserSwipe);
            await CacheRepository.CreateAsync(toUsersSwipe);

            bool exists = await CacheRepository.BothExistsByFromUserIdAndToUserIdAndType(fromUserId, toUserId, type);

            Assert.True(exists);
        }


    }
}
