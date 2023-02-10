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
    public class GetAllRightSwipesReceivedByUserTests : IntegrationTestBase
    {
        private readonly ISwipeIdFactory _swipeIdFactory;
        private readonly IDateTimeProvider _dateTimeProvider;

        public GetAllRightSwipesReceivedByUserTests(IntegrationTestFactory factory) : base(factory)
        {
            var scope = factory.Services.CreateScope();
            _swipeIdFactory = scope.ServiceProvider.GetRequiredService<ISwipeIdFactory>();
            _dateTimeProvider = scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();
        }

        [Fact]
        public async void GetAllRightSwipesReceivedByUser_WhenUserHaveNotSwipes_ShouldReturnEmptyList()
        {
            var userId = "aaa";
            var client = Factory.CreateClient();
            var getAllRightSwipesReceivedByUserResponse = await client.GetAsync($"/api/swipes/{userId}/right/received");
            Assert.Equal(HttpStatusCode.OK, getAllRightSwipesReceivedByUserResponse.StatusCode);
            var swipes = await getAllRightSwipesReceivedByUserResponse.Content.ReadFromJsonAsync<List<GetAllRightSwipesReceivedByUserResponse>>();
            Assert.NotNull(swipes);
            Assert.Empty(swipes);
        }

        private Swipe NewSwipe(string toUser, SwipeType swipeType)
        {
            var fromUserId = Guid.NewGuid().ToString();
            var toUserId = toUser;
            var type = swipeType;
            var swipeId = _swipeIdFactory.Create(fromUserId, toUserId, type);
            var createdAt = _dateTimeProvider.NowUtcOffset();
            return new Swipe(swipeId, fromUserId, toUserId, type, createdAt);
        }

        private List<Swipe> GenerateSwipes(string toUser)
        {
            var list = new List<Swipe>();
            for (int i = 0; i < 10; i++)
            {
                list.Add(NewSwipe(toUser, SwipeType.Left));
            }
            for (int i = 0; i < 10; i++)
            {
                list.Add(NewSwipe(toUser, SwipeType.Right));
            }
            return list;
        }

        [Fact]
        public async void GetAllRightSwipesReceivedByUser_WhenUserHaveSwipes_ShouldReturnList()
        {
            var toUserId = Guid.NewGuid().ToString();
            var swipes = GenerateSwipes(toUserId);
            Context.Swipes.AddRange(swipes);
            await Context.SaveChangesAsync();

            var client = Factory.CreateClient();
            var getAllRightSwipesReceivedByUserResponse = await client.GetAsync($"/api/swipes/{toUserId}/right/received");
            Assert.Equal(HttpStatusCode.OK, getAllRightSwipesReceivedByUserResponse.StatusCode);
            var receivedSwipes = await getAllRightSwipesReceivedByUserResponse.Content.ReadFromJsonAsync<List<GetAllRightSwipesReceivedByUserResponse>>();
            Assert.NotNull(receivedSwipes);
            Assert.NotEmpty(receivedSwipes);
            Assert.Equal(10, receivedSwipes.Count);
        }



    }
}
