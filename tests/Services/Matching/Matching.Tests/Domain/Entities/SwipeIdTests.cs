using Matching.Application.Domain.Entities;

namespace Matching.Tests.Domain.Entities
{
    public class SwipeIdTests
    {
        [Fact]
        public void CreateSwipeId_ShouldBeEqualToValue()
        {
            var value = "aaa.bbb.right";
            var swipeId = new SwipeId(value);
            Assert.Equal(value, swipeId.Value);
        }
    }
}
