using Matching.Application.Domain.Entities;
using Matching.Application.Domain.Factories;
using Matching.Tests.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Utils.Time;

namespace Matching.Tests.Persistence
{
    public class SwipesPersistenceTests : MssqlIntegrationTestBase
    {

        private readonly ISwipeIdFactory _swipeIdFactory;
        private readonly IDateTimeProvider _dateTimeProvider;

        public SwipesPersistenceTests(MssqlIntegrationTestFactory factory) : base(factory)
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


    }
}
