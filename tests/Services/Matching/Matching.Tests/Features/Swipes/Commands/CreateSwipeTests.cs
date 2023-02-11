using Matching.Application.Features.Swipes.Commands;
using Matching.Tests.Base;
using System.Net.Http.Json;
using System.Net;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using EventBus.Messages.Events;

namespace Matching.Tests.Features.Swipes.Commands
{
    public class CreateSwipeTests : IntegrationTestBase 
    {
        private readonly ITestHarness _harness;
        public CreateSwipeTests(IntegrationTestFactory factory) : base(factory)
        {
            var scope = Factory.Services.CreateScope();
            _harness = scope.ServiceProvider.GetRequiredService<ITestHarness>();
        }

        [Fact]
        public async Task CreateRightSwipe_WhenSwipeNotExists_StatusShouldReturn_Created()
        {
           
            var command = new CreateSwipeCommand
            {
                FromUserId = "aaa",
                ToUserId = "bbb",
                Type = "right"
            };

            await _harness.Start();
            var client = Factory.CreateClient();
            var createSwipeResponse = await client.PostAsJsonAsync($"/api/swipes", command);

            Assert.Equal(HttpStatusCode.Created, createSwipeResponse.StatusCode);

            var rightSwipeEventPublished = await _harness.Published.Any<RightSwipeEvent>();
            Assert.True(rightSwipeEventPublished);
            await _harness.Stop();
        }

        [Fact]
        public async Task CreateRightSwipe_WhenExists_StatusShouldReturn_UnprocessableEntity()
        {
            var command = new CreateSwipeCommand
            {
                FromUserId = "bbb",
                ToUserId = "ccc",
                Type = "right"
            };

            var client = Factory.CreateClient();
            var createSwipeResponse = await client.PostAsJsonAsync($"/api/swipes", command);

            Assert.Equal(HttpStatusCode.Created, createSwipeResponse.StatusCode);


            var createExistentSwipeResponse = await client.PostAsJsonAsync($"/api/swipes", command);

            Assert.Equal(HttpStatusCode.UnprocessableEntity, createExistentSwipeResponse.StatusCode);
        }


        [Fact]
        public async Task CreateLeftSwipe_WhenSwipeNotExists_StatusShouldReturn_Created()
        {
            var command = new CreateSwipeCommand
            {
                FromUserId = Guid.NewGuid().ToString(),
                ToUserId = Guid.NewGuid().ToString(),
                Type = "left"
            };

            await _harness.Start();
            var client = Factory.CreateClient();
            var createSwipeResponse = await client.PostAsJsonAsync($"/api/swipes", command);

            Assert.Equal(HttpStatusCode.Created, createSwipeResponse.StatusCode);

            var rightSwipeEventPublished = await _harness.Published.Any<LeftSwipeEvent>();
            Assert.True(rightSwipeEventPublished);
            await _harness.Stop();
        }

        [Fact]
        public async Task CreateSwipe_WhenUsersMatch_StatusShouldReturn_Created()
        {
            var fromUserId = Guid.NewGuid().ToString();
            var toUserId = Guid.NewGuid().ToString();

            var partnerOneCommand = new CreateSwipeCommand
            {
                FromUserId = fromUserId,
                ToUserId = toUserId,
                Type = "right"
            };

            var partnerTwoCommand = new CreateSwipeCommand
            {
                FromUserId = toUserId,
                ToUserId = fromUserId,
                Type = "right"
            };

            var client = Factory.CreateClient();
            await _harness.Start();

            var partnerOneResponse = await client.PostAsJsonAsync($"/api/swipes", partnerOneCommand);
            Assert.Equal(HttpStatusCode.Created, partnerOneResponse.StatusCode);


            var partnerTwoResponse = await client.PostAsJsonAsync($"/api/swipes", partnerTwoCommand);
            Assert.Equal(HttpStatusCode.Created, partnerTwoResponse.StatusCode);

            var matchEventPublished = await _harness.Published.Any<MatchEvent>();

            Assert.True(matchEventPublished);

            await _harness.Stop();
        }

        [Fact]
        public async Task CreateSwipe_WhenInvalidInput_ShouldReturn_BadRequest()
        {
            var command = new CreateSwipeCommand
            {
                FromUserId = "ccc",
                ToUserId = "ddd",
                Type = "righggt"
            };

            var client = Factory.CreateClient();
            var createSwipeResponse = await client.PostAsJsonAsync($"/api/swipes", command);

            Assert.Equal(HttpStatusCode.BadRequest, createSwipeResponse.StatusCode);
        }
    }
}
