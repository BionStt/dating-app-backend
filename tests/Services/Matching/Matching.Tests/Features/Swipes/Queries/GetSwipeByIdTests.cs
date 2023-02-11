using Matching.Application.Domain.Entities;
using Matching.Application.Domain.Factories;
using Matching.Application.Features.Swipes.Queries;
using Matching.Tests.Base;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Utils.Time;

namespace Matching.Tests.Features.Swipes.Queries
{
    public class GetSwipeByIdTests : MssqlIntegrationTestBase
    {
        private readonly ISwipeIdFactory _swipeIdFactory;
        private readonly IDateTimeProvider _dateTimeProvider;

        public GetSwipeByIdTests(MssqlIntegrationTestFactory factory) : base(factory)
        {
            var scope = factory.Services.CreateScope();
            _swipeIdFactory = scope.ServiceProvider.GetRequiredService<ISwipeIdFactory>();
            _dateTimeProvider = scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();
        }

        [Fact]
        public async Task GetSwipeById_WhenSwipeNotExists_Returns404()
        {

            var client = Factory.CreateClient();
            var getSwipeByIdResponse = await client.GetAsync("/api/swipes/aaa.zzz.right");
            
            Assert.Equal(HttpStatusCode.NotFound, getSwipeByIdResponse.StatusCode);
        }

        private Swipe NewSwipe()
        {
            var fromUserId = Guid.NewGuid().ToString();
            var toUserId = Guid.NewGuid().ToString();
            var type = SwipeType.Right;
            var swipeId = _swipeIdFactory.Create(fromUserId, toUserId, type);
            var createdAt = _dateTimeProvider.NowUtcOffset();
            return new Swipe(swipeId, fromUserId, toUserId, type, createdAt);
        }

        [Fact]
        public async Task GetSwipeById_WhenSwipeExists_ShouldReturnSwipe()
        {
            var swipe = NewSwipe();
            var swipeId = swipe.Id.Value;

            Context.Swipes.Add(swipe);
            await Context.SaveChangesAsync();

            var client = Factory.CreateClient();

            var foundSwipe = await client.GetFromJsonAsync<GetSwipeByIdResponse>($"/api/swipes/{swipeId}");

            Assert.NotNull(foundSwipe);

            Assert.Equal(swipeId, foundSwipe.Id);
            Assert.Equal(swipe.FromUserId, foundSwipe.FromUserId);
            Assert.Equal(swipe.ToUserId, foundSwipe.ToUserId);
            Assert.Equal(swipe.Type.ToString(), foundSwipe.Type);
            Assert.Equal(swipe.CreatedAt, foundSwipe.CreatedAt);

        }

        
    }
}
