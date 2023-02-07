using Matching.Application.Domain.Entities;
using Matching.Application.Domain.Factories;

namespace Matching.Tests.Domain.Entities
{
    public class SwipeTests
    {
        private readonly ISwipeIdFactory _factory;

        public SwipeTests()
        {
            _factory = new SwipeIdFactory();
        }

        [Fact]
        public void CreateSwipe_Fields_ShouldBeEqual_ToValues()
        {
            var fromUserId = "aaa";
            var toUserId = "bbb";
            var type = SwipeType.Right;
            var createdAt = DateTimeOffset.UtcNow;
            var swipeId = _factory.Create(fromUserId, toUserId, type);

            var swipe = new Swipe(swipeId, fromUserId, toUserId, type, createdAt);

            Assert.Equal(swipeId, swipe.Id);
            Assert.Equal(fromUserId, swipe.FromUserId);
            Assert.Equal(toUserId, swipe.ToUserId);
            Assert.Equal(type, swipe.Type);
            Assert.Equal(createdAt, swipe.CreatedAt);
        }
    }
}
