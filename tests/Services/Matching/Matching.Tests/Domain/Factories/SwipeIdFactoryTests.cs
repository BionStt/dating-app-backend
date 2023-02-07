using Matching.Application.Domain.Entities;
using Matching.Application.Domain.Factories;

namespace Matching.Tests.Domain.Factories
{
    public class SwipeIdFactoryTests
    {
        private readonly ISwipeIdFactory _factory;

        public SwipeIdFactoryTests() 
        {
            _factory = new SwipeIdFactory();
        }

        [Fact]
        public void CreateSwipeId_SwipeId_ShouldBeEqualToValue()
        {

            var fromUserId = "aaa";
            var toUserId = "bbb";
            var type = SwipeType.Right;
            var value = "aaa.bbb.right";
            var expected = new SwipeId(value);

            var actual = _factory.Create(fromUserId, toUserId, type);

            Assert.Equal(expected, actual);
        }
    }
}
