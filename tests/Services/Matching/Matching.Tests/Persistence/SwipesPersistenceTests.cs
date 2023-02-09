using Matching.Application.Domain.Entities;
using Matching.Application.Domain.Factories;
using Matching.Tests.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Utils.Time;

namespace Matching.Tests.Persistence
{
    public class SwipesPersistenceTests : IntegrationTestBase
    {

        private readonly ISwipeIdFactory _swipeIdFactory;
        private readonly IDateTimeProvider _dateTimeProvider;

        public SwipesPersistenceTests(IntegrationTestFactory factory) : base(factory)
        {
            var scope = factory.Services.CreateScope();
            _swipeIdFactory = scope.ServiceProvider.GetRequiredService<ISwipeIdFactory>();
            _dateTimeProvider = scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();
        }

        [Fact]
        public async Task CreateSwipe_When_SwipeIdxExists_ShouldThrow_DbUpdateException()
        {

            var swipe = new Swipe(
                  _swipeIdFactory.Create("xxx", "zzz", SwipeType.Right),
                  "xxx",
                  "zzz",
                  SwipeType.Right,
                  _dateTimeProvider.NowUtcOffset()
              );

            var newSwipe = new Swipe(
                _swipeIdFactory.Create("xxx", "zzz", SwipeType.Left),
                "xxx",
                "zzz",
                SwipeType.Left,
                _dateTimeProvider.NowUtcOffset()
            );


            var task = async () =>
            {
                Context.Swipes.Add(swipe);
                await Context.SaveChangesAsync();
                Context.Swipes.Add(newSwipe);
                await Context.SaveChangesAsync();
            };


            await Assert.ThrowsAsync<DbUpdateException>(task);
        }

        [Fact]
        public async Task CreateSwipe_When_FindCreatedSwipeById_ShouldReturnSwipe()
        {
            var fromUserId = "aaa";
            var toUserId = "bbb";
            var type = SwipeType.Right;
            var swipeId = _swipeIdFactory.Create(fromUserId, toUserId, type);
            var createdAt = _dateTimeProvider.NowUtcOffset();

            var swipe = new Swipe(swipeId, fromUserId, toUserId, type, createdAt);
            Context.Swipes.Add(swipe);
            await Context.SaveChangesAsync();

            var foundSwipe = await Context.Swipes.FindAsync(swipeId);

            Assert.NotNull(foundSwipe);

            Assert.Equal(swipe.Id, foundSwipe.Id);
            Assert.Equal(swipe.FromUserId, foundSwipe.FromUserId);
            Assert.Equal(swipe.ToUserId, foundSwipe.ToUserId);
            Assert.Equal(swipe.Type, foundSwipe.Type);
            Assert.Equal(swipe.CreatedAt, foundSwipe.CreatedAt);   
        }

    }
}
