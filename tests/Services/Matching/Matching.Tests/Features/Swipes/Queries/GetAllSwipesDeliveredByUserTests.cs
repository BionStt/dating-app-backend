using Matching.Application.Domain.Entities;
using Matching.Application.Domain.Factories;
using Matching.Application.Features.Swipes.Queries;
using Matching.Tests.Base;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using System.Net;
using Utils.Time;

namespace Matching.Tests.Features.Swipes.Queries
{
    public class GetAllSwipesDeliveredByUserTests : MssqlIntegrationTestBase
    {
        private readonly ISwipeIdFactory _swipeIdFactory;
        private readonly IDateTimeProvider _dateTimeProvider;
        public GetAllSwipesDeliveredByUserTests(MssqlIntegrationTestFactory factory) : base(factory)
        {
            var scope = factory.Services.CreateScope();
            _swipeIdFactory = scope.ServiceProvider.GetRequiredService<ISwipeIdFactory>();
            _dateTimeProvider = scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();
        }

        [Fact]
        public async void GetAllSwipesDeliveredByUser_WhenUserHaveNotDeliverSwipes_ShouldReturnEmptyList()
        {
            var userId = "aaa";
            var client = Factory.CreateClient();
            var getAllSwipesReceivedByUserResponse = await client.GetAsync($"/api/swipes/{userId}/delivered");
            Assert.Equal(HttpStatusCode.OK, getAllSwipesReceivedByUserResponse.StatusCode);
            var swipes = await getAllSwipesReceivedByUserResponse.Content.ReadFromJsonAsync<List<GetAllSwipesDeliveredByUserResponse>>();
            Assert.NotNull(swipes);
            Assert.Empty(swipes);
        }

        private Swipe NewSwipe(string fromUser)
        {
            var fromUserId = fromUser;
            var toUserId = Guid.NewGuid().ToString();
            var type = SwipeType.Right;
            var swipeId = _swipeIdFactory.Create(fromUserId, toUserId, type);
            var createdAt = _dateTimeProvider.NowUtcOffset();
            return new Swipe(swipeId, fromUserId, toUserId, type, createdAt);
        }

        private List<Swipe> GenerateSwipes(string toUser)
        {
            var list = new List<Swipe>();
            for (int i = 0; i < 10; i++)
            {
                list.Add(NewSwipe(toUser));
            }
            return list;
        }

        [Fact]
        public async void GetAllSwipesDeliveredByUser_WhenUserDeliverSwipes_ShouldReturnList()
        {
            var fromUserId = Guid.NewGuid().ToString();
            var swipes = GenerateSwipes(fromUserId);

            Context.Swipes.AddRange(swipes);
            await Context.SaveChangesAsync();

            var client = Factory.CreateClient();
            var getAllSwipesDeliveredByUserResponse = await client.GetAsync($"/api/swipes/{fromUserId}/delivered");
            Assert.Equal(HttpStatusCode.OK, getAllSwipesDeliveredByUserResponse.StatusCode);
            var deliveredSwipes = await getAllSwipesDeliveredByUserResponse.Content.ReadFromJsonAsync<List<GetAllSwipesDeliveredByUserResponse>>();
            Assert.NotNull(deliveredSwipes);
            Assert.NotEmpty(deliveredSwipes);
            Assert.Equal(10, deliveredSwipes.Count);
        }


    }
}
